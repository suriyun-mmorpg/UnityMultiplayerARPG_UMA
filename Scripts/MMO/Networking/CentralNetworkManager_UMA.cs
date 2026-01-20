using Insthync.DevExtension;
using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
    public partial class CentralNetworkManager
    {
        [DevExtMethods("SerializeCreateCharacterExtra")]
        public void SerializeCreateCharacterExtra_UMA(PlayerCharacterData characterData, NetDataWriter writer)
        {
            characterData.UmaAvatarData.Serialize(writer);
        }

        [DevExtMethods("DeserializeCreateCharacterExtra")]
        public void DeserializeCreateCharacterExtra_UMA(PlayerCharacterData characterData, NetDataReader reader)
        {
            UmaAvatarData umaAvatarData = new UmaAvatarData();
            umaAvatarData.Deserialize(reader);
            characterData.UmaAvatarData = umaAvatarData;
        }
    }
}
