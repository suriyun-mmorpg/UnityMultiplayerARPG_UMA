#if (UNITY_EDITOR || UNITY_SERVER) && UNITY_STANDALONE
using Cysharp.Threading.Tasks;
using Insthync.DevExtension;
using Mono.Data.Sqlite;
using System.Collections.Generic;

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

            onCreateCharacter += SQLiteDatabase_onCreateCharacter;
            onGetCharacter += SQLiteDatabase_onGetCharacter;
            onUpdateCharacter += SQLiteDatabase_onUpdateCharacter;
            onDeleteCharacter += SQLiteDatabase_onDeleteCharacter;
        }

        private UniTask SQLiteDatabase_onCreateCharacter(SqliteConnection connection, SqliteTransaction transaction, string userId, IPlayerCharacterData createData)
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
            ExecuteNonQuery(transaction, "INSERT INTO characterumasaves (id, data) VALUES (@id, @data)",
                new SqliteParameter("@id", createData.Id),
                new SqliteParameter("@data", saveData));
            return default;
        }

        private UniTask<PlayerCharacterData> SQLiteDatabase_onGetCharacter(PlayerCharacterData result, bool withEquipWeapons, bool withAttributes, bool withSkills, bool withSkillUsages, bool withBuffs, bool withEquipItems, bool withNonEquipItems, bool withSummons, bool withHotkeys, bool withQuests, bool withCurrencies, bool withServerCustomData, bool withPrivateCustomData, bool withPublicCustomData)
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
                    result.UmaAvatarData = umaAvatarData;
                }
            }, "SELECT data FROM characterumasaves WHERE id=@id",
                new SqliteParameter("@id", result.Id));
            return UniTask.FromResult(result);
        }

        private UniTask SQLiteDatabase_onUpdateCharacter(SqliteConnection connection, SqliteTransaction transaction, TransactionUpdateCharacterState state, IPlayerCharacterData updateData)
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
            ExecuteNonQuery(transaction, "UPDATE characterumasaves SET data=@data WHERE id=@id",
                new SqliteParameter("@id", updateData.Id),
                new SqliteParameter("@data", saveData));
            return default;
        }

        private UniTask SQLiteDatabase_onDeleteCharacter(SqliteConnection connection, SqliteTransaction transaction, string userId, string characterId)
        {
            // Delete uma data
            ExecuteNonQuery(transaction, "DELETE FROM characterumasaves WHERE id=@id",
                new SqliteParameter("@id", characterId));
            return default;
        }
    }
}
#endif