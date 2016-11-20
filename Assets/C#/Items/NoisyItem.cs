using UnityEngine;
using System.Collections;


public class NoisyItem : MonoBehaviour {
    public string soundTag;
    //public int itemType;


    void OnCollisionEnter2D(Collision2D col)
    {
        //SOUND: Ball collision, Arrow collisison, Stone collisison, Boulder rolling, Grenade collision
        //Use itemType and make a switch statement based off of that, or you can literally set the fmod component as each respective sound. Idk how that works.
        //This script is already added to the balls, arrows, stones, boulders, and grenades
        //I also added it to potatoes and some other physics objects if you want them to have it too

        switch (soundTag) {
            case "boulder":
                AkSoundEngine.PostEvent("BoulderRolling", gameObject);
                break;

            case "":
                AkSoundEngine.PostEvent("GeneralCollision", gameObject);
                break;

    }
    }
}
