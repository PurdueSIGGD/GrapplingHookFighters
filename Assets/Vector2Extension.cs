using UnityEngine;

public static class Vector2Extension {

	public static Vector2 Deg2Vector(float degrees) {


		float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
		float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

		return new Vector2(cos,sin);
	}
	public static float Vector2Deg(Vector2 v) {
		return (float)(Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg);
	}
}