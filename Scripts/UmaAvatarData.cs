using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib.Utils;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct UmaAvatarData : INetSerializable
    {
        public byte raceIndex;
        public byte genderIndex;
        public byte[] colors;


        public void Deserialize(NetDataReader reader)
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(NetDataWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }

    [System.Serializable]
    public class SyncFieldUmaAvatarData : LiteNetLibSyncField<UmaAvatarData>
    {
        protected override bool IsValueChanged(UmaAvatarData newValue)
        {
            return true;
        }
    }
}
