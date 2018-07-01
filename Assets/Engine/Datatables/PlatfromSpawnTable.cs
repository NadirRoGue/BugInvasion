using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */  
public sealed class PlatfromSpawnTable
{
	 
	class Singleton
	{
		public static PlatfromSpawnTable INSTANCE = new PlatfromSpawnTable ();
	}

	public static PlatfromSpawnTable getInstance ()
	{
		return Singleton.INSTANCE;
	}

	private Dictionary<byte, GameObject> _spawnPlatforms;

	PlatfromSpawnTable ()
	{
		_spawnPlatforms = new Dictionary<byte, GameObject> ();
	}

	public void spawnPlatforms ()
	{

		foreach (GameObject obj in _spawnPlatforms.Values) {
			GameObject.Destroy (obj);
		}
		_spawnPlatforms.Clear ();

		Dictionary<byte, SpawnPoint> towerSpawns = SpawnTable.getInstance ().getTowerSpawns ();

		Material[] platformMat = { Resources.Load ("Materials/glowing_platform") as Material };

		foreach (byte index in towerSpawns.Keys) {

			SpawnPoint sp = towerSpawns [index];

			//SpawnTable.SpawnPoint sp = to
			Vector3[] vertices = {
				new Vector3 (0, 0, 0),
				new Vector3 (0, 0, Constants.VERTEX_SPACING),
				new Vector3 (Constants.VERTEX_SPACING, 0, 0),
				new Vector3 (Constants.VERTEX_SPACING, 0, Constants.VERTEX_SPACING)

			};

			GameObject empty = new GameObject ();
			empty.name = "spawn_position_" + index;
			empty.transform.name = empty.name + "_transform";
			empty.AddComponent<MeshFilter> ();
			empty.AddComponent<MeshRenderer> ();

			int[] faces = {
				0, 3, 2,
				0, 1, 3

			};

			Mesh platform = new Mesh ();
			platform.vertices = vertices;
			platform.triangles = faces;

			platform.RecalculateNormals ();

			empty.GetComponent<MeshFilter> ().mesh = platform;
			empty.GetComponent<MeshRenderer> ().materials = platformMat;

			empty.transform.parent = World.getInstance ().getTerrainTransform ();
			Vector3 emptyPos = Formulas.getPositionInMap (sp.getAveragePosition ());
			emptyPos.y += 0.1f;
			emptyPos.x -= Constants.VERTEX_SPACING / 2.0f;
			emptyPos.z -= Constants.VERTEX_SPACING / 2.0f;
			empty.transform.position = emptyPos;

			empty.AddComponent<BoxCollider> ();
			empty.AddComponent<SpawnIndex> ();
			empty.GetComponent<SpawnIndex> ()._spawnIndex = index;

			_spawnPlatforms.Add (index, empty);
		}
	}

	public GameObject getPlatformByIndex (byte index)
	{
		return _spawnPlatforms [index];
	}

	public void disablePlatform (byte val)
	{
		if (_spawnPlatforms.ContainsKey (val)) {
			GameObject platForm = _spawnPlatforms [val];
			//_spawnPlatforms [val].GetComponent<MeshRenderer> ().enabled = false;
			//_spawnPlatforms [val].GetComponent<MeshRenderer> ().enabled = false;
			platForm.GetComponent<MeshRenderer> ().enabled = false;
			platForm.GetComponent<MeshRenderer> ().enabled = false;
		}
	}

	public void enablePlatform (byte val)
	{
		if (_spawnPlatforms.ContainsKey (val)) {
			GameObject platForm = _spawnPlatforms [val];
			//_spawnPlatforms [val].GetComponent<MeshRenderer> ().enabled = false;
			//_spawnPlatforms [val].GetComponent<MeshRenderer> ().enabled = false;
			platForm.GetComponent<MeshRenderer> ().enabled = true;
			platForm.GetComponent<MeshRenderer> ().enabled = true;
		}	
	}
}
