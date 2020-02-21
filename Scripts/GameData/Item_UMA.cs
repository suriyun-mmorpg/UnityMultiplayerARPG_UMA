using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;

namespace MultiplayerARPG
{
    public partial class Item
    {
        [Header("UMA Configs")]
        // TODO: I know `Receipe` should be `Recipe` but I don't want to break other developer projects
        // May fix this later
        public UmaRaceRecipeSlots[] umaRaceReceipeSlots;

        private Dictionary<string, UMATextRecipe[]> cacheUmaRecipeSlot;
        public Dictionary<string, UMATextRecipe[]> CacheUmaRecipeSlot
        {
            get
            {
                if (cacheUmaRecipeSlot == null)
                {
                    cacheUmaRecipeSlot = new Dictionary<string, UMATextRecipe[]>();
                    foreach (UmaRaceRecipeSlots umaRaceRecipeSlot in umaRaceReceipeSlots)
                    {
                        if (umaRaceRecipeSlot.raceData == null || string.IsNullOrEmpty(umaRaceRecipeSlot.raceData.raceName))
                            continue;
                        cacheUmaRecipeSlot[umaRaceRecipeSlot.raceData.raceName] = umaRaceRecipeSlot.recipes;
                    }
                }
                return cacheUmaRecipeSlot;
            }
        }
        public Dictionary<string, UMATextRecipe[]> UmaRecipeSlot { get { return CacheUmaRecipeSlot; } }
    }

    [System.Serializable]
    public struct UmaRaceRecipeSlots
    {
        public RaceData raceData;
        public UMATextRecipe[] recipes;
    }
}
