using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class UpgradeTable
{
	 
	private static class Singleton
	{
		public static readonly UpgradeTable INSTANCE = new UpgradeTable ();
	}

	public static UpgradeTable getInstance ()
	{
		return Singleton.INSTANCE;
	}

	private Dictionary<string, Upgrade[]> _towerUpgrades;

	private UpgradeTable ()
	{
		_towerUpgrades = new Dictionary<string, Upgrade[]> ();
		initialize ();
	}

	private void initialize ()
	{
		Upgrade defaultState = new Upgrade ();

		StatsSet firstUpgrade = buildUpgradeStats (120, 10.0f, 0.0f, 0.0f, 75.0f);
		addGraphicsUpdate (firstUpgrade, 0, Color.yellow);
		addGraphicsUpdate (firstUpgrade, 3, Color.yellow);
		addGraphicsUpdate (firstUpgrade, 4, Color.yellow);
		Upgrade FU = new Upgrade (firstUpgrade);

		StatsSet secondUpgrade = buildUpgradeStats (150, 15.0f, 0.0f, 0.1f, 100.0f);
		addGraphicsUpdate (secondUpgrade, 0, Color.red);
		addGraphicsUpdate (secondUpgrade, 3, Color.red);
		addGraphicsUpdate (secondUpgrade, 4, Color.red);
		Upgrade SU = new Upgrade (secondUpgrade);

		Upgrade[] cannonTowerUpgrades = {
			defaultState,
			FU,
			SU
		};

		_towerUpgrades.Add ("Torre Cañón", cannonTowerUpgrades);
	}

	private StatsSet buildUpgradeStats (int price, float attackB, float rangeB, float speedB, float healthB)
	{
		StatsSet set = new StatsSet ();
		set.set ("price", price);
		set.set ("attackBoost", attackB);
		set.set ("speedBoost", speedB);
		set.set ("rangeBoost", rangeB);
		set.set ("healthBoost", healthB);

		return set;
	}

	private void addGraphicsUpdate (StatsSet s, int index, Color c)
	{
		Dictionary<int, Color> graphics = s.getObject ("graphicsUpgrade") as Dictionary<int, Color>;

		if (graphics == null) {
			graphics = new Dictionary<int, Color> ();
			graphics.Add (index, c);

			s.set ("graphicsUpgrade", graphics);
		} else if (graphics.ContainsKey (index)) {
			graphics [index] = c;
		} else {
			graphics.Add (index, c);
		}
	}

	private void setAmmunition (StatsSet set, Ammunition a)
	{
		set.set ("ammunition", a);
	}

	public int getMaxUpgradeLevelForTower (string name)
	{
		if (_towerUpgrades.ContainsKey (name)) {
			return _towerUpgrades [name].Length - 1;
		}

		return 0;
	}

	public Upgrade getUpgrade (string name, int lvl)
	{
		if (_towerUpgrades.ContainsKey (name)) {
			Upgrade[] upgrades = _towerUpgrades [name];
			if (lvl >= upgrades.Length)
				lvl = upgrades.Length - 1;

			return upgrades [lvl];
		}

		return new Upgrade (); 
	}
}
