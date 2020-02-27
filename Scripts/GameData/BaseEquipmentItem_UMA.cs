using System.Collections;
using System.Collections.Generic;
using UMA;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class BaseEquipmentItem
    {
        [Header("UMA Configs")]
        // TODO: I know `Receipe` should be `Recipe` but I don't want to break other developer projects
        // May fix this later
        public UmaRaceRecipeSlots[] umaRaceReceipeSlots;

        private Dictionary<string, UMATextRecipe[]> cacheUmaRecipeSlot;
        public Dictionary<string, UMATextRecipe[]> UmaRecipeSlot
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
    }
}
