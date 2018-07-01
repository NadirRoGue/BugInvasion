using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class SwarmAI
{

	private class Singleton
	{
		public static readonly SwarmAI INSTANCE = new SwarmAI ();
	}

	public static SwarmAI getInstance ()
	{
		return Singleton.INSTANCE;
	}

	class TowerSpawnInfo
	{

		public bool _active = false;
		public int _killedSoldiers = 0;
		public float _aggro = 0.0f;
	}

	private Dictionary<int, Dictionary<byte, TowerSpawnInfo>> _activeTowers;

	private SwarmAI ()
	{
		_activeTowers = new Dictionary<int, Dictionary<byte, TowerSpawnInfo>> ();
	}

	public void initialize (Dictionary<int, PathSolution> computedPaths)
	{
		_activeTowers.Clear ();

		foreach (int spawnZone in computedPaths.Keys) {
			PathSolution ps = computedPaths [spawnZone];

			Dictionary<byte, TowerSpawnInfo> pathInfo = new Dictionary<byte, TowerSpawnInfo> ();
			Dictionary<byte, Queue<Vector3>>.KeyCollection kc = ps.getAllTowersInPath ();

			TowerSpawnInfo targetInfo = new TowerSpawnInfo ();
			targetInfo._active = true;
			targetInfo._aggro = 1.0f;
			pathInfo.Add (0, targetInfo);

			foreach (byte b in kc) {
				pathInfo.Add (b, new TowerSpawnInfo ());
			}

			_activeTowers.Add (spawnZone, pathInfo);
		}
	}

	public void prepareNewWave ()
	{
		foreach (int spawnZone in _activeTowers.Keys) {
			recomputeStrategy (spawnZone);
		}
	}

	public void notifyTowerSpawn (byte spawn)
	{
		foreach (int spawnZone in _activeTowers.Keys) {
			Dictionary<byte, TowerSpawnInfo> dic = _activeTowers [spawnZone];
			if (dic.ContainsKey (spawn)) {
				dic [spawn]._active = true;
				if (LevelManager.getInstance ().isGameActive ())
					recomputeStrategy (spawnZone);
				
			}
		}
	}

	public void notifyTowerKill (byte spawn)
	{
		foreach (int spawnZone in _activeTowers.Keys) {
			Dictionary<byte, TowerSpawnInfo> dic = _activeTowers [spawnZone];
			if (dic.ContainsKey (spawn)) {
				dic [spawn]._active = false;
				if (LevelManager.getInstance ().isGameActive ())
					recomputeStrategy (spawnZone);
			}
		}
	}

	public void notifySoldierKill (int spawnZone, byte towerKiller)
	{		
		Dictionary<byte, TowerSpawnInfo> dci = _activeTowers [spawnZone];
		if (dci.ContainsKey (towerKiller)) {
			dci [towerKiller]._killedSoldiers++;
		}
	}

	private void recomputeStrategy (int spawnZone)
	{
		recomputeAggro (spawnZone);
		Queue<byte> targets = computeTargets (spawnZone);

		SwarmController.getInstance ().setTargetsToAttack (spawnZone, targets);
	}

	private void recomputeAggroWithKills (int spawnZone)
	{
		/*Dictionary<byte, TowerSpawnInfo> dic = _activeTowers [spawnZone];
		int totalKills = 0;
		foreach (TowerSpawnInfo tsi in dic.Values) {
			if (tsi._active)
				totalTowers++;
		}

		float aggro = 1.0f / totalTowers;

		foreach (TowerSpawnInfo tsi in dic.Values) {
			if (tsi._active)
				tsi._aggro = aggro;
		}*/
	}

	private void recomputeAggro (int spawnZone)
	{
		Dictionary<byte, TowerSpawnInfo> dic = _activeTowers [spawnZone];
		int totalTowers = 0;
		foreach (TowerSpawnInfo tsi in dic.Values) {
			if (tsi._active)
				totalTowers++;
		}

		float aggro = 1.0f / totalTowers;

		foreach (TowerSpawnInfo tsi in dic.Values) {
			if (tsi._active)
				tsi._aggro = aggro;
		}
	}

	private Queue<byte> computeTargets (int spawnZone)
	{
		Dictionary<byte, TowerSpawnInfo> dic = _activeTowers [spawnZone];

		Queue<byte> targets = new Queue<byte> ();

		List<byte> list = new List<byte> ();
		foreach (byte b in dic.Keys) {
			TowerSpawnInfo tsi = dic [b];
			float aggroExp = tsi._aggro * 10.0f; // 1 order of precission
			int num = Mathf.RoundToInt (aggroExp);

			while (num > 0) {
				list.Add (b);
				num--;
			}
		}

		shuffleTargers (list);
		foreach (byte b in list)
			targets.Enqueue (b);

		return targets;
	}

	private void shuffleTargers (List<byte> targets)
	{

		Random.InitState (Random.Range (int.MinValue + 1, int.MaxValue));
		
		int n = targets.Count;
		while (n > 1) {
			n--;
			int k = Random.Range (0, n + 1);
			byte value = targets [k];
			targets [k] = targets [n];
			targets [n] = value;
		}
	}
}
