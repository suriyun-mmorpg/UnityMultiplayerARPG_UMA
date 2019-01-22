using System.Collections;
using System.Collections.Generic;
using UMA;

namespace MultiplayerARPG
{
    public partial class Item
    {
        public UmaReceiptSlot[] umaReceiptSlots;

        private Dictionary<string, UmaReceiptSlot> cacheUmaReceiptSlot;
        private Dictionary<string, UmaReceiptSlot> CacheUmaReceiptSlot
        {
            get
            {
                if (cacheUmaReceiptSlot == null)
                {
                    cacheUmaReceiptSlot = new Dictionary<string, UmaReceiptSlot>();
                    foreach (UmaReceiptSlot umaReceiptSlot in umaReceiptSlots)
                    {
                        if (umaReceiptSlot.raceData == null || string.IsNullOrEmpty(umaReceiptSlot.raceData.raceName))
                            continue;
                        cacheUmaReceiptSlot.Add(umaReceiptSlot.raceData.raceName, umaReceiptSlot);
                    }
                }
                return cacheUmaReceiptSlot;
            }
        }
    }

    [System.Serializable]
    public struct UmaReceiptSlot
    {
        public RaceData raceData;
        public UMATextRecipe recipe;
        public string slot;
    }

}
