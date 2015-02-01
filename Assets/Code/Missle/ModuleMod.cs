using UnityEngine;
using System.Collections;
[System.Serializable]

public class ModuleMod {

	public enum Type { Float, Bool };

	public string modName;
	public Type type;

	public float min;
	public float max;
	public float start;

	public float value;

	public void Draw (Rect rect, bool interactable) {
		value = Mathf.Round (GUI.HorizontalSlider (rect, Mathf.Round (value), min, max));
		if (!interactable) value = start;
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
