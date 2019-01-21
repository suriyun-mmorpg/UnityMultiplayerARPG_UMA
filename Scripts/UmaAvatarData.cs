using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib.Utils;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class UmaAvatarData : INetSerializable
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
            byte i;
            colors = new byte[reader.GetByte()];
            for (i = 0; i < colors.Length; ++i)
            {
                colors[i] = reader.GetByte();
            }
            slots = new byte[reader.GetByte()];
            for (i = 0; i < slots.Length; ++i)
            {
                slots[i] = reader.GetByte();
            }
            dnas = new byte[reader.GetByte()];
            for (i = 0; i < dnas.Length; ++i)
            {
                dnas[i] = reader.GetByte();
            }
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(raceIndex);
            writer.Put(genderIndex);
            byte i;
            writer.Put((byte)colors.Length);
            for (i = 0; i < colors.Length; ++i)
            {
                writer.Put(colors[i]);
            }
            writer.Put((byte)slots.Length);
            for (i = 0; i < slots.Length; ++i)
            {
                writer.Put(slots[i]);
            }
            writer.Put((byte)dnas.Length);
            for (i = 0; i < dnas.Length; ++i)
            {
                writer.Put(dnas[i]);
            }
        }

        public void SetBytes(IList<byte> bytes)
        {
            int index = 0;
            raceIndex = bytes[index++];
            genderIndex = bytes[index++];
            byte i;
            colors = new byte[bytes[index++]];
            for (i = 0; i < colors.Length; ++i)
            {
                colors[i] = bytes[index++];
            }
            slots = new byte[bytes[index++]];
            for (i = 0; i < slots.Length; ++i)
            {
                slots[i] = bytes[index++];
            }
            dnas = new byte[bytes[index++]];
            for (i = 0; i < dnas.Length; ++i)
            {
                dnas[i] = bytes[index++];
            }
        }

        public IList<byte> GetBytes()
        {
            List<byte> result = new List<byte>();
            result.Add(raceIndex);
            result.Add(genderIndex);
            byte i;
            result.Add((byte)colors.Length);
            for (i = 0; i < colors.Length; ++i)
            {
                result.Add(colors[i]);
            }
            result.Add((byte)slots.Length);
            for (i = 0; i < slots.Length; ++i)
            {
                result.Add(slots[i]);
            }
            result.Add((byte)dnas.Length);
            for (i = 0; i < dnas.Length; ++i)
            {
                result.Add(dnas[i]);
            }
            return result.ToArray();
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
