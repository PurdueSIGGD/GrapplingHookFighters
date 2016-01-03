using UnityEngine;
using System.Collections;

public class AutoParticleDestroy : MonoBehaviour 
{
	public bool useParticle;
	private ParticleSystem ps;
	public float time;

	public void Start() 
	{
		ps = GetComponent<ParticleSystem>();
	}

	public void Update() 
	{
		if (useParticle) {
			if(ps)
			{
				if(!ps.IsAlive())
				{
					Destroy(gameObject);
				}
			}
		} else {
			time-=Time.deltaTime;
			if (time < 0) {
				Destroy(gameObject);
			}
		}
	}
}