using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 * 
 * Struct:
 *  - PATH FROM SPAWN TO TARGET
 *  - PATHS FROM SPAWN TO TOWERS
 *  - FOR EACH TOWER
 *  - - PATH FROM TOWER TO TARGET
 *  - - - FOR EACH OTHER TOWER BETWEN THIS TOWER AND TARGET
 *  - - - - PATH FROM THIS TOWER TO OTHER TOWER
 */ 
public sealed class PathSolution
{
	private int _spawnIndex;
	private Queue<Vector3> _pathToTarget;
	public Dictionary<byte, Queue<Vector3>> _pathsToTowers;

	private Dictionary<byte, PathSolution> _solutionsFromTowers = null;

	public PathSolution (int spawnIndex)
	{
		_pathToTarget = new Queue<Vector3> ();
		_pathsToTowers = new Dictionary<byte, Queue<Vector3>> ();

		//_solutionsFromTowers = new Dictionary<byte, PathSolution> ();

		_spawnIndex = spawnIndex;
	}

	public void setMainPath (Stack<int> reversePath)
	{
		SwarmController swarmInstance = SwarmController.getInstance ();
		while (reversePath.Count > 0) {
			int index = reversePath.Pop ();
			Vector3 pos = swarmInstance._positions [index];
			_pathToTarget.Enqueue (pos);
		}
	}

	public void setTowerPath (byte towerSpawnId, Stack<int> reversePath)
	{
		SwarmController swarmInstance = SwarmController.getInstance ();
		Queue<Vector3> solution = new Queue<Vector3> ();

		while (reversePath.Count > 0) {
			int index = reversePath.Pop ();
			Vector3 pos = swarmInstance._positions [index];
			solution.Enqueue (pos);
		}

		_pathsToTowers.Add (towerSpawnId, solution);
	}

	public void setTowerCustomSolution (byte towerSpawn, PathSolution solution)
	{
		if (_solutionsFromTowers == null) {
			_solutionsFromTowers = new Dictionary<byte, PathSolution> ();
		}
		_solutionsFromTowers.Add (towerSpawn, solution);
	}

	public int getSpawnPointIndex ()
	{
		return _spawnIndex;
	}

	public Queue<Vector3> getPathToTarget ()
	{
		return new Queue<Vector3> (_pathToTarget);
	}

	public Queue<Vector3> getPathToTower (byte spawnIndex)
	{
		if (_pathsToTowers.ContainsKey (spawnIndex)) {
			return new Queue<Vector3> (_pathsToTowers [spawnIndex]);
		}

		return null;
	}

	public Dictionary<byte, Queue<Vector3>>.KeyCollection getAllTowersInPath ()
	{
		return _pathsToTowers.Keys;
	}

	public PathSolution getSpecifiSolution (byte source)
	{
		return _solutionsFromTowers [source];
	}
}
