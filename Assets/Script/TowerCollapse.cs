using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class TowerCollapse : MonoBehaviour {

	public bool Propagate = false;
	public bool isFinalTarget = false;
	public float _explosionStrength = 10.0f;
	private float _dissapearIn = 4.0f;

	public void setFlags(bool propagate, bool finalTarget) {
		Propagate = propagate;
		isFinalTarget = finalTarget;
	}

	void Start () {
		if (Propagate) {
			float explosion = 0.1f;//Random.Range (3.0f, 7.0f);

			/*if (isFinalTarget) {
				explosion = Random.Range (3.0f, 7.0f);
			}*/

			foreach (Transform t in transform) {
				t.gameObject.AddComponent<BoxCollider> ().size = new Vector3 (0.2f, 0.2f, 0.2f);
				t.gameObject.AddComponent<Rigidbody> ();
				t.gameObject.AddComponent<TowerCollapse> ()._explosionStrength = explosion;
				t.gameObject.GetComponent<TowerCollapse> ().Propagate = false;

				enabled = false;
			}
		}

		if (!Propagate) {
			Vector3 pos = transform.parent.position;
			//pos.y += 2.5f;
			GetComponent<Rigidbody> ().AddExplosionForce (_explosionStrength, pos, 15.0f, 0.0f, ForceMode.Impulse); 
		}
	}
	
	void Update () {
		_dissapearIn -= Time.deltaTime;

		if (_dissapearIn < 0.0f) {
			GameObject.Destroy (gameObject);
			enabled = false;
		}
	}
}
