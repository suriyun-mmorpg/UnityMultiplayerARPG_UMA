using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;

namespace MultiplayerARPG
{
    public partial class Item
    {
        [Header("UMA Configs")]
        public UmaRaceReceiptSlots[] umaRaceReceiptSlots;

        private Dictionary<string, UmaReceiptSlot[]> cacheUmaReceiptSlot;
        private Dictionary<string, UmaReceiptSlot[]> CacheUmaReceiptSlot
        {
            get
            {
                if (cacheUmaReceiptSlot == null)
                {
                    cacheUmaReceiptSlot = new Dictionary<string, UmaReceiptSlot[]>();
                    foreach (UmaRaceReceiptSlots umaRaceReceiptSlot in umaRaceReceiptSlots)
                    {
                        if (umaRaceReceiptSlot.raceData == null || string.IsNullOrEmpty(umaRaceReceiptSlot.raceData.raceName))
                            continue;
                        cacheUmaReceiptSlot[umaRaceReceiptSlot.raceData.raceName] = umaRaceReceiptSlot.recipeSlots;
                    }
                }
                return cacheUmaReceiptSlot;
            }
        }
    }

    [System.Serializable]
    public struct UmaRaceReceiptSlots
    {
        public RaceData raceData;
        public UmaReceiptSlot[] recipeSlots;
    }

    [System.Serializable]
    public struct UmaReceiptSlot
    {
        public UMATextRecipe recipe;
        public string slot;
    }
}
