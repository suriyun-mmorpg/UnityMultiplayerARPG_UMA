using Insthync.DevExtension;
using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public partial class PlayerCharacterDataExtensions
    {
        [DevExtMethods("CloneTo")]
        public static void CloneTo_UMA(IPlayerCharacterData from, IPlayerCharacterData to)
        {
            to.UmaAvatarData = from.UmaAvatarData;
        }

        [DevExtMethods("SerializeCharacterData")]
        public static void SerializeCharacterData_UMA(IPlayerCharacterData characterData, NetDataWriter writer)
        {
            characterData.UmaAvatarData.Serialize(writer);
        }

        [DevExtMethods("DeserializeCharacterData")]
        public static void DeserializeCharacterData_UMA(IPlayerCharacterData characterData, NetDataReader reader)
        {
            UmaAvatarData umaAvatarData = new UmaAvatarData();
            umaAvatarData.Deserialize(reader);
            characterData.UmaAvatarData = umaAvatarData;
        }
    }
}
