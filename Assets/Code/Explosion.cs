using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	public ParticleSystem par;
	public float size;

	// Use this for initialization
	void Start () {
		audio.volume = Platellery.soundLevel;
		transform.position += Vector3.back * 2;
		par.emissionRate = 300 * size;
		par.transform.localScale = Vector3.one * size / 1.25f;
		Destroy (gameObject, 2f);
	}
}