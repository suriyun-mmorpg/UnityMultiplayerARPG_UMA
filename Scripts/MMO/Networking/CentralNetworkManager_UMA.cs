using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG.MMO
{
    public partial class CentralNetworkManager
    {
        [DevExtMethods("ApplyCreateCharacterExtra")]
        public void ApplyCreateCharacterExtra_UMA(PlayerCharacterData characterData, byte[] extra)
        {
            UmaAvatarData umaAvatarData = new UmaAvatarData();
            umaAvatarData.SetBytes(extra);
            characterData.UmaAvatarData = umaAvatarData;
        }
    }
}
