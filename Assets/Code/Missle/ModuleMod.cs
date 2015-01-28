using UnityEngine;
using System.Collections;
[System.Serializable]

public class ModuleMod {

	public enum Type { Float, Bool };

	public string modName;
	public Type type;

	public float min;
	public float max;

	public float value;

	public void Draw (Rect rect) {
		value = Mathf.Round (GUI.HorizontalSlider (rect, Mathf.Round (value), min, max));
		GUI.Label (rect, ToString ());
	}

	public override string ToString () {
		if (type == Type.Float) return modName.ToUpper () + ": " + value.ToString ();
		if (type == Type.Bool ) {
			if (!ToBool ()) return modName.ToUpper () + ": FALSE";
			if (ToBool ()) return modName.ToUpper () + ": TRUE";
		}
		return "Something happened that shouldn't happen.";
	}

	public bool ToBool () {
		if (value < 0.1f) return false;
		if (value > 0.9f) return true;
		return false;
	}
}
