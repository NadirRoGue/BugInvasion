using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuTreeSpawner : MonoBehaviour {

	public GameObject[] SpawnPositions;

	// Use this for initialization
	void Start () {
		Biome b = GetComponent<TerrainGizmo> ().getCurrentBiome ();
		string biomeTree = b.getBiomeTree ();
		Object asset = Resources.Load ("Models/Trees/" + biomeTree + "/" + biomeTree);

		foreach (GameObject go in SpawnPositions) {
			GameObject tree = GameObject.Instantiate (asset, go.transform) as GameObject;
			//tree.transform.position = new Vector3 (0, 0, 0);

			Vector3 euler = tree.transform.rotation.eulerAngles;
			int randomDegrees = Random.Range (0, 360);
			euler.z =  (float)randomDegrees;
			tree.transform.rotation = Quaternion.Euler (euler);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
