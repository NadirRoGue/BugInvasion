using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 */ 
public sealed class KnowlistController : MonoBehaviour {

	private Dictionary<int, GameEnemyInstance> _knowList = new Dictionary<int, GameEnemyInstance>();

	void Start () {
		
	}
	

	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		EnemyController ec = null;
		if ((ec = other.transform.GetComponent<EnemyController> ()) != null) {
			GameEnemyInstance gei = ec._enemyInstance;

			if (gei != null && !gei.isDead ()) {
				_knowList.Add (gei.getObjectId (), gei);
			}
		}
	}

	void OnTriggerExit(Collider other) {
		EnemyController ec = null;
		if ((ec = other.transform.GetComponent<EnemyController> ()) != null) {
			GameEnemyInstance gei = ec._enemyInstance;

			if (gei != null && !gei.isDead ()) {
				_knowList.Remove(gei.getObjectId ());
			}
		}
	}

	public Dictionary<int, GameEnemyInstance> getKnowList() {
		Dictionary<int, GameEnemyInstance> copy = new Dictionary<int, GameEnemyInstance> (_knowList);
		return copy;
	}
}
