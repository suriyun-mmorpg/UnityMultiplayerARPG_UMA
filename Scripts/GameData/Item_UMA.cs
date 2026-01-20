using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UMA;

namespace MultiplayerARPG
{
    public partial class Item
    {
        [Header("UMA Configs")]
        [FormerlySerializedAs("umaRaceReceipeSlots")]
        public UmaRaceRecipeSlots[] umaRaceRecipeSlots;

        private Dictionary<string, UMATextRecipe[]> cacheUmaRecipeSlot;
        public Dictionary<string, UMATextRecipe[]> UmaRecipeSlot
        {
            get
            {
                if (cacheUmaRecipeSlot == null)
                {
                    cacheUmaRecipeSlot = new Dictionary<string, UMATextRecipe[]>();
                    if (umaRaceRecipeSlots != null)
                    {
                        foreach (UmaRaceRecipeSlots umaRaceRecipeSlot in umaRaceRecipeSlots)
                        {
                            if (umaRaceRecipeSlot.raceData == null || string.IsNullOrEmpty(umaRaceRecipeSlot.raceData.raceName))
                                continue;
                            cacheUmaRecipeSlot[umaRaceRecipeSlot.raceData.raceName] = umaRaceRecipeSlot.recipes;
                        }
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
