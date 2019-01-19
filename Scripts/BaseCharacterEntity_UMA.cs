using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using UMA.CharacterSystem;

namespace MultiplayerARPG
{
    public partial class BaseCharacterEntity
    {
        [Header("UMA Configs")]
        public DynamicCharacterAvatar umaAvatar;
        public BaseUmaAvatarApplier umaAvatarApplier;
        [SerializeField]
        protected SyncFieldUmaAvatarData umaAvatarData = new SyncFieldUmaAvatarData();

        [DevExtMethods("SetupNetElements")]
        public void SetupUmaNetElements()
        {
            umaAvatarData.sendOptions = SendOptions.ReliableUnordered;
            umaAvatarData.forOwnerOnly = false;
            umaAvatarData.onChange += OnUmaAvatarDataChange;
        }

        [DevExtMethods("OnDestroy")]
        public void OnUmaDestroy()
        {
            umaAvatarData.onChange -= OnUmaAvatarDataChange;
        }

        protected void OnUmaAvatarDataChange(UmaAvatarData avatarData)
        {
            if (umaAvatar == null || umaAvatarApplier == null)
            {
                Debug.LogWarning("[BaseCharacterEntity] Uma avatar or applier is empty, cannot change avatar appearances");
                return;
            }
            umaAvatarApplier.Apply(umaAvatar, avatarData);
        }

        public UmaAvatarData GetUmaAvatarData()
        {
            if (umaAvatar == null || umaAvatarApplier == null)
            {
                Debug.LogWarning("[BaseCharacterEntity] Uma avatar or applier is empty, cannot get avatar appearances data");
                return default(UmaAvatarData);
            }
            return umaAvatarApplier.GetData(umaAvatar);
        }
    }
}
