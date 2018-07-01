using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */  
public sealed class GameEnemy: CreatureTemplate
{
	 
	private Object _runtimeAnimationController;

	private int _onKillCoinReward;

	private float _health;
	private float _attackDmg;
	private float _attackRange;
	private float _attackFreq;

	public GameEnemy (StatsSet set)
		: base (set)
	{
		_health = set.getFloat ("health");
		_attackDmg = set.getFloat ("attackDamage");
		_attackRange = set.getFloat ("attackRange");
		_attackFreq = set.getFloat ("attackFrequency");
		_onKillCoinReward = set.getInt ("killReward");

		_runtimeAnimationController = Resources.Load ("Models/" + set.getString ("animationController"));
	}

	public int getKillReward ()
	{
		return _onKillCoinReward;
	}

	public Object getAnimationControllerAsset ()
	{
		return _runtimeAnimationController;
	}

	public float getHealth ()
	{
		return _health;
	}

	public float getAttackDamage ()
	{
		return _attackDmg;
	}

	public float getAttackRange ()
	{
		return _attackRange;
	}

	public float getAttackFrequency ()
	{
		return _attackFreq;
	}
}
