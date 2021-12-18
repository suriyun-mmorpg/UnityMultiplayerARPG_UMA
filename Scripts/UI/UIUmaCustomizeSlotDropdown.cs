using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;

namespace MultiplayerARPG
{
    public class UIUmaCustomizeSlotDropdown : MonoBehaviour
    {
        public TextWrapper textTitle;
        public DropdownWrapper dropdown;
        private UICharacterCreateUMA ui;
        private byte slotIndex;
        public virtual void Setup(UICharacterCreateUMA ui, byte slotIndex)
        {
            this.ui = ui;
            this.slotIndex = slotIndex;

            UmaCustomizableSlot slot = GameInstance.Singleton.UmaRaces[ui.SelectedRaceIndex].genders[ui.SelectedGenderIndex].customizableSlots[slotIndex];

            // Set Title
            if (textTitle != null)
                textTitle.text = slot.title;

            // Set Dropdown options
            if (dropdown != null)
            {
                Dictionary<string, List<UMATextRecipe>> recipes = ui.UmaModel.CacheUmaAvatar.AvailableRecipes;
                List<UMATextRecipe> usingRecipes;
                List<DropdownWrapper.OptionData> dropdownOptions = new List<DropdownWrapper.OptionData>();
                if (recipes.TryGetValue(slot.name, out usingRecipes))
                {
                    foreach (UMATextRecipe usingRecipe in usingRecipes)
                    {
                        string name;
                        if (string.IsNullOrEmpty(usingRecipe.DisplayValue))
                            name = usingRecipe.name;
                        else
                            name = usingRecipe.DisplayValue;

                        dropdownOptions.Add(new DropdownWrapper.OptionData()
                        {
                            text = name,
                        });
                    }
                }
                dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
                dropdown.options = dropdownOptions;
                OnDropdownValueChanged(0);
                dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
            }
        }

        private void OnDropdownValueChanged(int selectedIndex)
        {
            ui.SetSlot(slotIndex, (byte)selectedIndex);
        }
    }
}
