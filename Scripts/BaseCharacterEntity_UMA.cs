using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;

namespace MultiplayerARPG
{
    public partial class BaseCharacterEntity
    {
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
            CharacterModelUMA characterModelUma = CharacterModel as CharacterModelUMA;
            if (characterModelUma == null)
                return;
            characterModelUma.ApplyUmaAvatar(avatarData);
        }
    }
}
