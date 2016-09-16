using UnityEngine;
using System.Collections;
using UnityEditor;
[ExecuteInEditMode]
[InitializeOnLoad]
public class AutoSnap : MonoBehaviour
{
    
    void Update()
    {
        Debug.Log(Event.current.mousePosition);
        print(Time.deltaTime);
    }
}


