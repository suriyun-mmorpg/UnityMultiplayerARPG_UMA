using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public sealed class UICharacterListUMA : UICharacterList
    {
        public CharacterModelUMA UmaModel { get; private set; }
        public PlayerCharacterData CurrentPlayerCharacterData { get; private set; }
        private readonly Dictionary<string, PlayerCharacterData> SavedPlayerCharacters = new Dictionary<string, PlayerCharacterData>();
        
        protected override void LoadCharacters()
        {
            SelectionManager.Clear();
            // Unenabled buttons
            buttonStart.gameObject.SetActive(false);
            buttonDelete.gameObject.SetActive(false);
            // Remove all models
            characterModelContainer.RemoveChildren();
            CharacterModels.Clear();
            // Remove all saved data
            SavedPlayerCharacters.Clear();
            // Show list of created characters
            List<PlayerCharacterData> selectableCharacters = PlayerCharacterDataExtension.LoadAllPersistentCharacterData();
            for (int i = selectableCharacters.Count - 1; i >= 0; --i)
            {
                PlayerCharacterData selectableCharacter = selectableCharacters[i];
                if (selectableCharacter == null || !GameInstance.PlayerCharacters.ContainsKey(selectableCharacter.DataId))
                    selectableCharacters.RemoveAt(i);
            }
            selectableCharacters.Sort(new PlayerCharacterDataLastUpdateComparer().Desc());
            CacheList.Generate(selectableCharacters, (index, character, ui) =>
            {
                // Add player character saved data to dictionary, we will use it later
                SavedPlayerCharacters[character.Id] = character;
                // Setup UIs
                UICharacter uiCharacter = ui.GetComponent<UICharacter>();
                uiCharacter.Data = character;
                // Select trigger when add first entry so deactivate all models is okay beacause first model will active
                BaseCharacterModel characterModel = character.InstantiateModel(characterModelContainer);
                if (characterModel != null)
                {
                    CharacterModels[character.Id] = characterModel;
                    characterModel.gameObject.SetActive(false);
                    characterModel.SetEquipWeapons(character.EquipWeapons);
                    characterModel.SetEquipItems(character.EquipItems);
                    SelectionManager.Add(uiCharacter);
                }
            });
        }

        protected override void ShowCharacter(string id)
        {
            BaseCharacterModel characterModel;
            if (!CharacterModels.TryGetValue(id, out characterModel))
                return;
            characterModel.gameObject.SetActive(true);
            // Setup Uma model and customize options
            CharacterModelUMA characterModelUMA = characterModel as CharacterModelUMA;
            UmaModel = characterModelUMA;
            CurrentPlayerCharacterData = SavedPlayerCharacters[id];
            ShowUmaCharacter();
        }

        private void ShowUmaCharacter()
        {
            if (UmaModel != null)
                UmaModel.ApplyUmaAvatar(CurrentPlayerCharacterData.UmaAvatarData);
        }
    }
}
