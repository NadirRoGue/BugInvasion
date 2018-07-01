using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class SpawnTable
{
	 
	class Singleton
	{
		public static SpawnTable INSTANCE = new SpawnTable ();
	}

	public static SpawnTable getInstance ()
	{
		return Singleton.INSTANCE;
	}

	private static readonly byte SPAWN_PLATFORM_STARTING_INDEX = 2;

	private Dictionary<byte, SpawnPoint> _towerPlacePoints = new Dictionary<byte, SpawnPoint> ();
	private Dictionary<int, SpawnPoint> _targetPlacePoints = new Dictionary<int, SpawnPoint> ();
	private Dictionary<int, SpawnPoint> _enemySpawnPoints = new Dictionary<int, SpawnPoint> ();

	private Dictionary<byte, bool> _usedSpawn = new Dictionary<byte, bool> ();

	SpawnTable ()
	{
	}

	public void reset ()
	{
		_usedSpawn.Clear ();
	}

	public bool isSpawnInUse (byte val)
	{
		if (_usedSpawn.ContainsKey (val)) {
			return _usedSpawn [val];
		}

		return false;
	}

	public bool tryToUseSpawn (byte val)
	{
		if (_usedSpawn.ContainsKey (val)) {
			bool isUsed = _usedSpawn [val];

			if (isUsed)
				return false;

			_usedSpawn [val] = true;
			return true;
		} else {
			_usedSpawn.Add (val, true);
			return true;
		}
	}

	public void releaseSpawn (byte val)
	{
		if (_usedSpawn.ContainsKey (val)) {
			_usedSpawn [val] = false;
		} else {
			_usedSpawn.Add (val, false);
		}
	}

	public void gatherObserverResults (SpawnPointGatherer gatherer, Vector3[] baseV)
	{

		reset ();

		gatherSpawns (_targetPlacePoints, gatherer.getTagetSpawns (), baseV);
		gatherSpawns (_enemySpawnPoints, gatherer.getEnemySpawns (), baseV);
		gatherTowerSpawns (gatherer.getTowerSpawns (), baseV);

		Debug.Log ("SpawnTable: Loaded " + _towerPlacePoints.Count + " tower spawn point(s)");
		Debug.Log ("SpawnTable: Loaded " + _targetPlacePoints.Count + " target spawn point(s)");
		Debug.Log ("SpawnTable: Loaded " + _enemySpawnPoints.Count + " enemy spawn point(s)");
	}

	private void gatherSpawns (Dictionary<int, SpawnPoint> buffer, Dictionary<int, List<int>> rawData, Vector3[] baseV)
	{

		buffer.Clear ();

		foreach (int key in rawData.Keys) {
			List<int> vIndices = rawData [key];

			SpawnPoint sp = new SpawnPoint ();
			foreach (int indice in vIndices) {
				sp.addBound (indice, baseV [indice]);
			}

			buffer.Add (key, sp);
		}
	}

	private void gatherTowerSpawns (Dictionary<int, List<int>> rawData, Vector3[] baseV)
	{
		_towerPlacePoints.Clear ();

		byte keyStart = SPAWN_PLATFORM_STARTING_INDEX;

		foreach (int key in rawData.Keys) {
			List<int> vIndices = rawData [key];

			SpawnPoint sp = new SpawnPoint ();
			foreach (int indice in vIndices) {
				sp.addBound (indice, baseV [indice]);
			}

			_towerPlacePoints.Add (keyStart, sp);
			keyStart++;
		}

	}

	public Dictionary<int, SpawnPoint> getMonsterSpawnPoints ()
	{
		return _enemySpawnPoints;
	}

	public Dictionary<byte, SpawnPoint> getTowerSpawns ()
	{
		return _towerPlacePoints;
	}

	public SpawnPoint[] getTargetSpawns ()
	{
		SpawnPoint[] result = new SpawnPoint[_targetPlacePoints.Count];
		int i = 0;
		foreach (SpawnPoint sp in _targetPlacePoints.Values) {
			result [i++] = sp;
		}

		return result;
	}
}
