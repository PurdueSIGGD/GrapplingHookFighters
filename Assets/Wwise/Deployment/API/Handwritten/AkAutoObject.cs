#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
using UnityEngine;
//This object helps for internal housekeeping in the C#/C++ bindings.
public class AkAutoObject
{
    public AkAutoObject(UnityEngine.GameObject GameObj)
	{
        m_gameObj = GameObj;
        AkSoundEngine.RegisterGameObj(GameObj, "AkAutoObject.cs", 0x01);
	}

    ~AkAutoObject()
    {
        AkSoundEngine.UnregisterGameObj(m_gameObj);
    }
    private GameObject m_gameObj;
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.