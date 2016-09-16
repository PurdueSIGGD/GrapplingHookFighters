using UnityEngine;
using System.Collections;

public class SpriteSet : MonoBehaviour {
    public Sprite wall_bottom, wall_mid, wall_top;
    public Sprite bridge_sprite;
    public Sprite unsafe_plat;
    public Sprite floor_left, floor_mid, floor_right;
    public Sprite shootable_platform;
    public Sprite nograpple_wall_bottom, nograpple_wall_mid, nograpple_wall_top;
    public Sprite lava_bottom_left, lava_bottom_right, lava_bottom_mid, lava_left, lava_mid_mid, lava_right, lava_top_left, lava_top_mid, lava_top_right;
    public Sprite item_box;
    public Sprite spike;
    public Sprite trampoline;

    public LayerMask physicalTileLayers;  //layers to check for physical platforms


    public static void ApplySprites(SpriteSet s)
    {
        foreach (CustomMapObject g in GameObject.FindObjectsOfType<CustomMapObject>())
        {
            switch (g.identifier)
            {
                case "item_box":
                    g.transform.FindChild("Sprite").GetComponent<SpriteRenderer>().sprite = s.item_box;
                    break;
                //TODO finish for all items
                case "lava":
                    //search for lava around current spot

                    //search above
                    Collider2D[] cols = Physics2D.OverlapPointAll(
                                g.transform.position + Vector3.up,
                                s.physicalTileLayers
                    );

                    if (cols != null && cols.Length > 0)
                    {
                        //we know there is something above, set some bool or something for the logic
                    }
                    //continue for each corner


                    g.transform.FindChild("Lava Sprite").GetComponent<SpriteRenderer>().sprite = s.lava_mid_mid;
                        break;
                default:
                    break;
            }
            //get sprite, change according to the spriteset passed
        }
    }
}
