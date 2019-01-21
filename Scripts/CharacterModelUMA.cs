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

        public bool IsUmaCharacterCreated { get; private set; }
        public bool IsInitializedUMA { get; private set; }
        private UmaAvatarData applyingAvatarData;

        private void Start()
        {
            InitializeUMA();
        }

        public void InitializeUMA()
        {
            if (IsInitializedUMA)
                return;
            IsInitializedUMA = true;
            CacheUmaAvatar.raceAnimationControllers.defaultAnimationController = CacheAnimatorController;
            CacheUmaAvatar.CharacterCreated.RemoveListener(OnUmaCharacterCreated);
            CacheUmaAvatar.CharacterCreated.AddListener(OnUmaCharacterCreated);
        }

        private void OnUmaCharacterCreated(UMAData data)
        {
            IsUmaCharacterCreated = true;
            if (applyingAvatarData != null)
            {
                ApplyUmaAvatar(applyingAvatarData);
                applyingAvatarData = null;
            }
        }
        
        public void ApplyUmaAvatar(UmaAvatarData avatarData)
        {
            if (CacheUmaAvatar == null)
            {
                Debug.LogWarning("[CharacterModelUMA] Uma avatar or applier is empty, cannot change avatar appearances");
                return;
            }
            InitializeUMA();
            // If not initialized, do it then apply avatar later
            if (!IsUmaCharacterCreated)
            {
                applyingAvatarData = avatarData;
                return;
            }
            UmaRace race = gameInstance.umaRaces[avatarData.raceIndex];
            UmaRaceGender gender = race.genders[avatarData.genderIndex];
            CacheUmaAvatar.ChangeRace(gender.raceData.raceName);
            int i;
            // Set character hair, beard, eyebrows (or other things up to your settings)
            Dictionary<string, List<UMATextRecipe>> recipes = CacheUmaAvatar.AvailableRecipes;
            string slotName;
            for (i = 0; i < avatarData.slots.Length; ++i)
            {
                slotName = gender.customizableSlots[i].name;
                CacheUmaAvatar.SetSlot(recipes[slotName][avatarData.slots[i]]);
            }
            // Set character dna
            Dictionary<string, DnaSetter> dnas = CacheUmaAvatar.GetDNA();
            List<string> dnaNames = new List<string>(dnas.Keys);
            dnaNames.Sort();
            string dnaName;
            for (i = 0; i < avatarData.dnas.Length; ++i)
            {
                dnaName = dnaNames[i];
                dnas[dnaName].Set(avatarData.dnas[i] * 0.01f);
            }
            // Set skin color, eyes color, hair color (or other things up to your settings)
            SharedColorTable colorTable;
            for (i = 0; i < avatarData.colors.Length; ++i)
            {
                colorTable = race.colorTables[i];
                CacheUmaAvatar.SetColor(colorTable.sharedColorName, colorTable.colors[avatarData.colors[i]]);
            }
            CacheUmaAvatar.BuildCharacter(true);
            CacheUmaAvatar.ForceUpdate(true, true, true);
        }
    }
}
