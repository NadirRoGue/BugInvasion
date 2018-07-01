using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class GameEnemyInstance: Creature
{
	 
	private int _spawnZoneSource;

	public GameEnemyInstance (int objectId, GameEnemy template, int spawnSource)
		: base (objectId, template)
	{

		_gameInstance.AddComponent<EnemyController> ()._enemyInstance = this;
		_gameInstance.AddComponent<HealthBarController> ()._creature = this;
		_spawnZoneSource = spawnSource;

		RuntimeAnimatorController enemyController 
			= GameObject.Instantiate (getEnemyTemplate ().getAnimationControllerAsset ()) as RuntimeAnimatorController;

		_gameInstance.GetComponent<Animator> ().runtimeAnimatorController = enemyController;
	}

	public int getSpawnZoneOrigin ()
	{
		return _spawnZoneSource;
	}

	public GameEnemy getEnemyTemplate ()
	{
		return _template as GameEnemy; 
	}

	public override float getMaxHealth ()
	{
		return getEnemyTemplate ().getHealth ();
	}

	public void updateAnimation (bool attack)
	{
		_gameInstance.GetComponent<Animator> ().SetBool ("Attacking", attack);
	}

	public void animatorSwitch (bool enable)
	{
		_gameInstance.GetComponent<Animator> ().enabled = enable;
	}

	public override void notifyTargetKilled ()
	{
		//SwarmController.getInstance().notifyIdleSoldier(this);
	}

	public override void doDie (Creature killer)
	{
		GameTowerInstance gti = killer as GameTowerInstance;
		SwarmController.getInstance ().notifySoldierKilled (_spawnZoneSource, gti.getSpawn ());
		LevelManager.getInstance ().notifyEnemyKilled (getEnemyTemplate ());

		World.getInstance ().unregisterEnemy (this);

		_gameInstance.GetComponent<EnemyController> ().enabled = false;
		_gameInstance.GetComponent<HealthBarController> ().destroy ();

		_gameInstance.AddComponent<AudioSource> ().loop = false;
		SoundTable.getInstance ().getAudioPlayer (_gameInstance.GetComponent<AudioSource> (), "sound_enemy_death");
		_gameInstance.GetComponent<AudioSource> ().Play ();
		_gameInstance.GetComponent<Animator> ().SetBool ("Death", true);
		_gameInstance.AddComponent<EnemyDeathController> ();

		//GameObject.Destroy (_gameInstance);
	}
}
