using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */  
public sealed class TowerTable
{
	 
	static class Singleton
	{
		public static TowerTable INSTANCE = new TowerTable ();
	}

	public static TowerTable getInstance ()
	{
		return Singleton.INSTANCE;
	}

	private Dictionary<string, GameTower> _towers;

	private TowerTable ()
	{
		_towers = new Dictionary<string, GameTower> ();

		Ammunition cannonBall = new Ammunition ("Cannon_Ball/cannon_ball", 50.0f);

		StatsSet cannon = new StatsSet ();
		cannon.set ("name", "Torre Cañón");
		cannon.set ("relativeModelPath", "Tower_Cannon/cannon_tower");
		cannon.set ("fracturedAssetPath", "Tower_Cannon/Fractured/cannon_tower_fractured");
		cannon.set ("attackFrequency", 1.0f);
		cannon.set ("attackDamage", 100.0f);
		cannon.set ("attackRange", 15.0f);
		cannon.set ("health", 500.0f);
		cannon.set ("ammunition", cannonBall);
		cannon.set ("coinPrice", 100);
		cannon.set ("attackParticleSystem", "PS_CannonBurst");
		cannon.set ("attackSound", "sound_cannon");
		cannon.set ("towerIcon", "Icons/Tower_Thumbnails/cannon_tower");
		cannon.set ("repairCostPerPS", 0.1f);
		registerTower (new GameTower (cannon));
	}

	public void registerTower (GameTower tower)
	{
		if (_towers.ContainsKey (tower.getName ())) {
			Debug.Log ("Tower table: Duplicate entry " + tower.getName ());
		} else {
			_towers.Add (tower.getName (), tower);
		}
	}

	public GameTowerInstance instantiateTower (string towerName, byte spawnPos)
	{
		if (_towers.ContainsKey (towerName)) {
			GameTower gt = _towers [towerName];

			GameTowerInstance gti = new GameTowerInstance (IDFactory.getNextID (), gt, spawnPos);
			World.getInstance ().registerTower (gti);

			return gti;
		}

		return null;
	}

	public GameTower getTemplateByName (string name)
	{
		if (_towers.ContainsKey (name)) {
			return _towers [name];
		}

		return null;
	}
}