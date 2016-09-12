using UnityEngine;
using System.Collections;

public class VisualTile : MonoBehaviour {

	public LayerMask physicalTileLayers;  //layers to check for physical platforms
	public string physicalTileTag;  //tags physical platforms must have


	// Use this for initialization
	void Start () {
		BindToPhysicalTile ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void BindToPhysicalTile() {
		float x = Mathf.Abs(transform.position.x % 1);
		float y = Mathf.Abs(transform.position.x % 1);
		if ((x != 0.5 && x != 0) || (y != 0.5 && y != 0) ) {
			Debug.LogWarning ("Visual Tile is not aligned to .5 grid: " + this.name);
		}



		Collider2D[] cols = Physics2D.OverlapPointAll (
			                    transform.position,
			                    physicalTileLayers
		);

		if (cols != null && cols.Length > 0) {

			Transform targetTile = null;
			foreach (Collider2D col in cols) {
				if (physicalTileTag.Contains(col.tag) || string.IsNullOrEmpty (physicalTileTag)) {
					targetTile = col.transform;
					break;
				}
			}
			if (targetTile == null) {
				return;
			}

			HidePhysicalTileSprites (targetTile);
			transform.SetParent (targetTile);
		}
	}

	void HidePhysicalTileSprites(Transform physTile) {
		//hide parent graphic
		foreach (SpriteRenderer renderer in physTile.GetComponentsInChildren<SpriteRenderer>()) {
			if (renderer != null) {
				renderer.enabled = false;
			}
		}
	}
}
