using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UICharacterListUMA : UICharacterList
    {
        public ICharacterModelUma UmaModel { get; private set; }
        
        protected override void OnSelectCharacter(IPlayerCharacterData playerCharacterData)
        {
            base.OnSelectCharacter(playerCharacterData);
            if (SelectedModel != null)
            {
                // Setup Uma model and customize options
                ICharacterModelUma characterModelUMA = SelectedModel as ICharacterModelUma;
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
