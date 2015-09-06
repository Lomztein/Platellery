using UnityEngine;
using System.Collections;

public class Cannon : MonoBehaviour {

	public LayerMask targetLayer;
	public LayerMask solidLayer;

	public float maxAngle;
	public float range;

	public GameObject projectile;
	public float projectileSpeed;
	public float projectileInaccuracy;

	public float fireTime;
	public float reloadTime;
	public float turnSpeed;

	public Transform cannonTransform;
	public Transform muzzle;
	private Transform target;

	public Sprite[] sprites;
	public SpriteRenderer cannonSprite;

	private Transform startPos;
	public float recoil;
	public float recoilRecovery;

	private bool canFire = true;
	public int flipped = 1;
	private int flipDir = 1;

	void Start () {
		GameObject p = new GameObject ("StartPos");
		p.transform.parent = transform;
		p.transform.position = cannonTransform.position;
		startPos = p.transform;

		if (flipped == 0) {
			flipDir = -1;
		}
	}

	IEnumerator Fire () {
		canFire = false;
		for (int i = 1; i < sprites.Length; i++) {
			GameObject bullet = (GameObject)Instantiate (projectile, muzzle.position, muzzle.rotation);
			Projectile proj = bullet.GetComponent<Projectile>();
			proj.velocity = flipDir * muzzle.right * Random.Range (0.9f, 1.1f) * projectileSpeed + muzzle.up * Random.Range (-projectileInaccuracy, projectileInaccuracy);
			proj.hitLayer = targetLayer + solidLayer;
			Destroy (bullet, range / projectileSpeed);
			cannonSprite.sprite = sprites[i];
			cannonTransform.Translate (-recoil, 0, 0);
			yield return new WaitForSeconds (fireTime);
		}

		canFire = false;
		Invoke ("Reload", reloadTime);
	}

	void Reload () {
		canFire = true;
		cannonSprite.sprite = sprites [0];
	}

	// Update is called once per frame
	void FixedUpdate () {
		Rotate (Time.fixedDeltaTime);
		
		if (!target) {
			target = FindTarget (range);
		} else if (Vector3.Distance (transform.position, target.position) > range) {
			target = null;
		} else {
			/* && Vector3.Distance (cannonTransform.eulerAngles, startPos.eulerAngles*/
			if (canFire) {
				StartCoroutine (Fire ());
			}

			Ray ray = new Ray (muzzle.position, (target.position - muzzle.position).normalized);
			Debug.DrawLine (ray.origin, ray.origin + ray.direction * range);
			if (Physics.Raycast (ray, range, solidLayer)) {
				target = null;
			}
		}
		cannonTransform.position = Vector3.Lerp (cannonTransform.position, startPos.position, recoilRecovery * Time.fixedDeltaTime);
	}

	Transform FindTarget (float range) {
		Collider[] nearby = Physics.OverlapSphere (cannonTransform.position, range, targetLayer);

		float dist = float.MaxValue;
		Transform near = null;
		for (int i = 1; i < nearby.Length; i++) {

			Ray ray = new Ray (muzzle.position, (nearby[i].transform.position - muzzle.position).normalized);

			float locDist = Vector3.Distance (nearby [i].transform.position, cannonTransform.position);
			if (locDist < dist && !Physics.Raycast (ray, range, solidLayer)) {
				dist = locDist;
				near = nearby [i].transform;
			}
		}
		return near;
	}

	void Rotate (float deltaTime) {
		float angle = 0 + 180 * (1-flipped);
		if (target)
			angle = Angle.CalculateAngle (target, cannonTransform);
		cannonTransform.rotation = Quaternion.RotateTowards (cannonTransform.rotation, Quaternion.Euler (0, 0, flipped * 180 + angle), turnSpeed * deltaTime);
	}
}
