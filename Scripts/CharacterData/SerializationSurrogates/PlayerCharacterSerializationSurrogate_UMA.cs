using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

public partial class PlayerCharacterSerializationSurrogate
{
    [DevExtMethods("GetObjectData")]
    public void GetObjectData_UMA(System.Object obj,
        SerializationInfo info,
        StreamingContext context)
    {
        PlayerCharacterData data = (PlayerCharacterData)obj;
        info.AddListValue("UmaAvatarData", data.UmaAvatarData.GetBytes());
    }

    [DevExtMethods("SetObjectData")]
    public void SetObjectData_UMA(System.Object obj,
        SerializationInfo info,
        StreamingContext context,
        ISurrogateSelector selector)
    {
        PlayerCharacterData data = (PlayerCharacterData)obj;
        data.UmaAvatarData.SetBytes(info.GetListValue<byte>("UmaAvatarData"));
    }
}
