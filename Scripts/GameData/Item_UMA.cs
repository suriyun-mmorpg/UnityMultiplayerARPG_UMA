using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;

namespace MultiplayerARPG
{
    public partial class Item
    {
        [Header("UMA Configs")]
        public UmaRaceReceipeSlots[] umaRaceReceipeSlots;

        private Dictionary<string, UMATextRecipe[]> cacheUmaReceipeSlot;
        public Dictionary<string, UMATextRecipe[]> CacheUmaReceipeSlot
        {
            get
            {
                if (cacheUmaReceipeSlot == null)
                {
                    cacheUmaReceipeSlot = new Dictionary<string, UMATextRecipe[]>();
                    foreach (UmaRaceReceipeSlots umaRaceReceipeSlot in umaRaceReceipeSlots)
                    {
                        if (umaRaceReceipeSlot.raceData == null || string.IsNullOrEmpty(umaRaceReceipeSlot.raceData.raceName))
                            continue;
                        cacheUmaReceipeSlot[umaRaceReceipeSlot.raceData.raceName] = umaRaceReceipeSlot.recipes;
                    }
                }
                return cacheUmaReceipeSlot;
            }
        }
    }

    [System.Serializable]
    public struct UmaRaceReceipeSlots
    {
        public RaceData raceData;
        public UMATextRecipe[] recipes;
    }
}
