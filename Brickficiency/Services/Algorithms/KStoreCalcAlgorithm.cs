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
    public class KStoreCalcAlgorithm : BaseAlgorithm, IMatchAlgorithm
    {
        private List<Store> _storeList;
        private List<Item> _itemList;

        private long _solutionsChecked;

        public KStoreCalcAlgorithm(
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
            KStoreCalc(numberOfStores, _storeList, _itemList);
        }

        private void KStoreCalc(int k, List<Store> storeList, List<Item> itemList)
        {
            if (k == 1)
            {
                OneStoreCalc(storeList, itemList);
            }
            else
            {
                // The stores are sorted by how many of the first item they have, and then by 2nd, 3rd, and 4th, etc.
                // Thus a valid solution must contain one of the first store in the list.  Once a store in the list has 0 of the first item
                // the rest will have 0 so it makes no sense to consider them as the first store since the rest of the store can't have that item
                // either, so no solution will ever be found.  This idea can be applied to the second store, etc., although it is more complicated.
                // So here we compute the last index of the first (up to) 5 items on the list. The last index of the 5th should be the final index.  
                // The last index of the rest might be smaller. I haven't figured out exactly how to use this information for items 2-5 yet since
                // it gets a little more complicated.
                // CAC, 6/25/15
                int numToTrack = itemList.Count > 5 ? 5 : itemList.Count;
                int[] lastnonzeroindex = new int[numToTrack];
                for (int item = 0; item < numToTrack; item++)
                {
                    int j = storeList.Count - 1;

                    while (j >= 0 && storeList[j].getQty(itemList[item].extid) == 0)
                    {
                        j--;
                    }

                    lastnonzeroindex[item] = j;
                }
                //Debug.WriteLine("LastNonZero: " + intArrayToString(lastnonzeroindex));

                // Need to add one to the second argument since it is exclusive.
                Parallel.For(0, lastnonzeroindex[0] + 1, store1 =>
                {
                    if (IsCancellationPending) { return; }
                    // Do the next k stores have enough of the first element?  
                    // If not, none of the rest will so quit. CAC, 6/25/15
                    int totalQtyFirst = 0;
                    int lastToCheck = Math.Min(storeList.Count - 1, store1 + k);
                    for (int i = store1; i < lastToCheck; i++)
                    {
                        totalQtyFirst += storeList[i].getQty(itemList[0].extid);
                    }

                    if (totalQtyFirst < itemList[0].qty)
                    {
                        return;
                    }

                    int[] start = new int[k];
                    int[] end = new int[k];
                    for (int i = 0; i < k; i++)
                    {
                        start[i] = store1 + i;
                        end[i] = storeList.Count - k + i;
                        // The version below doesn't work.  We have to use a different method of omitting more
                        // possibilities based on lastnonzeroindex[i] for i>0.  Still need to think about this.
                        // I'm leaving it here commented out to remind me that I tried it and realized
                        // that it isn't correct.
                        // CAC, 7/2/15.
                        //end[i] = (i < numToTrack) ? lastnonzeroindex[i] : storeList.Count - k + i;
                    }

                    end[0] = store1;
                    KSubsetGenerator subs = new KSubsetGenerator(storeList.Count, start, end);

                    while (subs.hasNext())
                    {
                        if (IsCancellationPending)
                        {
                            break;
                        }

                        int[] current = subs.next();
                        Interlocked.Increment(ref _solutionsChecked);
                        bool fail = false;

                        foreach (Item item in itemList)
                        {
                            int totalQty = 0;
                            for (int i = 0; i < k; i++)
                            {
                                totalQty += storeList[current[i]].getQty(item.extid);
                            }

                            if (totalQty < item.qty)
                            {
                                fail = true;
                                break;
                            }
                        }

                        if (!fail)
                        {
                            List<string> storeNames = new List<string>();
                            for (int j = 0; j < k; j++)
                            {
                                storeNames.Add(storeList[current[j]].getName());
                            }

                            OnMatchFound(storeNames);
                        }
                    }

                    Progress();
                });
            }
        }

        protected void Progress()
        {
            OnProgress(_solutionsChecked);
        }
    }
}
