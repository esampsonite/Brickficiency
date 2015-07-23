using Brickficiency.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brickficiency.Services.Algorithms
{
    public abstract class BaseAlgorithm
    {
        private readonly IAlgorithmInteraction _algorithmInteraction;

        public BaseAlgorithm(IAlgorithmInteraction algorithmInteraction)
        {
            _algorithmInteraction = algorithmInteraction;
        }

        protected void OnMatchFound(ICollection<string> storeNames)
        {
            _algorithmInteraction.OnMatchFound(storeNames);
        }

        public bool IsCancellationPending
        {
            get;
            private set;
        }

        public void Cancel()
        {
            IsCancellationPending = true;
        }

        protected void OnProgress(long solutionsChecked)
        {
            _algorithmInteraction.OnProgress(solutionsChecked);
        }

        protected void OneStoreCalc(List<Store> storeList, List<Item> itemList)
        {
            for (int storeIndex = 0; storeIndex < storeList.Count; storeIndex++)
            {
                bool didFindMatch = true;
                foreach (Item item in itemList)
                {
                    if (storeList[storeIndex].getQty(item.extid) < item.qty)
                    {
                        didFindMatch = false;
                        break;
                    }
                }

                if (didFindMatch)
                {
                    List<string> storeNames = new List<string>();
                    storeNames.Add(storeList[storeIndex].getName());
                    OnMatchFound(storeNames);
                    OnProgress(storeIndex);
                }
            }
        }
    }
}
