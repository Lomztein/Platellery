using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public Vector3 velocity;
	public LayerMask hitLayer;

	// Update is called once per frame
	void FixedUpdate () {

		transform.position += velocity * Time.fixedDeltaTime;
		Ray ray = new Ray (transform.position, velocity.normalized);
		RaycastHit hit;

		Debug.DrawLine (ray.origin, ray.origin + ray.direction * velocity.magnitude * Time.fixedDeltaTime);

		if (Physics.Raycast (ray, out hit, velocity.magnitude * Time.fixedDeltaTime * 1.1f, hitLayer)) {
			hit.collider.SendMessage ("OnModuleHit", SendMessageOptions.DontRequireReceiver);
			Module m = hit.collider.GetComponent<Module>();
			if (m) {
				m.missle.rigidbody.AddForceAtPosition (velocity ,hit.point);
			}
			Planet.CreateExplosionEffect (hit.point.x, hit.point.y, 0.1f);
			Destroy (gameObject);
		}
	}
}
