using UnityEngine;
using System;
using UMA.CharacterSystem;

namespace MultiplayerARPG
{
    public interface ICharacterModelUma
    {
        DynamicCharacterAvatar CacheUmaAvatar { get; }
        Action OnUmaCharacterCreated { get; set; }
        bool IsUmaCharacterCreated { get; }
        bool IsInitializedUMA { get; }
        void InitializeUMA();
        void ApplyUmaAvatar(UmaAvatarData avatarData);
    }
}
