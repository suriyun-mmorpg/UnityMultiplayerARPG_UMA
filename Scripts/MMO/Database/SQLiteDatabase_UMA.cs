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
        }

        [DevExtMethods("CreateCharacter")]
        public void CreateCharacter_UMA(string userId, IPlayerCharacterData characterData)
        {
            // Save uma data
        }

        [DevExtMethods("ReadCharacter")]
        public void ReadCharacter_UMA(
            string userId,
            string id,
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
        }

        [DevExtMethods("UpdateCharacter")]
        public void UpdateCharacter_UMA(IPlayerCharacterData characterData)
        {
            // Save uma data
        }

        [DevExtMethods("DeleteCharacter")]
        public void DeleteCharacter_UMA(string userId, string id)
        {
            // Delete uma data
        }
    }
}
