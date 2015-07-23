using Brickficiency.Classes;
using Brickficiency.Services.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brickficiency.Services
{
    public class AlgorithmFactory
    {
        public IMatchAlgorithm CreateAlgorithm(
            MatchAlgorithmType algorithmType,
            List<Store> storeList,
            List<Item> itemList,
            IAlgorithmInteraction algorithmInteraction)
        {
            switch (algorithmType)
            {
                case MatchAlgorithmType.Linear:
                    return new LinearAlgorithm(
                        storeList,
                        itemList,
                        algorithmInteraction);
                case MatchAlgorithmType.KStoreCalc:
                    return new KStoreCalcAlgorithm(
                        storeList,
                        itemList,
                        algorithmInteraction);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
