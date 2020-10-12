#if UNITY_STANDALONE && !CLIENT_BUILD
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySqlConnector;
using Cysharp.Threading.Tasks;

namespace MultiplayerARPG.MMO
{
    public partial class MySQLDatabase
    {
        [DevExtMethods("CreateCharacter")]
        public void CreateCharacter_UMA(string userId, IPlayerCharacterData characterData)
        {
            // Save uma data
            IList<byte> bytes = characterData.UmaAvatarData.GetBytes();
            string saveData = string.Empty;
            for (int i = 0; i < bytes.Count; ++i)
            {
                if (i > 0)
                    saveData += ",";
                saveData += bytes[i];
            }
            ExecuteNonQuerySync("INSERT INTO characterumasaves (id, data) VALUES (@id, @data)",
                new MySqlParameter("@id", characterData.Id),
                new MySqlParameter("@data", saveData));
        }

        [DevExtMethods("ReadCharacter")]
        public void ReadCharacter_UMA(
            PlayerCharacterData characterData,
            bool withEquipWeapons,
            bool withAttributes,
            bool withSkills,
            bool withSkillUsages,
            bool withBuffs,
            bool withEquipItems,
            bool withNonEquipItems,
            bool withSummons,
            bool withHotkeys,
            bool withQuests)
        {
            // Read uma data
            ExecuteReaderSync((reader) =>
            {
                if (reader.Read())
                {
                    string data = reader.GetString(0);
                    string[] splitedData = data.Split(',');
                    List<byte> bytes = new List<byte>();
                    foreach (string entry in splitedData)
                    {
                        bytes.Add(byte.Parse(entry));
                    }
                    UmaAvatarData umaAvatarData = new UmaAvatarData();
                    umaAvatarData.SetBytes(bytes);
                    characterData.UmaAvatarData = umaAvatarData;
                }
            }, "SELECT data FROM characterumasaves WHERE id=@id",
                new MySqlParameter("@id", characterData.Id));
        }

        [DevExtMethods("UpdateCharacter")]
        public void UpdateCharacter_UMA(IPlayerCharacterData characterData)
        {
            // Save uma data
            IList<byte> bytes = characterData.UmaAvatarData.GetBytes();
            string saveData = string.Empty;
            for (int i = 0; i < bytes.Count; ++i)
            {
                if (i > 0)
                    saveData += ",";
                saveData += bytes[i];
            }
            ExecuteNonQuerySync("UPDATE characterumasaves SET data=@data WHERE id=@id",
                new MySqlParameter("@id", characterData.Id),
                new MySqlParameter("@data", saveData));
        }

        [DevExtMethods("DeleteCharacter")]
        public void DeleteCharacter_UMA(string userId, string id)
        {
            // Delete uma data
            ExecuteNonQuerySync("DELETE FROM characterumasaves WHERE id=@id",
                new MySqlParameter("@id", id));
        }
    }
}
#endif