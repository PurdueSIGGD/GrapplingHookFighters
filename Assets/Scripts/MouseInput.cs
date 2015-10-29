using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RawInputSharp;

public class MouseInput : MonoBehaviour {

    public float sensitivity;

    RawMouseDriver.RawMouseDriver mousedriver;
    private RawMouse[] mice;
    private Vector2[] mousePosition;
    private const int NUM_MICE = 4;

    // Use this for initialization
    void Start() {
        Cursor.visible = false;
        mousedriver = new RawMouseDriver.RawMouseDriver();
        mice = new RawMouse[NUM_MICE];
        mousePosition = new Vector2[NUM_MICE];
    }

    void Update() {
        // Loop through all the connected mice
        for (int i = 0; i < mice.Length; i++) {
            try {
                mousedriver.GetMouse(i, ref mice[i]);
                // Cumulative mousePositionment
                mousePosition[i] = new Vector2(mice[i].X, -mice[i].Y);
            } catch { }
        }
        for (int i = 0; i < mousePosition.Length; i++) {
            switch(i) {
                case 0:
                    Vector3 look = new Vector3(mousePosition[i].x, mousePosition[i].y, GameObject.Find("Player1").transform.position.z);
                    Debug.DrawLine(GameObject.Find("Player1").transform.position, look);
                    //GameObject.Find("Player1").transform.FindChild("Center").LookAt(look, Vector3.right);
                    break;
                case 1:
                    look = new Vector3(mousePosition[i].x, mousePosition[i].y, GameObject.Find("Player1").transform.position.z);
                    Debug.DrawLine(GameObject.Find("Player2").transform.position, look);
                    break;
            }
        }
    }

    void OnGUI() {
        GUILayout.Label("Connected:");
        for (int i = 0; i < mice.Length; i++) {
            if (mice[i] != null)
                GUILayout.Label("");
        }
    }

    void OnApplicationQuit() {
        // Clean up
        mousedriver.Dispose();
    }
}