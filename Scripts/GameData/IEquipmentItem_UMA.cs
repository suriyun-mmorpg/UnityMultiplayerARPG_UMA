using System.Collections.Generic;
using UMA;

namespace MultiplayerARPG
{
    public partial interface IEquipmentItem
    {
        Dictionary<string, UMATextRecipe[]> UmaRecipeSlot { get; }
    }
}
