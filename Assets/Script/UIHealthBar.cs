using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class UIHealthBar : MonoBehaviour {

	public Texture2D EmptyHealthBar;
	public Texture2D FullHealthBar;

	void OnGUI() {
		RectTransform rTrans = transform as RectTransform;
		Vector2 size = rTrans.rect.size;
		Vector2 pos = rTrans.position;

		GUI.BeginGroup (new Rect (pos.x /*- size.x / 2*/, pos.y /*- size.y / 2*/, size.x, size.y));
			GUI.Box (new Rect (0, 0, size.x / 2, size.y / 2), EmptyHealthBar);

		GUI.BeginGroup (new Rect (0.0f, 0.0f, size.x * 0.8f, (float)size.y));
		GUI.EndGroup ();
		GUI.EndGroup ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
