using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class SpriteSet {

    public Sprite bridge_sprite;
    public Sprite unsafe_plat;
    public Sprite shootable_platform;
    public Sprite nograpple_wall_bottom, nograpple_wall_mid, nograpple_wall_top;
    public Sprite wall_bottom, wall_mid, wall_top;
    public Sprite floor_thin_left, floor_thin_middle, floor_thin_right;
    public Sprite floor_bottom_left, floor_bottom_right, floor_bottom_mid, floor_left, floor_mid_mid, floor_right, floor_top_left, floor_top_mid, floor_top_right;
    public Sprite lava_bottom_left, lava_bottom_right, lava_bottom_mid, lava_left, lava_mid_mid, lava_right, lava_top_left, lava_top_mid, lava_top_right;
	public GameObject lavaTop, lavaLeft, lavaRight, lavaBottom, lava; //the actual gameobjects that we want different colliders and what not forx
    public Sprite item_box;
    public Sprite spike;
    public Sprite trampoline;
    public Sprite errorSprite;

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
                    bool[] hits = findHits("lava", g.transform.position, s.physicalTileLayers);
                    // 0 1 2
                    // 3   4
                    // 5 6 7 
					GameObject finalGameObject = s.lava;
					if (hits[1] && hits[2] && hits[3] && hits[4] && hits[5] && hits[6] && hits[7]) finalGameObject = s.lava;
					else if (hits[1] && hits[2] && hits[4] && hits[6] && hits[7]) finalGameObject = s.lavaLeft;
					else if (hits[1] && hits[0] && hits[3] && hits[5] && hits[6]) finalGameObject = s.lavaRight;
					else if (hits[0] && hits[2] && !hits[6]) finalGameObject = s.lavaBottom;
					else if (hits[5] && hits[7] && !hits[1]) finalGameObject = s.lavaTop;
					else if (hits[6] && hits[4]) finalGameObject = s.lavaLeft;
					else if (hits[3] && hits[6]) finalGameObject = s.lavaRight;
					else if (hits[1] && hits[4]) finalGameObject = s.lavaLeft;
					else if (hits[1] && hits[3]) finalGameObject = s.lavaRight;

					Vector3 tmpPos = g.transform.position;
					Quaternion tmpRot = g.transform.rotation;
					GameObject.Destroy(g.gameObject);
					GameObject newG = (GameObject)GameObject.Instantiate(finalGameObject, tmpPos, tmpRot);

					//Debug.Log(g.name + " " + g.transform.position);
                    //for (int i = 0; i < hits.Length; i++) Debug.Log(hits[i]);
					Sprite finalSprite = s.errorSprite;


                    if (hits[1] && hits[2] && hits[3] && hits[4] && hits[5] && hits[6] && hits[7]) finalSprite = s.lava_mid_mid;
                    else if (hits[1] && hits[2] && hits[4] && hits[6] && hits[7]) finalSprite = s.lava_left;
                    else if (hits[1] && hits[0] && hits[3] && hits[5] && hits[6]) finalSprite = s.lava_right;
                    else if (hits[0] && hits[2] && !hits[6]) finalSprite = s.lava_bottom_mid;
                    else if (hits[5] && hits[7] && !hits[1]) finalSprite = s.lava_top_mid;
                    else if (hits[6] && hits[4]) finalSprite = s.lava_top_left;
                    else if (hits[3] && hits[6]) finalSprite = s.lava_top_right;
                    else if (hits[1] && hits[4]) finalSprite = s.lava_bottom_left;
                    else if (hits[1] && hits[3]) finalSprite = s.lava_bottom_right;
                    newG.transform.FindChild("Lava Sprite").GetComponent<SpriteRenderer>().sprite = finalSprite;
                    break;
                case "floor":

                    //TODO include thin plates(1 thin, horiz = thin, vert = walls)
                    bool[] hits1 = findHits("floor", g.transform.position, s.physicalTileLayers);
                    // 0 1 2
                    // 3   4
                    // 5 6 7 
                    Sprite finalSprite1 = s.errorSprite;
                    //Debug.Log(g.name + " " + g.transform.position);
                    //for (int i = 0; i < hits1.Length; i++) Debug.Log(hits1[i]);

                    if (hits1[1] && hits1[2] && hits1[3] && hits1[4] && hits1[5] && hits1[6] && hits1[7]) finalSprite1 = s.floor_mid_mid;
                    else if (hits1[1] && hits1[2] && hits1[4] && hits1[6] && hits1[7]) finalSprite1 = s.floor_left;
                    else if (hits1[1] && hits1[0] && hits1[3] && hits1[5] && hits1[6]) finalSprite1 = s.floor_right;
                    else if (hits1[0] && hits1[2] && !hits1[6]) finalSprite1 = s.floor_bottom_mid;
                    else if (hits1[5] && hits1[7] && !hits1[1]) finalSprite1 = s.floor_top_mid;
                    else if (hits1[6] && hits1[4]) finalSprite1 = s.floor_top_left;
                    else if (hits1[3] && hits1[6]) finalSprite1 = s.floor_top_right;
                    else if (hits1[1] && hits1[4]) finalSprite1 = s.floor_bottom_left;
                    else if (hits1[1] && hits1[3]) finalSprite1 = s.floor_bottom_right;
                    else if (hits1[4] && hits1[3]) finalSprite1 = s.floor_thin_middle;
                    else if (hits1[3]) finalSprite1 = s.floor_thin_right;
                    else if (hits1[4]) finalSprite1 = s.floor_thin_left;
                    else if (hits1[1] && hits1[6]) finalSprite1 = s.wall_mid;
                    else if (hits1[1]) finalSprite1 = s.wall_bottom;
                    else if (hits1[6]) finalSprite1 = s.wall_top;
                    if (!(hits1[1]  || hits1[3] || hits1[4] || hits1[6])) finalSprite1 = s.floor_thin_middle;


                    g.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = finalSprite1;
                    break;
                case "unsafe_plat":
                    g.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = s.unsafe_plat;
                    break;
                case "shootable_plat":
                    g.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = s.shootable_platform;
                    break;
                case "bridge_sprite":
                    for (int i = 0; i < g.transform.childCount; i++)
                    {
                        g.transform.GetChild(i).GetChild(0).GetComponent<SpriteRenderer>().sprite = s.bridge_sprite;
                    }
                    break;
                case "spike":
                    for (int i = 0; i < g.transform.childCount; i++)
                    {
                        g.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = s.spike;
                    }
                    break;
                case "trampoline":
                    g.transform.FindChild("GameObject").GetComponent<SpriteRenderer>().sprite = s.trampoline;
                    break;
                default:
                    break;
            }
            //get sprite, change according to the spriteset passed
        }
    }

    private static bool[] findHits(string identifier, Vector3 position, LayerMask physicalTileLayers)
    {
        //search for tiles around current spot
        bool[] hits = new bool[8];
        // 0 1 2
        // 3   4
        // 5 6 7 
        hits[0] = SpriteSet.doesItHit(identifier, position + Vector3.left + Vector3.up, physicalTileLayers);
        hits[1] = SpriteSet.doesItHit(identifier, position + Vector3.up, physicalTileLayers);
        hits[2] = SpriteSet.doesItHit(identifier, position + Vector3.right + Vector3.up, physicalTileLayers);
        hits[3] = SpriteSet.doesItHit(identifier, position + Vector3.left, physicalTileLayers);
        hits[4] = SpriteSet.doesItHit(identifier, position + Vector3.right, physicalTileLayers);
        hits[5] = SpriteSet.doesItHit(identifier, position + Vector3.left + Vector3.down, physicalTileLayers);
        hits[6] = SpriteSet.doesItHit(identifier, position + Vector3.down, physicalTileLayers);
        hits[7] = SpriteSet.doesItHit(identifier, position + Vector3.right + Vector3.down, physicalTileLayers);

        //continue for each corner

        return hits;
    }
    public static bool doesItHit(string identifier, Vector3 position, LayerMask physicalTileLayers)
    {
        Collider2D[] cols = Physics2D.OverlapPointAll(
                   position, physicalTileLayers
       );

        if (cols != null && cols.Length > 0)
        {
            //we know there is something above, set some bool or something for the logic
            foreach(Collider2D col in cols)
            {
                CustomMapObject c;
                //is a custom map object
                if (c = col.GetComponent<CustomMapObject>())
                {
                    //has the same identifier
                    if (c.identifier == identifier) return true;
                }
            }
        }
        return false;
    }
}
