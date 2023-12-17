#if UNITY_STANDALONE && !CLIENT_BUILD
using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG.MMO
{
    public partial class SQLiteDatabase
    {
        [DevExtMethods("Init")]
        public void Init_UMA()
        {
            // Prepare uma data
            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS characterumasaves (
              id TEXT NOT NULL PRIMARY KEY,
              data TEXT NOT NULL
            )");
        }

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
            ExecuteNonQuery("INSERT INTO characterumasaves (id, data) VALUES (@id, @data)",
                new SqliteParameter("@id", characterData.Id),
                new SqliteParameter("@data", saveData));
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
            bool withQuests,
            bool withCurrencies,
            bool withServerCustomData,
            bool withPrivateCustomData,
            bool withPublicCustomData)
        {
            ExecuteReader((reader) =>
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
                new SqliteParameter("@id", characterData.Id));
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
            ExecuteNonQuery("UPDATE characterumasaves SET data=@data WHERE id=@id",
                new SqliteParameter("@id", characterData.Id),
                new SqliteParameter("@data", saveData));
        }

        [DevExtMethods("DeleteCharacter")]
        public void DeleteCharacter_UMA(string userId, string id)
        {
            // Delete uma data
            ExecuteNonQuery("DELETE FROM characterumasaves WHERE id=@id",
                new SqliteParameter("@id", id));
        }
    }
}
#endif