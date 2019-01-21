using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UMA;

namespace MultiplayerARPG
{
    public class UIUmaColorDropdown : MonoBehaviour
    {
        private ToggleGroup cacheToggleGroup;
        public ToggleGroup CacheToggleGroup
        {
            get
            {
                if (cacheToggleGroup == null)
                    cacheToggleGroup = gameObject.AddComponent<ToggleGroup>();
                return cacheToggleGroup;
            }
        }
        public TextWrapper textTitle;
        public Sprite dropdownSprite;
        public DropdownWithColor dropdown;
        private UICharacterCreateUMA ui;
        private byte slotIndex;
        public void Setup(UICharacterCreateUMA ui, byte slotIndex)
        {
            this.ui = ui;
            this.slotIndex = slotIndex;

            SharedColorTable colorTable = GameInstance.Singleton.umaRaces[ui.SelectedRaceIndex].colorTables[slotIndex];

            if (textTitle != null)
                textTitle.text = colorTable.sharedColorName;

            if (dropdown != null)
            {
                List<DropdownWithColor.OptionData> dropdownOptions = new List<DropdownWithColor.OptionData>();
                OverlayColorData[] colors = colorTable.colors;
                for (int i = 0; i < colors.Length; ++i)
                {
                    dropdownOptions.Add(new DropdownWithColor.OptionData()
                    {
                        image = dropdownSprite,
                        color = colors[i].color,
                    });
                }
                dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
                dropdown.options = dropdownOptions;
                OnDropdownValueChanged(0);
                dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
            }
        }

        private void OnDropdownValueChanged(int selectedIndex)
        {
            ui.SetColor(slotIndex, (byte)selectedIndex);
        }
    }
}
