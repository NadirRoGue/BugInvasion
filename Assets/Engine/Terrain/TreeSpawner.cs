using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class TreeSpawner
{
	 
	private bool[] _blueprint;
	private bool[] _spawnedTrees;

	private List<Vector3> _treeSpawns = new List<Vector3> ();

	private float _vertexSpacing;

	private string _tree;
	private int _mapWidth;

	public TreeSpawner (string tree, bool[] blueprint, int mapWidth) : this (tree, blueprint, mapWidth, Constants.VERTEX_SPACING)
	{
	}

	public TreeSpawner (string tree, bool[] blueprint, int mapWidth, float vertexSpacing)
	{
		_tree = tree;
		_mapWidth = mapWidth;
		_vertexSpacing = vertexSpacing;
		_blueprint = blueprint;

		_spawnedTrees = new bool[_mapWidth * _mapWidth];
		for (int i = 0; i < _spawnedTrees.Length; i++)
			_spawnedTrees [i] = false;
	}

	public void processTriangle (int ai, int bi, int ci, Vector3 a, Vector3 b, Vector3 c)
	{

		if (_blueprint [ai] || _blueprint [bi] || _blueprint [ci])
			return;

		Vector3 up = new Vector3 (0.0f, 1.0f, 0.0f);

		Vector3 triangleNormal = Vector3.Cross (b - a, c - a).normalized;

		float cos = Vector3.Dot (up, triangleNormal);
		float sin = (Vector3.Cross (up, triangleNormal).magnitude);
		float angle = Mathf.Atan2 (sin, cos) * Mathf.Rad2Deg;

		angle = Mathf.Abs (angle);

		if (angle <= 30.0f) {
			//Debug.Log ("Spawning");
			if (_vertexSpacing <= Constants.VERTEX_SPACING) {
				List<int> vertices = new List<int> ();
				vertices.Add (ai);
				vertices.Add (bi);
				vertices.Add (ci);

				foreach (int v in vertices) {
					
					int x = v / _mapWidth;
					int y = v % _mapWidth;

					int istart = Mathf.Max (x - 1, 0);
					int iend = Mathf.Min (_mapWidth, x + 2);
					int jstart = Mathf.Max (y - 1, 0);
					int jend = Mathf.Min (_mapWidth, y + 2);

					for (int i = istart; i < iend; i++) {
						for (int j = jstart; j < jend; j++) {

							int index = i * _mapWidth + j;

							if (_spawnedTrees [index]) {
								return;
							}
						}
					}
				}
			}

			if (Random.value >= 0.95) {
				_spawnedTrees [ai] = true;
				_spawnedTrees [bi] = true;
				_spawnedTrees [ci] = true;

				addSpawnPosition (a, b, c);
			}
		}

	}

	private void addSpawnPosition (Vector3 a, Vector3 b, Vector3 c)
	{
		Vector3 avg = (a + b + c) / 3.0f;

		_treeSpawns.Add (avg);
	}

	public void spawnTrees ()
	{

		//Debug.Log (_treeSpawns.Count);
		Object asset = Resources.Load ("Models/Trees/" + _tree + "/" + _tree);

		foreach (Vector3 spawn in _treeSpawns) {
			
			Vector3 realPos = Formulas.getPositionInMap (spawn);

			GameObject go = GameObject.Instantiate (asset, World.getInstance ().getTerrainTransform ()) as GameObject;

			// Position adjust
			Bounds b = go.GetComponent<MeshRenderer> ().bounds;
			float halfYSize = b.size.y / 2.0f;
			float yFix = halfYSize * Random.value;
			realPos.y -= yFix;
			go.transform.position = realPos;

			float randomRot = (float)(Random.Range (0, 360));
			Vector3 euler = go.transform.rotation.eulerAngles;
			euler.y = randomRot;
			go.transform.rotation = Quaternion.Euler (euler);
		}
	}
}
