using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public abstract class Creature
{

	private readonly int _objectId;

	protected CreatureTemplate _template;
	protected GameObject _gameInstance;

	protected float _currentHealth;

	public Creature (int objectId, CreatureTemplate template)
	{
		_objectId = objectId;
		_template = template;

		_gameInstance = GameObject.Instantiate (_template.getGameAsset (), World.getInstance ().getTerrainTransform ()) as GameObject;
		template.setBounds (_gameInstance);

		_currentHealth = getMaxHealth ();
	}

	public Vector3 getPosition ()
	{
		return _gameInstance.transform.position;
	}

	public int getObjectId ()
	{
		return _objectId;
	}

	public GameObject getGameInstance ()
	{
		return _gameInstance;
	}

	public float getCurrentHealth ()
	{
		return _currentHealth;
	}

	public void updateHealth (float delta)
	{
		_currentHealth += delta;
		_currentHealth = Mathf.Clamp (_currentHealth, 0, getMaxHealth ());
	}

	public CreatureTemplate getTemplate ()
	{
		return _template;
	}

	public void onAttack (Creature attacker, float dmg)
	{
		if (_currentHealth > 0.0f) {
			_currentHealth -= dmg;

			if (_currentHealth <= 0.0f) {
				doDie (attacker);
			}
		}
	}

	public virtual bool isDead ()
	{
		return _currentHealth <= 0.0f;
	}

	public abstract void doDie (Creature killer);

	public abstract void notifyTargetKilled ();

	public abstract float getMaxHealth ();
}
