using Brickficiency.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brickficiency.Common
{
    public static class SortUtil
    {
        public static List<Item> SortItemsByStoreAvailability(ICollection<Item> itemList)
        {
            return itemList.OrderBy(i => i.availstores).ToList();
        }

        /// <remarks>
        /// This sorts the stores so they are in decending order by the number of the first item on the wanted list.
        /// If they have the same number, then they are ordered by the number of the second item, etc.
        /// </remarks>
        public static List<Store> SortStoresByNumberOfFirstSeveralItems(List<Store> storeList, List<Item> itemList)
        {
            StoreComparer sc = new StoreComparer(itemList);
            storeList.Sort(sc);
            return storeList;
        }
    }
}
