using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLibManager;

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
            umaAvatarData.deliveryMethod = DeliveryMethod.ReliableOrdered;
            umaAvatarData.syncMode = LiteNetLibSyncField.SyncMode.ServerToClients;
            umaAvatarData.onChange += OnUmaAvatarDataChange;
        }

        [DevExtMethods("OnDestroy")]
        public void OnUmaDestroy()
        {
            umaAvatarData.onChange -= OnUmaAvatarDataChange;
            onSetupNetElements -= OnSetupNetElements_UMA;
        }

        protected void OnUmaAvatarDataChange(bool isInit, UmaAvatarData avatarData)
        {
            if (CharacterModel is AnimatorCharacterModelUMA)
            {
                AnimatorCharacterModelUMA animatorCharacterModelUma = CharacterModel as AnimatorCharacterModelUMA;
                if (animatorCharacterModelUma == null)
                    return;
                animatorCharacterModelUma.ApplyUmaAvatar(avatarData);
            }
        }
    }
}
