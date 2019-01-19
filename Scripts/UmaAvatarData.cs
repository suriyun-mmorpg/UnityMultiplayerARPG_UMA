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
        public byte[] slots;
        public byte[] dnas;

        public void Deserialize(NetDataReader reader)
        {
            raceIndex = reader.GetByte();
            genderIndex = reader.GetByte();
            colors = reader.GetBytesWithLength();
            slots = reader.GetBytesWithLength();
            dnas = reader.GetBytesWithLength();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(raceIndex);
            writer.Put(genderIndex);
            writer.PutBytesWithLength(colors);
            writer.PutBytesWithLength(slots);
            writer.PutBytesWithLength(dnas);
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
