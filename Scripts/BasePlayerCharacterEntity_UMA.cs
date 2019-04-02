using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;

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
            umaAvatarData.forOwnerOnly = false;
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
            CharacterModelUMA characterModelUma = CharacterModel as CharacterModelUMA;
            if (characterModelUma == null)
                return;
            characterModelUma.ApplyUmaAvatar(avatarData);
        }
    }
}
