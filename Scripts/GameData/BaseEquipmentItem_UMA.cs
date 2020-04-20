using System.Collections;
using System.Collections.Generic;
using UMA;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public partial class BaseEquipmentItem
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
                    foreach (UmaRaceRecipeSlots umaRaceRecipeSlot in umaRaceRecipeSlots)
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
