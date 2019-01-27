using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public sealed class UICharacterListUMA : UICharacterList
    {
        public CharacterModelUMA UmaModel { get; private set; }
        
        protected override void OnSelectCharacter(IPlayerCharacterData playerCharacterData)
        {
            base.OnSelectCharacter(playerCharacterData);
            if (SelectedModel != null)
            {
                // Setup Uma model and customize options
                CharacterModelUMA characterModelUMA = SelectedModel as CharacterModelUMA;
                UmaModel = characterModelUMA;
                ShowUmaCharacter();
            }
        }

        private void ShowUmaCharacter()
        {
            if (UmaModel != null)
                UmaModel.ApplyUmaAvatar(SelectedPlayerCharacterData.UmaAvatarData);
        }
    }
}
