using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class GameTowerInstance: Creature
{
	 
	private Upgrade _upgrade = new Upgrade ();
	private int _upgradeLvl = -1;

	private byte _spawn;

	private bool _repairing = false;

	private float _attackDmg;
	private float _attackRng;
	private float _attackFreq;
	private Ammunition _ammo;


	public GameTowerInstance (int objectId, GameTower tower, byte spawn)
		: base (objectId, tower)
	{

		_spawn = spawn;
		PlatfromSpawnTable.getInstance ().disablePlatform (_spawn);
		World.getInstance ().registerTowerInSpawn (_spawn, this);

		_gameInstance.AddComponent<TowerController> ().setTowerInstance (this);
		_gameInstance.AddComponent<HealthBarController> ()._creature = this;

		setUpgradeLevel (0);
	}

	private void computeStats ()
	{
		_attackDmg = getTower ().getAttack () + _upgrade.getAttackBoost ();
		_attackRng = getTower ().getAttackRange () + _upgrade.getRangeBoost ();
		_attackFreq = getTower ().getAttackFrequency () - _upgrade.getAttackSpeedBoost ();

		_currentHealth += _upgrade.getHealthBoost (); 

		Ammunition newAmmo = _upgrade.getAmmunition ();
		if (newAmmo != null)
			_ammo = newAmmo;
		else
			_ammo = getTower ().getAmmoTemplate ();

		_gameInstance.GetComponent<TowerController> ().updateKnowlistRange ();
	}

	public int getCurrentUpgradeLvl ()
	{
		return _upgradeLvl;
	}

	public bool canBeUpgraded ()
	{
		int maxLevel = UpgradeTable.getInstance ().getMaxUpgradeLevelForTower (getTower ().getName ());
		return maxLevel > _upgradeLvl;
	}

	public void setUpgradeLevel (int lvl)
	{
		if (lvl != _upgradeLvl) {
			_upgradeLvl = lvl;
			_upgrade = UpgradeTable.getInstance ().getUpgrade (getTower ().getName (), _upgradeLvl);
			computeStats ();
			updateGameObjectGraphics ();
		}
	}

	public void increaseUpgradeLvl ()
	{
		int maxLevel = UpgradeTable.getInstance ().getMaxUpgradeLevelForTower (getTower ().getName ());
		if (_upgradeLvl < maxLevel) {
			_upgradeLvl++;
			_upgrade = UpgradeTable.getInstance ().getUpgrade (getTower ().getName (), _upgradeLvl);
			computeStats ();
			updateGameObjectGraphics ();
		}
	}

	private void updateGameObjectGraphics ()
	{
		Material[] mats = _gameInstance.transform.Find ("Box001").GetComponent<Renderer> ().materials;
		for (int i = 0; i < mats.Length; i++) {
			Color c = _upgrade.getColorForIndex (i, mats [i].color);
			mats [i].color = c;
		}
		_gameInstance.transform.Find ("Box001").GetComponent<Renderer> ().materials = mats;
	}

	public void setRepairing (bool val)
	{
		_repairing = val;
	}

	public bool isBeingRepaired ()
	{
		return _repairing;
	}

	public GameTower getTower ()
	{
		return _template as GameTower;
	}

	public byte getSpawn ()
	{
		return _spawn;
	}

	public float getAttackDmg ()
	{
		return _attackDmg;
	}

	public float getAttackRange ()
	{
		return _attackRng;
	}

	public float getAttackFrequency ()
	{
		return _attackFreq;
	}

	public Ammunition getAmmoTemplate ()
	{
		return _ammo;
	}

	public override float getMaxHealth ()
	{
		return getTower ().getMaxHealth () + _upgrade.getHealthBoost ();
	}

	public override void doDie (Creature killer)
	{
		World.getInstance ().unregisterTower (this);
		World.getInstance ().unregisterTowerBySpawn (_spawn);

		_gameInstance.GetComponent<HealthBarController> ().destroy ();

		Vector3 originalPos = getGameInstance ().transform.position;

		GameObject.Destroy (getGameInstance ());

		GameObject fracturedInstance = GameObject.Instantiate (
			                               getTower ().getFracturedAsset (), World.getInstance ().getTerrainTransform ()) as GameObject;
		
		fracturedInstance.transform.position = originalPos;
		fracturedInstance.AddComponent<TowerCollapse> ().Propagate = true;

		GameObject ps = ParticleSystemTable.getInstance ().instantiateParticleSystem ("PS_Collapse");
		ps.transform.position = originalPos;
		ps.AddComponent<ParticleSystemCollector> ();
		ps.GetComponent<ParticleSystem> ().Play ();
		ps.AddComponent<AudioSource> ().loop = false;
		SoundTable.getInstance ().getAudioPlayer (ps.GetComponent<AudioSource> (), "sound_collapse");
		ps.GetComponent<AudioSource> ().Play ();
		CameraShake.INSTANCE.prepareShake ();

		PlatfromSpawnTable.getInstance ().enablePlatform (_spawn);
		SpawnTable.getInstance ().releaseSpawn (_spawn);
	}

	public override void notifyTargetKilled ()
	{
		
	}
}
