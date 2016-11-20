#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

/// <summary>
///  @brief This class is used to perform DragAndDrop operations from the AkWwisePicker to any GameObject.
///  We found out that DragAndDrop operations in Unity do not transfer components, but only scripts. This
///  prevented us to set the name and ID of our components before performing the drag and drop. To fix this,
///  the DragAndDrop operation always transfers a AkDragDropHelper component that gets instantiated on the 
///  target GameObject. On its first Update() call, it will parse the DragAndDrop structure, which contains
///  all necessary information to instantiate the correct component, with the correct information
/// </summary>
[ExecuteInEditMode]
public class AkDragDropHelper : MonoBehaviour
{
    void Awake()
    {
        // Need a minimum of 4 members in DragAndDrop generic data:
		// GenericData[0] contains the component's name (string)
		// GenericData[1] contains the component's Guid (Guid)
		// GenericData[2] contains the component's AkGameObjID (int)
		// GenericData[3] contains the object's type (string)
        // We need two more fields for states and switches:
		// GenericData[4] contains the state or switch group Guid (Guid)
		// GenericData[5] contains the state or switch group AkGameObjID (int)
		object[] DDInfo = (object[])DragAndDrop.GetGenericData ("AKWwiseDDInfo");
		if( DDInfo != null && DDInfo.Length >= 4 )
        {
			Guid componentGuid = (Guid)DDInfo[1];
			int ID = (int)DDInfo[2];
			string type = (string)DDInfo[3];
            switch(type)
            {
                case "AuxBus":
                    CreateAuxBus(componentGuid, ID);
                    break;
                case "Event":
					CreateAmbient(componentGuid, ID);
                    break;
                case "Bank":
					CreateBank(componentGuid, (string)DDInfo[0]);
                    break;
                case "State":
					if (DDInfo.Length == 6)
                    {
						CreateState(componentGuid, ID, (Guid)DDInfo[4], (int)DDInfo[5]);
                    }
                    break;
                case "Switch":
					if (DDInfo.Length == 6)
                    {
						CreateSwitch(componentGuid, ID, (Guid)DDInfo[4], (int)DDInfo[5]);
                    }
                    break;
            }
				
			GUIUtility.hotControl = 0;
        }
    }


    
    void Start()
    {
		// Don't forget to destroy the AkDragDropHelper when we're done!
		Component.DestroyImmediate(this);
    }

    void CreateAuxBus(Guid componentGuid, int ID)
    {
        AkEnvironment[] akEnvironments = gameObject.GetComponents<AkEnvironment>();

        bool found = false;
        for (int i = 0; i < akEnvironments.Length; i++)
        {
			if (new Guid(akEnvironments[i].valueGuid) == componentGuid)
			{
				found = true;
                break;
            }
        }

        if (found == false)
        {
			AkEnvironment akEnvironment = Undo.AddComponent<AkEnvironment>(gameObject);
			if (akEnvironment != null)
            {
				akEnvironment.valueGuid = componentGuid.ToByteArray();
				akEnvironment.SetAuxBusID(ID);
			}
		}
	}

	void CreateAmbient(Guid componentGuid, int ID)
    {
		AkAmbient ambient = Undo.AddComponent<AkAmbient>(gameObject);

        if (ambient != null)
        {
			ambient.valueGuid = componentGuid.ToByteArray();
            ambient.eventID = ID;
        }
    }

	void CreateBank(Guid componentGuid, string name)
    {
		AkBank bank = Undo.AddComponent<AkBank>(gameObject);

		if (bank != null)
        {
			bank.valueGuid = componentGuid.ToByteArray();
			bank.bankName = name;
        }
    }

    void CreateState(Guid componentGuid, int ID, Guid groupGuid, int groupID)
    {
		AkState akState = Undo.AddComponent<AkState>(gameObject);
		
		if (akState != null)
        {
			akState.groupGuid = groupGuid.ToByteArray();
			akState.groupID = groupID;
            akState.valueGuid = componentGuid.ToByteArray();
            akState.valueID = ID;
        }
    }

    void CreateSwitch(Guid componentGuid, int ID, Guid groupGuid, int groupID)
    {
		AkSwitch akSwitch = Undo.AddComponent<AkSwitch>(gameObject);
		
		if (akSwitch != null)
        {
			akSwitch.groupGuid = groupGuid.ToByteArray();
			akSwitch.groupID = groupID;
			akSwitch.valueGuid = componentGuid.ToByteArray();
			akSwitch.valueID = ID;
        }
    }


}
#endif // UNITY_EDITOR
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.