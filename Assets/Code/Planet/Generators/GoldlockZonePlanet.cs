using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoldlockZonePlanet : Generator {

	public override string GetTile (int x, int y, float distance, float angle) {

		string t = "Stone";
		float snow = 90*Mathf.Pow (0.92f, temperature);
		
		// Generate edge
		// This is all done pretty manually, plz don't puke at it

		// Generate desert;
		if (angle < temperature / 1.5f && angle > -temperature / 1.5f) {
			t = TileToDesert (distance, t);
		}else if (angle > 180 - temperature / 1.5f) {
			t = TileToDesert (distance, t);
		}else if (angle < -(180 - temperature / 1.5f)) { // Generate tempered
			t = TileToDesert (distance, t);
		}else if (angle > 90 - (snow) && angle < 90 + (snow)) {
			t = TileToSnow (distance, t);
		}else if (angle > -(90 + snow) && angle < -(90 - snow)) {
			t = TileToSnow (distance, t);
		}else{
			if (distance > radius - 5) t = "Dirt";
			if (distance > radius - 10 && Random.Range (0,(int)distance - radius) == 0) t = "Dirt";
			if (distance > radius - 5 && Random.Range (0,(int)distance - radius) == 0) t = "Grass";
		}

		// Cut planet into sphere
		if (distance > (float)radius - 0.1f) t = "Air";

		// Generate core
		float perlinScale = 5f;
		if (Mathf.PerlinNoise ((float)x / perlinScale,(float)y / perlinScale) > distance/(float)radius + 0.2f) t = "Lava";
		if (distance < radius / 3) t = "Lava";
		if (distance < radius / 5) t = "Magma";

		return t;
	}

	string TileToDesert (float distance, string t) {
		if (distance > radius - 5) t = "Sandstone";
		if (distance > radius - 12 && Random.Range (0,(int)distance - radius) == 0) t = "Sandstone";
		if (distance > radius - 5 && Random.Range (0,(int)distance - radius) == 0) t = "Sand";
		return t;
	}

	string TileToSnow (float distance, string t) {
		if (distance > radius - 7) t = "Snow";
		if (distance > radius - 12 && Random.Range (0,(int)distance - radius) == 0) t = "Snow";
		return t;
	}
}