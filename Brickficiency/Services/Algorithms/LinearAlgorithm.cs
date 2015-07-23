using Brickficiency.Classes;
using Brickficiency.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Brickficiency.Services.Algorithms
{
    public class LinearAlgorithm : BaseAlgorithm, IMatchAlgorithm
    {
        private List<Store> _storeList;
        private List<Item> _itemList;

        private long _solutionsChecked;

        public LinearAlgorithm(
            List<Store> storeList,
            List<Item> itemList,
            IAlgorithmInteraction algorithmInteraction)
            : base(algorithmInteraction)
        {
            _storeList = storeList;
            _itemList = itemList;
        }

        public void PreProcess()
        {
            _itemList = SortUtil.SortItemsByStoreAvailability(_itemList);
            _storeList = SortUtil.SortStoresByNumberOfFirstSeveralItems(_storeList, _itemList);
        }

        public void Run(int numberOfStores)
        {
            _solutionsChecked = 0;
            RunForNumberOfStores(numberOfStores, _storeList, _itemList);
        }

        private void RunForNumberOfStores(int numberOfStores, List<Store> storeList, List<Item> itemList)
        {
            switch (numberOfStores)
            {
                case 1:
                    OneStoreCalc(storeList, itemList);
                    break;
                case 2:
                    TwoStoreCalc(storeList, itemList);
                    break;
                case 3:
                    ThreeStoreCalc(storeList, itemList);
                    break;
                case 4:
                    FourStoreCalc(storeList, itemList);
                    break;
                case 5:
                    FiveStoreCalc(storeList, itemList);
                    break;
            }
        }

        private void TwoStoreCalc(List<Store> storeList, List<Item> itemList)
        {
            // This is like a for loop that makes the variable store1 go from 0 (inclusive) to storeList.Count (exclusive).
            // The only difference is that this one will run the loops in parallel.  All of the hard work to accomplish 
            // running it in parallel is done behind the scenes.  This only works because what each iteration of the loop
            // is doing is independent of the others.  If each loop depended on the previous loop this wouldn't work properly.
            Parallel.For(0, storeList.Count, store1 =>
            {
                for (int store2 = store1 + 1; store2 < storeList.Count; store2++)
                {
                    if (IsCancellationPending) { return; }
                    Interlocked.Increment(ref _solutionsChecked);
                    bool fail = false;
                    foreach (Item item in itemList)
                    {
                        if (storeList[store1].getQty(item.extid) +
                            storeList[store2].getQty(item.extid) < item.qty)
                        {
                            fail = true;
                            break;
                        }
                    }

                    if (!fail)
                    {
                        List<string> storeNames = new List<string>();
                        storeNames.Add(storeList[store1].getName());
                        storeNames.Add(storeList[store2].getName());
                        OnMatchFound(storeNames);
                    }
                }

                Progress();
            });
        }

        private void ThreeStoreCalc(List<Store> storeList, List<Item> itemList)
        {
            Parallel.For(0, storeList.Count, store1 =>
            {
                for (int store2 = store1 + 1; store2 < storeList.Count; store2++)
                {
                    if (IsCancellationPending) { return; }
                    for (int store3 = store2 + 1; store3 < storeList.Count; store3++)
                    {
                        Interlocked.Increment(ref _solutionsChecked);
                        if (IsCancellationPending) { return; }
                        bool fail = false;
                        foreach (Item item in itemList)
                        {
                            if (storeList[store1].getQty(item.extid) +
                                storeList[store2].getQty(item.extid) +
                                storeList[store3].getQty(item.extid) < item.qty)
                            {
                                fail = true;
                                break;
                            }
                        }

                        if (!fail)
                        {
                            List<string> storeNames = new List<string>();
                            storeNames.Add(storeList[store1].getName());
                            storeNames.Add(storeList[store2].getName());
                            storeNames.Add(storeList[store3].getName());
                            OnMatchFound(storeNames);
                        }
                    }
                }

                Progress();
            });
        }

        private void FourStoreCalc(List<Store> storeList, List<Item> itemList)
        {
            Parallel.For(0, storeList.Count, store1 =>
            {
                for (int store2 = store1 + 1; store2 < storeList.Count; store2++)
                {
                    if (IsCancellationPending) { return; }
                    for (int store3 = store2 + 1; store3 < storeList.Count; store3++)
                    {
                        if (IsCancellationPending) { return; }
                        for (int store4 = store3 + 1; store4 < storeList.Count; store4++)
                        {
                            if (IsCancellationPending) { return; }
                            Interlocked.Increment(ref _solutionsChecked);
                            bool fail = false;
                            foreach (Item item in itemList)
                            {
                                if (storeList[store1].getQty(item.extid) +
                                    storeList[store2].getQty(item.extid) +
                                    storeList[store3].getQty(item.extid) +
                                    storeList[store4].getQty(item.extid) < item.qty)
                                {
                                    fail = true;
                                    break;
                                }
                            }

                            if (!fail)
                            {
                                List<string> storeNames = new List<string>();
                                storeNames.Add(storeList[store1].getName());
                                storeNames.Add(storeList[store2].getName());
                                storeNames.Add(storeList[store3].getName());
                                storeNames.Add(storeList[store4].getName());
                                OnMatchFound(storeNames);
                            }
                        }
                    }
                }

                Progress();
            });
        }

        private void FiveStoreCalc(List<Store> storeList, List<Item> itemList)
        {
            Parallel.For(0, storeList.Count, store1 =>
            {
                for (int store2 = store1 + 1; store2 < storeList.Count; store2++)
                {
                    if (IsCancellationPending) { return; }
                    for (int store3 = store2 + 1; store3 < storeList.Count; store3++)
                    {
                        if (IsCancellationPending) { return; }
                        for (int store4 = store3 + 1; store4 < storeList.Count; store4++)
                        {
                            if (IsCancellationPending) { return; }
                            for (int store5 = store4 + 1; store5 < storeList.Count; store5++)
                            {
                                if (IsCancellationPending) { return; }
                                Interlocked.Increment(ref _solutionsChecked);
                                bool fail = false;
                                foreach (Item item in itemList)
                                {
                                    if (storeList[store1].getQty(item.extid) +
                                        storeList[store2].getQty(item.extid) +
                                        storeList[store3].getQty(item.extid) +
                                        storeList[store4].getQty(item.extid) +
                                        storeList[store5].getQty(item.extid) < item.qty)
                                    {
                                        fail = true;
                                        break;
                                    }
                                }

                                if (!fail)
                                {
                                    List<string> storeNames = new List<string>();
                                    storeNames.Add(storeList[store1].getName());
                                    storeNames.Add(storeList[store2].getName());
                                    storeNames.Add(storeList[store3].getName());
                                    storeNames.Add(storeList[store4].getName());
                                    storeNames.Add(storeList[store5].getName());
                                    OnMatchFound(storeNames);
                                }
                            }
                        }
                    }
                }

                Progress();
            });
        }

        protected void Progress()
        {
            OnProgress(_solutionsChecked);
        }
    }
}