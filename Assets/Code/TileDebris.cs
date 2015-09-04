using UnityEngine;
using System.Collections;

public class TileDebris : MonoBehaviour {

	public Vector3 velocity;
	public Planet planet;
	public int id;

	public MeshFilter meshFilter;
	private float rotateSpeed;

	public void ChangeUVs () {

		Mesh mesh = meshFilter.mesh;

		float v = 1f / (float)planet.tileTypes.Length;
		float h = 1f / (planet.bitmaskAtlasMask.width / Planet.textureSize);
		float mask = 0f;
		
		mesh.uv[0] = new Vector2 (mask * h, (float)id * v);
		mesh.uv[1] = new Vector2 (mask * h, (float)id * v + v);
		mesh.uv[2] = new Vector2 (mask * h + h, (float)id * v + v);
		mesh.uv[3] = new Vector2 (mask * h + h, (float)id * v);

		rotateSpeed = velocity.magnitude * 4f;
		if (Random.Range (0, 2) == 1)
			rotateSpeed = -rotateSpeed;

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.Rotate (0, 0, rotateSpeed * Time.fixedDeltaTime);
		transform.position += velocity * Time.fixedDeltaTime;
		velocity += planet.GetPositionalGravity (transform.position) * Time.fixedDeltaTime;
		Ray ray = new Ray (transform.position, velocity.normalized);

		if (Physics.Raycast (ray, Mathf.Max (velocity.magnitude * Time.fixedDeltaTime, 1f))) {
			Planet.CreateExplosionEffect (transform.position.x, transform.position.y, 0.8f);
			Destroy (gameObject);
		}
	}
}
