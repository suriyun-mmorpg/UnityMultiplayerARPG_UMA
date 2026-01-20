#if (UNITY_EDITOR || UNITY_SERVER) && UNITY_STANDALONE
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Insthync.DevExtension;
using MySqlConnector;

namespace MultiplayerARPG.MMO
{
    public partial class MySQLDatabase
    {
        [DevExtMethods("Init")]
        public void Init_UMA()
        {
            onCreateCharacter += MySQLDatabase_onCreateCharacter;
            onGetCharacter += MySQLDatabase_onGetCharacter;
            onUpdateCharacter += MySQLDatabase_onUpdateCharacter;
            onDeleteCharacter += MySQLDatabase_onDeleteCharacter;
        }

        private async UniTask MySQLDatabase_onCreateCharacter(MySqlConnection connection, MySqlTransaction transaction, string userId, IPlayerCharacterData createData)
        {
            // Save uma data
            IList<byte> bytes = createData.UmaAvatarData.GetBytes();
            string saveData = string.Empty;
            for (int i = 0; i < bytes.Count; ++i)
            {
                if (i > 0)
                    saveData += ",";
                saveData += bytes[i];
            }
            await ExecuteNonQuery(connection, transaction, "INSERT INTO characterumasaves (id, data) VALUES (@id, @data)",
                new MySqlParameter("@id", createData.Id),
                new MySqlParameter("@data", saveData));
        }

        private async UniTask<PlayerCharacterData> MySQLDatabase_onGetCharacter(PlayerCharacterData result, bool withEquipWeapons, bool withAttributes, bool withSkills, bool withSkillUsages, bool withBuffs, bool withEquipItems, bool withNonEquipItems, bool withSummons, bool withHotkeys, bool withQuests, bool withCurrencies, bool withServerCustomData, bool withPrivateCustomData, bool withPublicCustomData)
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
                    result.UmaAvatarData = umaAvatarData;
                }
            }, "SELECT data FROM characterumasaves WHERE id=@id",
                new MySqlParameter("@id", result.Id));
            return result;
        }


        private async UniTask MySQLDatabase_onUpdateCharacter(MySqlConnection connection, MySqlTransaction transaction, TransactionUpdateCharacterState state, IPlayerCharacterData updateData)
        {
            // Save uma data
            IList<byte> bytes = updateData.UmaAvatarData.GetBytes();
            string saveData = string.Empty;
            for (int i = 0; i < bytes.Count; ++i)
            {
                if (i > 0)
                    saveData += ",";
                saveData += bytes[i];
            }
            await ExecuteNonQuery(connection, transaction, "UPDATE characterumasaves SET data=@data WHERE id=@id",
                new MySqlParameter("@id", updateData.Id),
                new MySqlParameter("@data", saveData));
        }


        private async UniTask MySQLDatabase_onDeleteCharacter(MySqlConnection connection, MySqlTransaction transaction, string userId, string characterId)
        {
            // Delete uma data
            await ExecuteNonQuery(connection, transaction, "DELETE FROM characterumasaves WHERE id=@id",
                    new MySqlParameter("@id", characterId));
        }
    }
}
#endif