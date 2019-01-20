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
        private DnaSetter dnaSetter;
        public void Setup(UICharacterCreateUMA ui, DnaSetter dnaSetter)
        {
            this.ui = ui;
            this.dnaSetter = dnaSetter;

            if (textTitle != null)
            {
                string dnaName = dnaSetter.Name;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < dnaName.Length; i++)
                {
                    char c = dnaName[i];
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
            dnaSetter.Set(value);
            ui.UmaModel.CacheUmaAvatar.ForceUpdate(true, false, false);
        }
    }
}
