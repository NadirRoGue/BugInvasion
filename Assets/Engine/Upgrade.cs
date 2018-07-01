using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class Upgrade
{

	private int _price;
	private float _attackBoost = 0.0f;
	private float _attackSpeedBoost = 0.0f;
	private float _rangeBoost = 0.0f;
	private float _healthBoost = 0.0f;

	private Dictionary<int, Color> _graphicsUpgrade;
	 
	private Ammunition _newAmmunition = null;

	public Upgrade ()
	{

	}

	public Upgrade (StatsSet set)
	{
		_price = set.getInt ("price");
		_attackBoost = set.getFloat ("attackBoost");
		_attackSpeedBoost = set.getFloat ("speedBoost");
		_rangeBoost = set.getFloat ("rangeBoost");
		_healthBoost = set.getFloat ("healthBoost");
		_newAmmunition = set.getObject ("ammunition") as Ammunition;
		_graphicsUpgrade = set.getObject ("graphicsUpgrade") as Dictionary<int, Color>;
	}

	public int getPriceForUpgrade ()
	{
		return _price;
	}

	public float getAttackBoost ()
	{
		return _attackBoost;
	}

	public float getAttackSpeedBoost ()
	{
		return _attackSpeedBoost;
	}

	public float getRangeBoost ()
	{
		return _rangeBoost;
	}

	public float getHealthBoost ()
	{
		return _healthBoost;
	}

	public Ammunition getAmmunition ()
	{
		return _newAmmunition;
	}

	public Color getColorForIndex (int i, Color original)
	{
		if (_graphicsUpgrade != null && _graphicsUpgrade.ContainsKey (i)) {
			return _graphicsUpgrade [i];
		}

		return original;
	}
}
