using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;
using UMA.CharacterSystem;

namespace MultiplayerARPG
{
    public class UICharacterCreateUMA : UICharacterCreate
    {
        public GameObject umaPanelRoot;
        public DropdownWrapper raceDropdown;
        public DropdownWrapper genderDropdown;
        public UIUmaCustomizeSlotDropdown prefabCustomizeSlotDropdown;
        public Transform customizeSlotContainer;
        public UIUmaDnaSlider prefabDnaSlider;
        public Transform dnaSliderContainer;
        public UIUmaColorDropdown prefabColorDropdown;
        public Transform colorOptionContainer;

        public CharacterModelUMA UmaModel { get; private set; }
        public byte SelectedRaceIndex { get; private set; }
        public byte SelectedGenderIndex { get; private set; }
        public byte[] SelectedSlots { get; private set; }
        public byte[] SelectedColors { get; private set; }
        public byte[] SelectedDnas { get; private set; }
        private bool dontApplyAvatar;
        
        protected override void OnSelectCharacter(IPlayerCharacterData playerCharacterData)
        {
            base.OnSelectCharacter(playerCharacterData);
            CharacterModelUMA characterModelUMA = SelectedModel as CharacterModelUMA;
            if (umaPanelRoot != null)
                umaPanelRoot.SetActive(characterModelUMA != null);
            UmaModel = characterModelUMA;
            ShowUmaCharacter();
        }

        private void ShowUmaCharacter()
        {
            if (UmaModel != null)
            {
                UmaModel.InitializeUMA();
                if (!UmaModel.IsUmaCharacterCreated)
                {
                    UmaModel.onUmaCharacterCreated -= OnUmaCharacterCreated;
                    UmaModel.onUmaCharacterCreated += OnUmaCharacterCreated;
                    return;
                }

                dontApplyAvatar = true;
                if (raceDropdown != null)
                {
                    raceDropdown.onValueChanged.RemoveListener(OnRaceDropdownValueChanged);
                    raceDropdown.options = new List<DropdownWrapper.OptionData>();
                    List<DropdownWrapper.OptionData> dropdownOptions = new List<DropdownWrapper.OptionData>();
                    UmaRace[] races = GameInstance.Singleton.UmaRaces;
                    foreach (UmaRace race in races)
                    {
                        dropdownOptions.Add(new DropdownWrapper.OptionData()
                        {
                            text = race.name,
                        });
                    }
                    raceDropdown.options = dropdownOptions;
                    OnRaceDropdownValueChanged(0);
                    raceDropdown.onValueChanged.AddListener(OnRaceDropdownValueChanged);
                }
                dontApplyAvatar = false;
                ApplyAvatar();
            }
        }

        private void OnUmaCharacterCreated()
        {
            StartCoroutine(OnCharacterCreatedRoutine());
        }

        private IEnumerator OnCharacterCreatedRoutine()
        {
            yield return null;
            ShowUmaCharacter();
        }

        private void OnRaceDropdownValueChanged(int selectedIndex)
        {
            SelectedRaceIndex = (byte)selectedIndex;
            if (genderDropdown != null)
            {
                genderDropdown.onValueChanged.RemoveListener(OnGenderDropdownValueChanged);
                genderDropdown.options = new List<DropdownWrapper.OptionData>();
                List<DropdownWrapper.OptionData> dropdownOptions = new List<DropdownWrapper.OptionData>();
                UmaRace race = GameInstance.Singleton.UmaRaces[selectedIndex];
                UmaRaceGender[] genders = race.genders;
                foreach (UmaRaceGender gender in genders)
                {
                    dropdownOptions.Add(new DropdownWrapper.OptionData()
                    {
                        text = gender.name,
                    });
                }
                // Setup color options
                GenericUtils.RemoveChildren(colorOptionContainer);
                SelectedColors = new byte[race.colorTables.Length];
                for (byte i = 0; i < race.colorTables.Length; ++i)
                {
                    UIUmaColorDropdown uiColor = Instantiate(prefabColorDropdown);
                    uiColor.Setup(this, i);
                    uiColor.transform.SetParent(colorOptionContainer);
                    uiColor.transform.localScale = Vector3.one;
                }
                // Switch dropdown
                genderDropdown.options = dropdownOptions;
                OnGenderDropdownValueChanged(0);
                genderDropdown.onValueChanged.AddListener(OnGenderDropdownValueChanged);
            }
        }

        private void OnGenderDropdownValueChanged(int selectedIndex)
        {
            SelectedGenderIndex = (byte)selectedIndex;
            UmaRace race = GameInstance.Singleton.UmaRaces[SelectedRaceIndex];
            UmaRaceGender gender = race.genders[SelectedGenderIndex];
            UmaModel.CacheUmaAvatar.ChangeRace(gender.raceData.raceName);
            UmaModel.CacheUmaAvatar.BuildCharacter(true);
            // Setup customizable slots
            GenericUtils.RemoveChildren(customizeSlotContainer);
            UmaCustomizableSlot[] slots = gender.customizableSlots;
            SelectedSlots = new byte[slots.Length];
            for (byte i = 0; i < slots.Length; ++i)
            {
                UIUmaCustomizeSlotDropdown uiSlot = Instantiate(prefabCustomizeSlotDropdown);
                uiSlot.Setup(this, i);
                uiSlot.transform.SetParent(customizeSlotContainer);
                uiSlot.transform.localScale = Vector3.one;
            }
            // Setup customizable dnas
            GenericUtils.RemoveChildren(dnaSliderContainer);
            Dictionary<string, DnaSetter> dnas = UmaModel.CacheUmaAvatar.GetDNA();
            List<string> dnaNames = new List<string>(dnas.Keys);
            dnaNames.Sort();
            SelectedDnas = new byte[dnaNames.Count];
            for (byte i = 0; i < dnaNames.Count; ++i)
            {
                UIUmaDnaSlider uiDnaSlider = Instantiate(prefabDnaSlider);
                uiDnaSlider.Setup(this, i, dnas[dnaNames[i]]);
                uiDnaSlider.transform.SetParent(dnaSliderContainer);
                uiDnaSlider.transform.localScale = Vector3.one;
            }
            ApplyAvatar();
        }

        public void SetSlot(byte index, byte value)
        {
            SelectedSlots[index] = value;
            ApplyAvatar();
        }

        public void SetColor(byte index, byte value)
        {
            SelectedColors[index] = value;
            ApplyAvatar();
        }

        public void SetDna(byte index, float value)
        {
            SelectedDnas[index] = (byte)(value * 100);
            ApplyAvatar();
        }

        public void ApplyAvatar()
        {
            if (dontApplyAvatar)
                return;
            UmaModel.ApplyUmaAvatar(GetAvatarData());
        }

        public UmaAvatarData GetAvatarData()
        {
            UmaAvatarData result = new UmaAvatarData();
            result.raceIndex = SelectedRaceIndex;
            result.genderIndex = SelectedGenderIndex;
            result.slots = SelectedSlots;
            result.colors = SelectedColors;
            result.dnas = SelectedDnas;
            return result;
        }

        protected override void SaveCreatingPlayerCharacter(string characterName)
        {
            PlayerCharacterData characterData = new PlayerCharacterData();
            characterData.Id = GenericUtils.GetUniqueId();
            characterData.SetNewPlayerCharacterData(characterName, SelectedDataId, SelectedEntityId);
            characterData.UmaAvatarData = GetAvatarData();
            characterData.SavePersistentCharacterData();
        }
    }
}
