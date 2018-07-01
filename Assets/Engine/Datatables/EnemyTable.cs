using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class EnemyTable
{
	 
	class Singleton
	{
		public static EnemyTable INSTANCE = new EnemyTable ();
	}

	public static EnemyTable getInstance ()
	{
		return Singleton.INSTANCE;
	}

	private Dictionary<string, GameEnemy> _enemies;

	private EnemyTable ()
	{
		_enemies = new Dictionary<string, GameEnemy> ();

		StatsSet spiderSet = new StatsSet ();
		spiderSet.set ("name", "Araña Exploradora");
		spiderSet.set ("relativeModelPath", "Spider_Scout/spider_scout_walk");
		spiderSet.set ("health", 350.0f);
		spiderSet.set ("attackDamage", 50.0f);
		spiderSet.set ("attackRange", 5.0f);
		spiderSet.set ("attackFrequency", 1.0f);
		spiderSet.set ("killReward", 15);
		spiderSet.set ("animationController", "Spider_Scout/spider_scout_controller");
		registerEnemy (new GameEnemy (spiderSet));
	}

	public void registerEnemy (GameEnemy enemy)
	{
		if (_enemies.ContainsKey (enemy.getName ())) {
			Debug.Log ("EnemyTable: Duplicate entry for " + enemy.getName ());
		} else {
			_enemies.Add (enemy.getName (), enemy);
		}
	}

	public GameEnemy getEnemyByName (string name)
	{
		if (_enemies.ContainsKey (name)) {
			return _enemies [name];
		}

		return null;
	}
}
