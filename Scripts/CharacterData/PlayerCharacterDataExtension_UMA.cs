using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerCharacterDataExtension
{
    [DevExtMethods("CloneTo")]
    public static void CloneTo_UMA(IPlayerCharacterData from, IPlayerCharacterData to)
    {
        to.UmaAvatarData = from.UmaAvatarData;
    }
}
