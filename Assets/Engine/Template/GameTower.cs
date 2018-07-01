using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */  
public sealed class GameTower: CreatureTemplate
{
	 
	private Object _fracturedAsset;

	private float _attackFrequency;
	private float _damagePerAttack;
	private float _attackRange;

	private float _health;

	private int _coinPrice;
	private float _repairCostPerPS;

	private string _attackPS;
	private string _attackSound;

	private Ammunition _ammunition;

	private Sprite _towerIconAsset;

	public GameTower (StatsSet set)
		: base (set)
	{

		_attackFrequency = set.getFloat ("attackFrequency");
		_damagePerAttack = set.getFloat ("attackDamage");
		_attackRange = set.getFloat ("attackRange");
		_health = set.getFloat ("health");
		_ammunition = set.getObject ("ammunition") as Ammunition;
		_coinPrice = set.getInt ("coinPrice");
		_attackPS = set.getString ("attackParticleSystem");
		_attackSound = set.getString ("attackSound");
		_repairCostPerPS = set.getFloat ("repairCostPerPS");

		_fracturedAsset = Resources.Load ("Models/" + set.getString ("fracturedAssetPath"));
		_towerIconAsset = Resources.Load (set.getString ("towerIcon"), typeof(Sprite)) as Sprite;
	}

	public Sprite getTowerIcon ()
	{
		return _towerIconAsset;
	}

	public float getRepairCostPerPS ()
	{
		return _repairCostPerPS;
	}

	public string getAttackParticleSystemName ()
	{
		return _attackPS;
	}

	public string getAttackSoundName ()
	{
		return _attackSound;
	}

	public int getPrice ()
	{
		return _coinPrice;
	}

	public Object getFracturedAsset ()
	{
		return _fracturedAsset;
	}

	public Ammunition getAmmoTemplate ()
	{
		return _ammunition;
	}

	public float getAttack ()
	{
		return _damagePerAttack;
	}

	public float getAttackRange ()
	{
		return _attackRange;
	}

	public float getAttackFrequency ()
	{
		return _attackFrequency;
	}

	public float getMaxHealth ()
	{
		return _health;
	}
}
