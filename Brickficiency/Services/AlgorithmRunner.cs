using Brickficiency.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;

namespace Brickficiency.Services
{
    public class AlgorithmRunner
    {
        private readonly AlgorithmFactory _algorithmFactory;

        public AlgorithmRunner()
        {
            _algorithmFactory = new AlgorithmFactory();
        }

        public void Run(
            Settings settings, 
            MatchAlgorithmType algorithmType, 
            AlgorithmMode mode,
            IAlgorithmInteraction algorithmInteraction,
            List<Store> storeList,
            List<Item> itemList)
        {
            var algorithm = _algorithmFactory.CreateAlgorithm(algorithmType, storeList, itemList, algorithmInteraction);
            algorithm.PreProcess();

            switch (mode)
            {
                case AlgorithmMode.Complete:
                    RunComplete(algorithm, settings);
                    break;
                case AlgorithmMode.Approximation:
                    RunApproximation(algorithm, settings, storeList);
                    break;
                default:
                    throw new NotSupportedException("The algorithm mode is not supported: " + mode.ToString());
            }
        }

        private void RunComplete(IMatchAlgorithm algorithm, Settings settings)
        {
            for(int i = settings.minstores; i <= settings.maxstores; i++)
            {
                algorithm.Run(i);
            }
        }

        private void RunApproximation(IMatchAlgorithm algorithm, Settings settings, List<Store> storeList)
        {
            var timeoutTimer = new Timer();
            timeoutTimer.Elapsed += new ElapsedEventHandler(
                (object sender, ElapsedEventArgs e) =>
                {
                    algorithm.Cancel();
                    timeoutTimer.Stop();
                });

            var interval = TimeSpan.FromSeconds(settings.approxtime);

            timeoutTimer.Interval = interval.TotalMilliseconds;

            for (int k = settings.minstores; k <= settings.maxstores; k++)
            {
                if (!algorithm.IsCancellationPending)
                {
                    BeginCalculation(k, storeList.Count);
                    timeoutTimer.Start();
                    algorithm.Run(k);
                    timeoutTimer.Stop();
                    EndCalculation(k);
                }
            }
        }

        private void BeginCalculation(int k, int numTotalStores)
        {
            previousPrinted = 0;
            AddStatus(Environment.NewLine + "Calculating " + k + " store solutions...");
            if (k > 2)
            {
                AddStatus(Environment.NewLine + "  Millions of solutions checked: ");
            }

            var stopWatch = new Stopwatch();
            //shortcount = 0;
            longcount = 0;

            // Added to this method by CAC, 7/6/15 to fix a bug related to the report.
            var matches = new List<FinalMatch>();

            matchesfoundcount = 0;
            stopWatch.Start();
            SetProgressBar(numTotalStores);
            running = true;
        }

        private void EndCalculation(int k)
        {
            running = false;
            AddStatus(Environment.NewLine + "  Total solutions checked: " + longcount);

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, 
                ts.Minutes, 
                ts.Seconds,
                ts.Milliseconds / 10);

            AddStatus(Environment.NewLine);
            ResetProgressBar();

            if (calcWorker.CancellationPending || stopAlgorithmEarly)
            {
                AddStatus("  ** Not all possible combinations were tried **" + Environment.NewLine);
            }

            if (matches.Count > 0)
            {
                AddStatus("  " + matchesfoundcount + " Matches Found in " + elapsedTime + Environment.NewLine);
                WriteDebug();
                ReportMiddle();
            }
            else
            {
                AddStatus("  No Matches Found in " + elapsedTime + Environment.NewLine);
                WriteDebug();
            }
        }
    }
}
