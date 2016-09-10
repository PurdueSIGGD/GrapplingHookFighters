using UnityEngine;
using System.Collections;

public class GrappleDetacher : MonoBehaviour {

	void OnDestroy()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            //evacuate the children.
            Transform t = transform.GetChild(i);
            print(t.name);
            GrappleScript g;
            if (g = t.GetComponent<GrappleScript>())
            {
                if (g.center != null) g.center.GetComponentInParent<GrappleLauncher>().Disconnect();
            }
            if (t.CompareTag("Effect") && t.GetComponent<Sticky>())
            {
                t.SendMessage("Unstuck");
            }

        }

    }
}
