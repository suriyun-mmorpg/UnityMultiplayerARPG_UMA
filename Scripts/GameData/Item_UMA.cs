using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;

namespace MultiplayerARPG
{
    public partial class Item
    {
        [Header("UMA Configs")]
        public UmaRaceRecipeSlots[] umaRaceReceipeSlots;

        private Dictionary<string, UMATextRecipe[]> cacheUmaRecipeSlot;
        public Dictionary<string, UMATextRecipe[]> CacheUmaRecipeSlot
        {
            get
            {
                if (cacheUmaRecipeSlot == null)
                {
                    cacheUmaRecipeSlot = new Dictionary<string, UMATextRecipe[]>();
                    foreach (UmaRaceRecipeSlots umaRaceReceipeSlot in umaRaceReceipeSlots)
                    {
                        if (umaRaceReceipeSlot.raceData == null || string.IsNullOrEmpty(umaRaceReceipeSlot.raceData.raceName))
                            continue;
                        cacheUmaRecipeSlot[umaRaceReceipeSlot.raceData.raceName] = umaRaceReceipeSlot.recipes;
                    }
                }
                return cacheUmaRecipeSlot;
            }
        }
    }

    [System.Serializable]
    public struct UmaRaceRecipeSlots
    {
        public RaceData raceData;
        public UMATextRecipe[] recipes;
    }
}
