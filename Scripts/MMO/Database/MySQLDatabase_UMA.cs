#if (UNITY_EDITOR || UNITY_SERVER) && UNITY_STANDALONE
using System.Collections.Generic;
using MySqlConnector;

namespace MultiplayerARPG.MMO
{
    public partial class MySQLDatabase
    {
        [DevExtMethods("CreateCharacter")]
        public async void CreateCharacter_UMA(MySqlConnection connection, MySqlTransaction transaction, string userId, IPlayerCharacterData characterData)
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
            await ExecuteNonQuery(connection, transaction, "INSERT INTO characterumasaves (id, data) VALUES (@id, @data)",
                new MySqlParameter("@id", characterData.Id),
                new MySqlParameter("@data", saveData));
        }

        [DevExtMethods("ReadCharacter")]
        public async void ReadCharacter_UMA(
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
            // Read uma data
            await ExecuteReader((reader) =>
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
        public async void UpdateCharacter_UMA(IPlayerCharacterData characterData)
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
            await ExecuteNonQuery("UPDATE characterumasaves SET data=@data WHERE id=@id",
                new MySqlParameter("@id", characterData.Id),
                new MySqlParameter("@data", saveData));
        }

        [DevExtMethods("DeleteCharacter")]
        public async void DeleteCharacter_UMA(MySqlConnection connection, MySqlTransaction transaction, string userId, string id)
        {
            // Delete uma data
            await ExecuteNonQuery(connection, transaction, "DELETE FROM characterumasaves WHERE id=@id",
                new MySqlParameter("@id", id));
        }
    }
}
#endif