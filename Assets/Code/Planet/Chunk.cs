using UnityEngine;
using System.Collections;

public class Chunk : MonoBehaviour {

	public static int size = 10;
	public static float scale = 1f;

	public int startX;
	public int startY;

	public Vector3 center;

	public Planet planet;

	public GameObject colPrefab;
	public GameObject[,] cols;

	private Vector3[] verts;
	private int[] tris;
	private Vector2[] uvs;
	private Vector3[] norms;

	public void GenerateMesh () {

		center = transform.position + Vector3.one * Chunk.size / 2;

		int tileAmount = Chunk.size * Chunk.size;
		verts = new Vector3[tileAmount * 4];
		tris = new int[tileAmount * 6];
		uvs = new Vector2[verts.Length];
		norms = new Vector3[verts.Length];

		for (int y = 0; y < Chunk.size; y++) {
			for (int x = 0; x < Chunk.size; x++) {
				AddFace (x,y, x + y * Chunk.size, planet.tiles[x + startX,y + startY]);
				UpdateCollider (x + startX,y + startY);

			}
		}

		Mesh mesh = new Mesh ();
		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.uv = uvs;
		mesh.normals = norms;

		GetComponent<MeshFilter>().mesh = mesh;
		planet.queue.Remove (this);
	}

	Vector2 GlobalToLocalCoords (int x, int y) {
		return new Vector2 (x - startX,y - startY);
	}

	bool IsInsideChunk (int x, int y) {
		if (x < 0 || x > Chunk.size - 1) return false;
		if (y < 0 || y > Chunk.size - 1) return false;
		return true;
	}

	public void ForceCollider (int x, int y, bool status) {
		int lx = (int)GlobalToLocalCoords (x,y).x;
		int ly = (int)GlobalToLocalCoords (x,y).y;

		if (IsInsideChunk (lx, ly)) cols[lx,ly].SetActive (status);
	}

	// Change col.SetActive to col.enabled when Unity 5 is out, should be much faster by then.
	public void UpdateCollider (int x, int y) {
		if (cols == null) cols = new GameObject[Chunk.size,Chunk.size];
		
		int lx = (int)GlobalToLocalCoords (x,y).x;
		int ly = (int)GlobalToLocalCoords (x,y).y;

		if (IsInsideChunk (lx,ly)) {
			if (!cols[lx,ly]) {
				cols[lx,ly] = (GameObject)Instantiate (colPrefab, new Vector3 (x + Chunk.scale / 2, y + Chunk.scale / 2), Quaternion.identity);
				cols[lx,ly].name = "Collider " + new Vector2 (x, y).ToString();
				cols[lx,ly].transform.parent = transform;
			}

			GameObject col = cols[lx,ly];

			if (planet.GetTile (x, y) == 0) {
				col.SetActive (false);
				return;
			}else{
				if (
					planet.GetTile (x, y) != 0 &&
					planet.GetTile (x + 1, y) != 0 &&
					planet.GetTile (x, y + 1) != 0 &&
					planet.GetTile (x - 1, y) != 0 &&
					planet.GetTile (x, y - 1) != 0) {
					col.SetActive (false);
					return;
				}else{
					col.SetActive (true);
					return;
				}
			}
		}
	}

	void AddFace (int x, int y, int index, int id) {

		verts[index * 4 + 0] = new Vector3 (x,y) * Chunk.scale;
		verts[index * 4 + 1] = new Vector3 (x,y + 1) * Chunk.scale;
		verts[index * 4 + 2] = new Vector3 (x + 1,y + 1) * Chunk.scale;
		verts[index * 4 + 3] = new Vector3 (x + 1,y) * Chunk.scale;

		// Create tris dependant on nearby tiles
		if (planet.GetTile (x + startX + 1,y + startY) == 0 && planet.GetTile (x + startX,y + startY + 1) == 0 ) {
			tris[index * 6 + 0] = index * 4 + 0;
			tris[index * 6 + 1] = index * 4 + 1;
			tris[index * 6 + 2] = index * 4 + 3;
		}else if (planet.GetTile (x + startX - 1,y + startY) == 0 && planet.GetTile (x + startX,y + startY + 1)  == 0 ) {
			tris[index * 6 + 0] = index * 4 + 0;
			tris[index * 6 + 1] = index * 4 + 2;
			tris[index * 6 + 2] = index * 4 + 3;
		}else if (planet.GetTile (x + startX + 1,y + startY) == 0 && planet.GetTile (x + startX, y + startY - 1) == 0 ) {
			tris[index * 6 + 0] = index * 4 + 0;
			tris[index * 6 + 1] = index * 4 + 1;
			tris[index * 6 + 2] = index * 4 + 2;
		}else if (planet.GetTile (x + startX - 1,y + startY) == 0 && planet.GetTile (x + startX,y + startY - 1) == 0 ) {
			tris[index * 6 + 0] = index * 4 + 1;
			tris[index * 6 + 1] = index * 4 + 2;
			tris[index * 6 + 2] = index * 4 + 3;
		}else{
			tris[index * 6 + 0] = index * 4 + 0;
			tris[index * 6 + 1] = index * 4 + 1;
			tris[index * 6 + 2] = index * 4 + 2;

			tris[index * 6 + 3] = index * 4 + 2;
			tris[index * 6 + 4] = index * 4 + 3;
			tris[index * 6 + 5] = index * 4 + 0;
		}

		norms[index * 4 + 0] = Vector3.back;
		norms[index * 4 + 1] = Vector3.back;
		norms[index * 4 + 2] = Vector3.back;
		norms[index * 4 + 3] = Vector3.back;

		float s = 1f/(float)planet.tileTypes.Length;

		uvs[index * 4 + 0] = new Vector2 ((float)id * s,0);
		uvs[index * 4 + 1] = new Vector2 ((float)id * s,1);
		uvs[index * 4 + 2] = new Vector2 ((float)id * s + s,1);
		uvs[index * 4 + 3] = new Vector2 ((float)id * s + s,0);

	}
}
