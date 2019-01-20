using UnityEngine;
using UMA.CharacterSystem;

namespace MultiplayerARPG
{
    public abstract class BaseUmaAvatarApplier : ScriptableObject
    {
        public GameInstance gameInstance { get { return GameInstance.Singleton; } }
        public abstract void Apply(DynamicCharacterAvatar umaAvatar, UmaAvatarData avatarData);
    }
}
