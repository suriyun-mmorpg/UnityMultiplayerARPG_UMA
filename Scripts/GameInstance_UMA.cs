using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class GameInstance
    {
        [Header("UMA Setting")]
        public UmaAvatarDatabase umaAvatarDatabase;

        public UmaRace[] UmaRaces
        {
            get
            {
                if (umaAvatarDatabase != null)
                    return umaAvatarDatabase.umaRaces;
                return null;
            }
        }
    }

}
