using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UMA.CharacterSystem;

namespace MultiplayerARPG
{
    public class UIUmaDnaSlider : MonoBehaviour
    {
        public TextWrapper textTitle;
        public Slider slider;
        private UICharacterCreateUMA ui;
        private byte slotIndex;
        private DnaSetter dnaSetter;
        private string dnaName;
        public void Setup(UICharacterCreateUMA ui, byte slotIndex, DnaSetter dnaSetter)
        {
            this.ui = ui;
            this.slotIndex = slotIndex;
            this.dnaSetter = dnaSetter;
            dnaName = dnaSetter.Name;
            if (textTitle != null)
            {
                string displayDnaName = dnaName;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < displayDnaName.Length; i++)
                {
                    char c = displayDnaName[i];
                    if (i > 0 && char.IsUpper(c))
                    {
                        sb.Append(' ');
                    }
                    if (i == 0)
                        c = char.ToUpper(c);
                    sb.Append(c);
                }
                textTitle.text = sb.ToString();
            }

            if (slider != null)
            {
                slider.onValueChanged.RemoveListener(OnSliderValueChanged);
                slider.minValue = 0f;
                slider.maxValue = 1f;
                slider.value = 0.5f;
                OnSliderValueChanged(0.5f);
                slider.onValueChanged.AddListener(OnSliderValueChanged);
            }
        }

        private void OnSliderValueChanged(float value)
        {
            ui.SetDna(slotIndex, value);
            ui.UmaModel.CacheUmaAvatar.GetDNA()[dnaName].Set(value);
            ui.UmaModel.CacheUmaAvatar.ForceUpdate(true, false, false);
        }
    }
}
