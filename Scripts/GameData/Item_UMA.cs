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

        private Dictionary<string, UMATextRecipe[]> cacheUmaReceiptSlot;
        public Dictionary<string, UMATextRecipe[]> CacheUmaReceiptSlot
        {
            get
            {
                if (cacheUmaReceiptSlot == null)
                {
                    cacheUmaReceiptSlot = new Dictionary<string, UMATextRecipe[]>();
                    foreach (UmaRaceReceiptSlots umaRaceReceiptSlot in umaRaceReceiptSlots)
                    {
                        if (umaRaceReceiptSlot.raceData == null || string.IsNullOrEmpty(umaRaceReceiptSlot.raceData.raceName))
                            continue;
                        cacheUmaReceiptSlot[umaRaceReceiptSlot.raceData.raceName] = umaRaceReceiptSlot.recipes;
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
        public UMATextRecipe[] recipes;
    }
}
