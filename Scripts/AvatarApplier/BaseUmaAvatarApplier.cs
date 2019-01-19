using UnityEngine;
using UMA.CharacterSystem;

namespace MultiplayerARPG
{
    public abstract class BaseUmaAvatarApplier : ScriptableObject
    {
        public abstract void Apply(DynamicCharacterAvatar umaAvatar, UmaAvatarData avatarData);
        public abstract UmaAvatarData GetData(DynamicCharacterAvatar umaAvatar);
    }
}
