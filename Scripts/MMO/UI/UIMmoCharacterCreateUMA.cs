using Cysharp.Threading.Tasks;
using LiteNetLibManager;

namespace MultiplayerARPG.MMO
{
    public class UIMmoCharacterCreateUMA : UICharacterCreateUMA
    {
        protected override void OnClickCreate()
        {
            PlayerCharacterData characterData = new PlayerCharacterData();
            characterData.Id = GenericUtils.GetUniqueId();
            characterData.SetNewPlayerCharacterData(inputCharacterName.text.Trim(), SelectedDataId, SelectedEntityId);
            characterData.FactionId = SelectedFactionId;
            characterData.UmaAvatarData = GetAvatarData();
            MMOClientInstance.Singleton.RequestCreateCharacter(characterData, OnRequestedCreateCharacter);
        }

        private async UniTaskVoid OnRequestedCreateCharacter(ResponseHandlerData responseHandler, AckResponseCode responseCode, ResponseCreateCharacterMessage response)
        {
            await UniTask.Yield();
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            if (eventOnCreateCharacter != null)
                eventOnCreateCharacter.Invoke();
        }
    }
}
