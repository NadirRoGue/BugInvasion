using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class GameTarget : CreatureTemplate
{

	public float _maxHealth;

	public GameTarget (StatsSet set)
		: base (set)
	{

		_maxHealth = set.getFloat ("health");
	}

	public float getMaxHealth ()
	{
		return _maxHealth;
	}
}
