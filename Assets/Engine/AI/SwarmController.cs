using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */  
public sealed class SwarmController
{

	class Singleton
	{
		public static SwarmController INSTANCE = new SwarmController ();
	}

	public static SwarmController getInstance ()
	{
		return Singleton.INSTANCE;
	}

	private static readonly byte MAIN_TARGET = 0;

	public Vector3[] _positions;
	public byte[] _obstacleGrid;

	private int _maxPossiblePositions = 0;

	private Dictionary<int, PathSolution> _computedPaths;
	private Dictionary<int, Queue<byte>> _attackTargets;

	private Dictionary<int, GameObject> _spawnZoneControllers;

	private int _waveArmySize;
	private int _totalArmy;
	private int _waveArmyKilled;
	private float _spawnPace;

	SwarmController ()
	{
		_computedPaths = new Dictionary<int, PathSolution> ();
		_attackTargets = new Dictionary<int, Queue<byte>> ();
		_spawnZoneControllers = new Dictionary<int, GameObject> ();
	}

	public void reset ()
	{
		_computedPaths.Clear ();
		_attackTargets.Clear ();
		_spawnZoneControllers.Clear ();
	}

	public void preComputePaths ()
	{
		_computedPaths.Clear ();

		Dictionary<int, SpawnPoint> enemySpawns = SpawnTable.getInstance ().getMonsterSpawnPoints ();
		foreach (int soldierSpawnIndex in enemySpawns.Keys) {

			SpawnPoint spawnObject = enemySpawns [soldierSpawnIndex];

			SpawnPoint target = SpawnTable.getInstance ().getTargetSpawns () [0];
			float closer = Mathf.Infinity;
			int index = 0;
			foreach (int vIndex in target.getBounds().Keys) {
				float sqrtMag = (target.getBounds () [vIndex] - spawnObject.getAveragePosition ()).sqrMagnitude;
				if (sqrtMag < closer) {
					closer = sqrtMag;
					index = vIndex;
				}
			}

			Vector3 finalTarget = Formulas.getPositionInMap (target.getBounds () [index]);
			Pathfinder pf = new Pathfinder (spawnObject, finalTarget);
			PathSolution ps = pf.computePaths ();

			GameObject go = new GameObject ("Spawn_Zone_Controller_" + soldierSpawnIndex);
			go.transform.parent = World.getInstance ().getTerrainTransform ();
			go.transform.position = Formulas.getPositionInMap (spawnObject.getAveragePosition ()); // Irrelevant, but still...
			go.AddComponent<SpawnZoneController> ()._spawnZone = soldierSpawnIndex;
			_spawnZoneControllers.Add (soldierSpawnIndex, go);

			_computedPaths.Add (soldierSpawnIndex, ps);
		}

		SwarmAI.getInstance ().initialize (_computedPaths);
	}

	public int getMaxPossiblePositions ()
	{
		return _maxPossiblePositions;
	}

	public void setPositionGrid (Vector3[] positions, byte[] obstacleGrid)
	{

		_obstacleGrid = obstacleGrid;
		_positions = new Vector3[positions.Length];

		for (int i = 0; i < positions.Length; i++) {

			_positions [i] = Formulas.getPositionInMap (positions [i]);
			if (_obstacleGrid [i] == Pathfinder.WALKABLE_CELL) {
				_maxPossiblePositions++;
			}
		}

		updateObstacleGridWithTowers ();

		SpawnPoint target = SpawnTable.getInstance ().getTargetSpawns () [0];
		_maxPossiblePositions += target.getBounds ().Count;
	}

	private void updateObstacleGridWithTowers ()
	{
		Dictionary<byte, SpawnPoint> towerSpawns = SpawnTable.getInstance ().getTowerSpawns ();

		foreach (byte key in towerSpawns.Keys) {
			SpawnPoint sp = towerSpawns [key];

			foreach (int vertexIndex in sp.getBounds().Keys) {
				_obstacleGrid [vertexIndex] = key;
				_maxPossiblePositions++;
			}
		}
	}

	public void setTargetsToAttack (int spawnZone, Queue<byte> targets)
	{
		if (_attackTargets.ContainsKey (spawnZone)) {
			_attackTargets [spawnZone] = targets;
		} else {
			_attackTargets.Add (spawnZone, targets);
		}

		// Notify the controller to update soldier orders in the next frame
		_spawnZoneControllers [spawnZone].GetComponent<SpawnZoneController> ().setTargets (targets);
	}

	public SwarmOrder createOrder (byte source, byte target, int spawnZone)
	{
		PathSolution ps = _computedPaths [spawnZone];

		if (source != 0) {
			ps = ps.getSpecifiSolution (source);
		}

		Vector3 targetAvgPos = new Vector3 ();
		Queue<Vector3> path = null;
		if (target == MAIN_TARGET) {
			path = ps.getPathToTarget ();
			targetAvgPos = SpawnTable.getInstance ().getTargetSpawns () [0].getAveragePosition ();
		} else {
			path = ps.getPathToTower (target);

			if (path == null) {
				Debug.Log ("Null order. source: " + source + ", target: " + target + ", spawnZone: " + spawnZone);
				return null;
			}

			targetAvgPos = SpawnTable.getInstance ().getTowerSpawns () [target].getAveragePosition ();
		}

		SwarmOrder order = new SwarmOrder (target, targetAvgPos, path);
		return order;
	}

	public GameEnemyInstance spawnSoldier (string attackerTemplate, int spawnZone, byte order)
	{

		SwarmOrder orderObj = createOrder (0, order, spawnZone);

		GameEnemy template = EnemyTable.getInstance ().getEnemyByName (attackerTemplate);

		if (template == null)
			return null;

		GameEnemyInstance attacker = new GameEnemyInstance (IDFactory.getNextID (), template, spawnZone);
		World.getInstance ().registerEnemy (attacker);

		Vector3 spawnPosition = _positions [_computedPaths [spawnZone].getSpawnPointIndex ()];
		attacker.getGameInstance ().transform.position = spawnPosition;

		attacker.getGameInstance ().GetComponent<EnemyController> ().setOrder (orderObj);

		return attacker;
	}

	public void notifyIdleSoldier (GameEnemyInstance gei)
	{

		int spawnZone = gei.getSpawnZoneOrigin ();

		SpawnZoneController szc = _spawnZoneControllers [spawnZone].GetComponent<SpawnZoneController> ();

		szc.addIldeSoldier (gei);
	}

	public void notifyTowerSpawn (byte index)
	{
		SwarmAI.getInstance ().notifyTowerSpawn (index);
	}

	public void beginWave (float spawnPace, Dictionary<string, int> waveData)
	{
		_spawnPace = spawnPace;

		_waveArmySize = 0;
		foreach (int amount in waveData.Values) {
			_waveArmySize += amount;
		}

		_waveArmyKilled = 0;

		SwarmAI.getInstance ().prepareNewWave ();

		foreach (GameObject spawnController in _spawnZoneControllers.Values) {
			spawnController.GetComponent<SpawnZoneController> ().setWaveData (_waveArmySize, _spawnPace);
			spawnController.GetComponent<SpawnZoneController> ().enabled = true;
		}

		_totalArmy = _waveArmySize * _spawnZoneControllers.Count;
	}

	public void notifyWaveEnd ()
	{
		foreach (GameObject spawnController in _spawnZoneControllers.Values) {
			spawnController.GetComponent<SpawnZoneController> ().enabled = false;
		}
	}

	public void notifySoldierKilled (int originZone, byte towerSkillerSpawn)
	{

		lock (this) {
			_waveArmyKilled++;
			if (_waveArmyKilled == _totalArmy) {
				notifyWaveEnd ();
				LevelManager.getInstance ().waveFinished ();
			}

			UIBuilder.INSTANCE.setEnemyArmyText (_totalArmy - _waveArmyKilled);
		}

		SwarmAI.getInstance ().notifySoldierKill (originZone, towerSkillerSpawn);
	}

	public void notifyTargetDestroyed ()
	{ 

		Dictionary<int, GameEnemyInstance> soldiers = World.getInstance ().getAllEnemies ();
		foreach (GameEnemyInstance gei in soldiers.Values) {
			gei.getGameInstance ().GetComponent<EnemyController> ().enabled = false;
		}
	}

	public float getCurrentSpawnPace ()
	{
		return _spawnPace;
	}

	public int getCurrentArmySize ()
	{
		return _totalArmy;
	}

	public int getKilledArmy ()
	{
		return _waveArmyKilled;
	}
}
