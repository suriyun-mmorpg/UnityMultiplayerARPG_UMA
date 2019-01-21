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
        private readonly List<UIUmaCustomizeSlotDropdown> uiSlots = new List<UIUmaCustomizeSlotDropdown>();
        private readonly List<UIUmaColorDropdown> uiColors = new List<UIUmaColorDropdown>();

        private void OnEnable()
        {
            StartCoroutine(ShowUmaCharacterRoutine());
        }

        protected override void ShowCharacter(int id)
        {
            BaseCharacterModel characterModel;
            if (!CharacterModels.TryGetValue(id, out characterModel))
                return;
            characterModel.gameObject.SetActive(true);
            // Setup Uma model and customize options
            CharacterModelUMA characterModelUMA = characterModel as CharacterModelUMA;
            if (umaPanelRoot != null)
                umaPanelRoot.SetActive(characterModelUMA != null);
            UmaModel = characterModelUMA;
            ShowUmaCharacter();
        }

        private IEnumerator ShowUmaCharacterRoutine()
        {
            // Setup and show uma character on next frame to prevent data load unfinished
            yield return null;
            ShowUmaCharacter();
        }

        private void ShowUmaCharacter()
        {
            if (UmaModel != null)
            {
                UmaModel.InitializeUMA();
                if (raceDropdown != null)
                {
                    List<DropdownWrapper.OptionData> dropdownOptions = new List<DropdownWrapper.OptionData>();
                    UmaRace[] races = GameInstance.Singleton.umaRaces;
                    foreach (UmaRace race in races)
                    {
                        dropdownOptions.Add(new DropdownWrapper.OptionData()
                        {
                            text = race.name,
                        });
                    }
                    raceDropdown.onValueChanged.RemoveListener(OnRaceDropdownValueChanged);
                    raceDropdown.options = dropdownOptions;
                    OnRaceDropdownValueChanged(0);
                    raceDropdown.onValueChanged.AddListener(OnRaceDropdownValueChanged);
                }
            }
        }

        private void OnRaceDropdownValueChanged(int selectedIndex)
        {
            SelectedRaceIndex = (byte)selectedIndex;
            if (genderDropdown != null)
            {
                List<DropdownWrapper.OptionData> dropdownOptions = new List<DropdownWrapper.OptionData>();
                UmaRace race = GameInstance.Singleton.umaRaces[selectedIndex];
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
                uiColors.Clear();
                SelectedColors = new byte[race.colorTables.Length];
                for (byte i = 0; i < race.colorTables.Length; ++i)
                {
                    UIUmaColorDropdown uiColor = Instantiate(prefabColorDropdown);
                    uiColor.Setup(this, i);
                    uiColor.transform.SetParent(colorOptionContainer);
                    uiColors.Add(uiColor);
                }
                // Switch dropdown
                genderDropdown.onValueChanged.RemoveListener(OnGenderDropdownValueChanged);
                genderDropdown.options = dropdownOptions;
                OnGenderDropdownValueChanged(0);
                genderDropdown.onValueChanged.AddListener(OnGenderDropdownValueChanged);
            }
        }

        private void OnGenderDropdownValueChanged(int selectedIndex)
        {
            SelectedGenderIndex = (byte)selectedIndex;
            UmaRace race = GameInstance.Singleton.umaRaces[SelectedRaceIndex];
            UmaRaceGender gender = race.genders[SelectedGenderIndex];
            // Change race
            UmaModel.CacheUmaAvatar.ChangeRace(gender.raceData);
            // Setup customizable slots
            GenericUtils.RemoveChildren(customizeSlotContainer);
            uiSlots.Clear();
            UmaCustomizableSlot[] slots = gender.customizableSlots;
            SelectedSlots = new byte[slots.Length];
            for (byte i = 0; i < slots.Length; ++i)
            {
                UIUmaCustomizeSlotDropdown uiSlot = Instantiate(prefabCustomizeSlotDropdown);
                uiSlot.Setup(this, i);
                uiSlot.transform.SetParent(customizeSlotContainer);
                uiSlots.Add(uiSlot);
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
            }
        }

        public void SetSlot(byte index, byte value)
        {
            SelectedSlots[index] = value;
            UmaRace race = GameInstance.Singleton.umaRaces[SelectedRaceIndex];
            UmaRaceGender gender = race.genders[SelectedGenderIndex];
            Dictionary<string, List<UMATextRecipe>> recipes = UmaModel.CacheUmaAvatar.AvailableRecipes;
            string slotName = gender.customizableSlots[index].name;
            if (recipes.ContainsKey(slotName))
            {
                UmaModel.CacheUmaAvatar.SetSlot(recipes[slotName][value]);
                UmaModel.CacheUmaAvatar.BuildCharacter(true);
                UmaModel.CacheUmaAvatar.ForceUpdate(false, true, true);
            }
        }

        public void SetColor(byte index, byte value)
        {
            SelectedColors[index] = value;
            UmaRace race = GameInstance.Singleton.umaRaces[SelectedRaceIndex];
            SharedColorTable colorTable = race.colorTables[index];
            UmaModel.CacheUmaAvatar.SetColor(colorTable.sharedColorName, colorTable.colors[value]);
            UmaModel.CacheUmaAvatar.ForceUpdate(false, true, false);
        }

        public void SetDna(byte index, float value)
        {
            SelectedDnas[index] = (byte)(value * 100);
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
    }
}
