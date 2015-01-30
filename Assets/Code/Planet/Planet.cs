using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Planet : MonoBehaviour {

	public Tile[] tileTypes;
	public Dictionary<string, int> tile = new Dictionary<string, int>();

	public int radius;
	public int chunkResolution;
	public int[,] tiles;

	public Chunk[,] chunks;

	public GameObject chunkPrefab;

	public Vector3 center;
	public float gravity;
	public float temperature;
	public Atmosphere atmosphere;
	public Generator generator;
	public Transform atmos;

	private Texture2D tileAtlas;
	public static int textureSize = 16;

	public Vector2[] nearby;

	public static Planet current;

	public Transform moon;
	public float moonDistance;

	// Use this for initialization
	void Start () {
		current = this;
		Randomize ();
		InitializeTileDictionary ();
		InitializePlanet ();
	}

	void Randomize () {
		temperature = Random.Range (-5, 100);
		radius = Random.Range (25, 75);
	}

	void Update () {
		moon.position = center + new Vector3 (Mathf.Cos (Time.time / 4) * moonDistance, Mathf.Sin (Time.time / 4) * moonDistance);
	}

	void InitializeTileDictionary () {
		for (int i = 0 ; i < tileTypes.Length ; i++) {
			tile.Add (tileTypes[i].name, i);
		}
	}

	void GenerateAtmosphereTexture () {
		atmos.position = center + Vector3.forward;
		atmos.localScale = Vector3.one * (radius + atmosphere.altitude) * 2;

		int size = 1024;

		Texture2D tex = new Texture2D (size, size);

		for (int y = 0; y < tex.height; y++) {
			for (int x = 0; x < tex.width; x++) {
				tex.SetPixel (x,y,atmosphere.GetAtmosColor (atmosphere.PositionToAltitude01 (TexCoordsToWorld (x,y,size,size,center,atmos.localScale.x))));
			}
		}

		tex.Apply ();
		atmos.renderer.material.SetTexture (0, tex);
	}

	Vector2 TexCoordsToWorld (int x, int y, int width, int height, Vector2 pos, float scale) { // Assuming square quad
		return new Vector2 ((float)x/(float)width, (float)y/(float)height) * scale - new Vector2 (scale, scale)/2 + pos;
	}

	void GenerateTextureAtlas () {
		Texture2D tex = new Texture2D (textureSize * tileTypes.Length, textureSize);
		for (int i = 0; i < tileTypes.Length; i ++) {
			Texture2D loc = tileTypes[i].texture;

			for (int y = 0; y < textureSize; y++) {
				for (int x = 0; x < textureSize; x++) {

					tex.SetPixel (x + textureSize * i,y,loc.GetPixel (x,y));

				}
			}
		}

		tex.filterMode = FilterMode.Point;
		tex.Apply ();
		tileAtlas = tex;
	}

	public Vector3 GetPositionalGravity (Vector3 pos) {
		Quaternion rot = Quaternion.LookRotation (pos - center);
		return rot * Vector3.forward * gravity;
	}

	void InitializePlanet () {
		center = new Vector3 (radius, radius);
		InitializeTiles ();
		GenerateTextureAtlas ();
		InitializeChunks ();
		GenerateAtmosphereTexture ();

		Camera.main.GetComponent<CameraController>().MoveToPlanet (this);
	}

	void InitializeChunks () {
		Chunk.size = radius / chunkResolution;
		int chunkAmount = radius / Chunk.size * 2;
		chunks = new Chunk[chunkAmount,chunkAmount];

		for (int y = 0; y < chunkAmount; y++) {
			for (int x = 0; x < chunkAmount; x++) {

				GameObject newChunk = (GameObject)Instantiate (chunkPrefab, new Vector3 (x,y) * Chunk.size * Chunk.scale, Quaternion.identity);
				Chunk c = newChunk.GetComponent<Chunk>();
				chunks[x,y] = c;

				c.planet = this;
				c.startX = x * Chunk.size;
				c.startY = y * Chunk.size;

				newChunk.name = "Chunk (" + x.ToString() + "," + y.ToString() + ")";
				newChunk.transform.parent = transform;
				newChunk.renderer.sharedMaterial.SetTexture (0, tileAtlas);
			}
		}
	}

	void InitializeTiles () {
		tiles = new int[radius * 2,radius * 2];
		generator.Initialize (this, tile, radius, temperature);

		for (int y = 0; y < tiles.GetLength (1); y++) {
			for (int x = 0; x < tiles.GetLength (0); x++) {

				float distance = Vector3.Distance (new Vector3 (x,y), center) * Chunk.scale;
				float angle	   = Angle.CalculateAngle (center, new Vector3 (x,y));
				
				tiles[x,y] = tile[generator.GetTile (x, y, distance, angle)];

			}
		}
	}

	public Chunk GetChunk (int tileX, int tileY) {
		if (IsTileInsidePlanet (tileX,tileY))
			return chunks[Mathf.FloorToInt ((float)tileX / (float)Chunk.size), Mathf.FloorToInt ((float)tileY / (float)Chunk.size)];
		return null;
	}

	public Vector2 SceneToTilePosition (Vector3 pos) {
		return new Vector3 (Mathf.FloorToInt (pos.x),Mathf.FloorToInt (pos.y)) / Chunk.scale;
	}

	public bool IsTileInsidePlanet (int x, int y) {
		if (x < 0 || x > radius * 2 - 1) return false;
		if (y < 0 || y > radius * 2 - 1) return false;
		return true;
	}

	public int GetTile (int x, int y) {
		if (IsTileInsidePlanet (x,y)) {
			return tiles[x,y];
		}
		return 0;
	}

	public Tile GetTileType (int x, int y) {
		if (IsTileInsidePlanet (x,y)) {
			return tileTypes[tiles[x,y]];
		}
		return null;
	}

	void AddRandomizedProps () {

	}

	public void ChangeSingleTile (int x, int y, int newType) {
		tiles[x,y] = newType;
		GetChunk (x,y).GenerateMesh ();
	}

	Vector2 FloorCoordinates (float x, float y) {
		return new Vector2 (Mathf.Floor (x), Mathf.Floor (y));
	}

	public void CreateExplosion (float x, float y, float range, float strength) {
		List<Chunk> toUpdate = new List<Chunk>();

		int casts = Mathf.Min ((int)strength * 36, 360);

		for (int i = 0; i < casts; i++) {

			Ray ray = new Ray (new Vector3 (x,y), Quaternion.Euler (0,0,(float)i/(float)casts * 360f) * Vector3.right);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, range)) {

				Debug.DrawLine (ray.origin, hit.point, Color.white, 1f);

				Vector2 locPos = SceneToTilePosition (hit.transform.position);
				if (IsTileInsidePlanet ((int)locPos.x, (int)locPos.y)) {
					if (strength >= GetTileType ((int)locPos.x, (int)locPos.y).strength) {
						tiles[(int)locPos.x, (int)locPos.y] = 0;
						Chunk c = GetChunk ((int)locPos.x, (int)locPos.y);
						c.ForceCollider ((int)locPos.x, (int)locPos.y, false);
						if (!toUpdate.Contains (c)) toUpdate.Add (c);

						for (int j = 0; j < nearby.Length; j++) {
							Chunk cc = GetChunk ((int)locPos.x + (int)nearby[j].x,(int)locPos.y + (int)nearby[j].y);
							if (cc) cc.UpdateCollider ((int)locPos.x + (int)nearby[j].x,(int)locPos.y + (int)nearby[j].y);
							for (int a = 0; a < nearby.Length; a++) {
								int xx = (int)(locPos.x + nearby[a].x);
								int yy = (int)(locPos.y + nearby[a].y);
								Tile t = GetTileType (xx,yy);
								if (t != null) if (t.destroyedName != "") if (Random.Range (0,4) == 0) tiles[xx,yy] = tile[t.destroyedName];
							}
						}
					}else{
						if (GetTileType ((int)locPos.x, (int)locPos.y).destroyedName != "") tiles[(int)locPos.x, (int)locPos.y] = tile[GetTileType ((int)locPos.x, (int)locPos.y).destroyedName];
					}
				}
			}
		}

		for (int i = 0; i < toUpdate.Count ; i++) {
			toUpdate[i].GenerateMesh ();
		}
	}

	void OnDrawGizmos () {
		if (Application.isPlaying) {
			Vector2 pos = SceneToTilePosition (Camera.main.ScreenToWorldPoint (Input.mousePosition));
			if (IsTileInsidePlanet ((int)pos.x, (int)pos.y)) {
				Gizmos.DrawSphere (new Vector3 (pos.x, pos.y), 0.5f);
				Gizmos.DrawWireCube (GetChunk ((int)pos.x,(int)pos.y).center, Vector3.one * Chunk.size);
			}
		}
	}
}
