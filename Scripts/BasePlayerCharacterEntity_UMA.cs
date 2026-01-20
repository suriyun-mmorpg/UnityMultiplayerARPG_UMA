using Insthync.DevExtension;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class BasePlayerCharacterEntity
    {
        public UmaAvatarData UmaAvatarData
        {
            get { return umaAvatarData.Value; }
            set { umaAvatarData.Value = value; }
        }

        [SerializeField]
        protected SyncFieldUmaAvatarData umaAvatarData = new SyncFieldUmaAvatarData();

        [DevExtMethods("Awake")]
        public void Awake_UMA()
        {
            onSetupNetElements += OnSetupNetElements_UMA;
        }

        public void OnSetupNetElements_UMA()
        {
            umaAvatarData.onChange += OnUmaAvatarDataChange;
        }

        [DevExtMethods("OnDestroy")]
        public void OnDestroy_UMA()
        {
            umaAvatarData.onChange -= OnUmaAvatarDataChange;
            onSetupNetElements -= OnSetupNetElements_UMA;
        }

        protected void OnUmaAvatarDataChange(bool isInit, UmaAvatarData oldAvatarData, UmaAvatarData avatarData)
        {
            if (CharacterModel is ICharacterModelUma characterModelUma)
                characterModelUma.ApplyUmaAvatar(avatarData);
        }
    }
}
