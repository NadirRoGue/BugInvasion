using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class World
{

	private class Singleton
	{
		public static World INSTANCE = new World ();
	}

	public static World getInstance ()
	{
		return Singleton.INSTANCE;
	}

	private SwarmController _swarm;

	private Material _worldMaterial;
	private Transform _terrain;
	private int _worldWidth;
	private int _worldHeight;

	private Player _player;

	private Dictionary<int, GameTowerInstance> _towers;
	private Dictionary<int, GameEnemyInstance> _enemies;
	private Dictionary<int, GameTargetInstance> _targets;

	private Dictionary<byte, Creature> _targetsBySpawn;

	private World ()
	{
		_player = new Player ();
		_towers = new Dictionary<int, GameTowerInstance> ();
		_enemies = new Dictionary<int, GameEnemyInstance> ();
		_targets = new Dictionary<int, GameTargetInstance> ();

		_targetsBySpawn = new Dictionary<byte, Creature> ();
	}

	public void reset ()
	{
		_towers.Clear ();
		_enemies.Clear ();
		_targets.Clear ();
		_targetsBySpawn.Clear ();
	}

	public void setWorldMaterial (Material mat)
	{
		_worldMaterial = mat;
	}

	public void setSwarm (SwarmController swarm)
	{
		_swarm = swarm;
	}

	public Material getWorldMaterial ()
	{
		return _worldMaterial;
	}

	public SwarmController getSwarmController ()
	{
		return _swarm;
	}

	public Player getPlayer ()
	{
		return _player;
	}

	public void savePlayerData ()
	{
		_player.saveData ();
	}

	public void setWorldSize (int width, int height)
	{
		_worldWidth = width;
		_worldHeight = height;
	}

	public int getWorldWidth ()
	{
		return _worldWidth;
	}

	public int getWorldHeight ()
	{
		return _worldHeight;
	}

	public void registerTower (GameTowerInstance towerInstance)
	{
		lock (_towers) {
			_towers.Add (towerInstance.getObjectId (), towerInstance);
		}
	}

	public void unregisterTower (GameTowerInstance towerInstance)
	{
		lock (_towers) {
			_towers.Remove (towerInstance.getObjectId ());
		}
	}

	public void registerTowerInSpawn (byte spawn, GameTowerInstance tower)
	{
		if (_targetsBySpawn.ContainsKey (spawn)) {
			_targetsBySpawn.Remove (spawn);
		}

		_targetsBySpawn.Add (spawn, tower);
	}

	public void unregisterTowerBySpawn (byte spawn)
	{
		_targetsBySpawn.Remove (spawn);
	}

	public void registerEnemy (GameEnemyInstance gei)
	{
		lock (_enemies) {
			_enemies.Add (gei.getObjectId (), gei);
		}
	}

	public void unregisterEnemy (GameEnemyInstance gei)
	{
		lock (_enemies) {
			_enemies.Remove (gei.getObjectId ());
		}
	}

	public void registerTarget (GameTargetInstance gti)
	{
		lock (_targets) {
			_targets.Add (gti.getObjectId (), gti);
		}

		_targetsBySpawn.Add (0, gti);
	}

	public void unregisterTarget (GameTargetInstance gti)
	{
		lock (_targets) {
			_targets.Remove (gti.getObjectId ());
		}

		_targetsBySpawn.Remove (0);
	}

	public Creature getObjectBySpwan (byte spawn)
	{
		if (!_targetsBySpawn.ContainsKey (spawn)) {
			return null;
		}

		return _targetsBySpawn [spawn];
	}

	public void setTerrainTransform (Transform t)
	{
		_terrain = t;
	}

	public Transform getTerrainTransform ()
	{
		return _terrain;
	}

	public Dictionary<int, GameEnemyInstance> getAllEnemies ()
	{
		return _enemies;
	}

	public Dictionary<int, GameTowerInstance> getAllTowers ()
	{
		return _towers;
	}
}