using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;
using UMA.CharacterSystem;

namespace MultiplayerARPG
{
    [RequireComponent(typeof(DynamicCharacterAvatar))]
    public class CharacterModelUMA : CharacterModel
    {
        private DynamicCharacterAvatar cacheUmaAvatar;
        public DynamicCharacterAvatar CacheUmaAvatar
        {
            get
            {
                if (cacheUmaAvatar == null)
                    cacheUmaAvatar = GetComponent<DynamicCharacterAvatar>();
                return cacheUmaAvatar;
            }
        }

        public void ApplyUmaAvatar(UmaAvatarData avatarData)
        {
            if (CacheUmaAvatar == null)
            {
                Debug.LogWarning("[CharacterModelUMA] Uma avatar or applier is empty, cannot change avatar appearances");
                return;
            }
            UmaRace race = gameInstance.umaRaces[avatarData.raceIndex];
            UmaRaceGender gender = race.genders[avatarData.genderIndex];
            CacheUmaAvatar.ChangeRace(gender.raceData.name);
            SharedColorTable colorTable;
            int i;
            // Set skin color, eyes color, hair color (or other things up to your settings)
            for (i = 0; i < avatarData.colors.Length; ++i)
            {
                colorTable = race.colorTables[i];
                CacheUmaAvatar.SetColor(colorTable.name, colorTable.colors[avatarData.colors[i]]);
            }
            // Set character hair, beard, eyebrows (or other things up to your settings)
            Dictionary<string, List<UMATextRecipe>> recipes = CacheUmaAvatar.AvailableRecipes;
            string slotName;
            for (i = 0; i < avatarData.slots.Length; ++i)
            {
                slotName = gender.customizableSlots[i].name;
                CacheUmaAvatar.SetSlot(recipes[slotName][avatarData.slots[i]]);
            }
            // Set character dna
            List<string> dnaNames = new List<string>(CacheUmaAvatar.GetDNA().Keys);
            dnaNames.Sort();
            string dnaName;
            for (i = 0; i < avatarData.dnas.Length; ++i)
            {
                dnaName = dnaNames[i];
                CacheUmaAvatar.GetDNA()[dnaName].Set(avatarData.dnas[i] * 0.01f);
            }
            CacheUmaAvatar.BuildCharacter(true);
            CacheUmaAvatar.ForceUpdate(true, true, true);
        }
    }
}
