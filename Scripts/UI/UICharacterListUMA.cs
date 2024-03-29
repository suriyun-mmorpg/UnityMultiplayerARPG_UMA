﻿namespace MultiplayerARPG
{
    public class UICharacterListUMA : UICharacterList
    {
        public ICharacterModelUma UmaModel { get; protected set; }
        
        protected override void OnSelectCharacter(IPlayerCharacterData playerCharacterData)
        {
            if (buttonStart)
                buttonStart.gameObject.SetActive(true);
            if (buttonDelete)
                buttonDelete.gameObject.SetActive(true);
            characterModelContainer.SetChildrenActive(false);
            // Load selected character, set selected player character data and also validate its data
            _playerCharacterDataById.TryGetValue(playerCharacterData.Id, out _selectedPlayerCharacterData);
            // Validate map data
            if (!GameInstance.Singleton.GetGameMapIds().Contains(SelectedPlayerCharacterData.CurrentMapName))
            {
                PlayerCharacter database = SelectedPlayerCharacterData.GetDatabase() as PlayerCharacter;
                SelectedPlayerCharacterData.CurrentMapName = database.StartMap.Id;
                SelectedPlayerCharacterData.CurrentPosition = database.StartPosition;
            }
            // Set selected character to network manager
            (BaseGameNetworkManager.Singleton as LanRpgNetworkManager).selectedCharacter = SelectedPlayerCharacterData;
            // Show selected character model
            _characterModelById.TryGetValue(playerCharacterData.Id, out _selectedModel);
            if (SelectedModel != null && SelectedModel is ICharacterModelUma)
            {
                // Setup Uma model and applies options
                ICharacterModelUma characterModelUMA = SelectedModel as ICharacterModelUma;
                UmaModel = characterModelUMA;
                SelectedModel.gameObject.SetActive(true);
                UmaModel.ApplyUmaAvatar(SelectedPlayerCharacterData.UmaAvatarData);
            }
        }
    }
}
