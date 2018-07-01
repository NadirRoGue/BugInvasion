using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class GameTargetInstance : Creature
{
	 
	private static GameTarget _templateObject = null;

	public static GameTargetInstance instantiate ()
	{
		int objectId = IDFactory.getNextID ();

		if (_templateObject == null) {
			StatsSet set = new StatsSet ();
			set.set ("name", "Castillo");
			set.set ("relativeModelPath", "Target_Castle/target_castle");
			set.set ("health", 2500.0f);
			_templateObject = new GameTarget (set);
		}

		GameTargetInstance gti = new GameTargetInstance (objectId, _templateObject);
		World.getInstance ().registerTarget (gti);

		return gti;
	}

	public GameTargetInstance (int objectId, CreatureTemplate template)
		: base (objectId, template)
	{

		_gameInstance.AddComponent<HealthBarController> ()._creature = this;
	}

	public GameTarget getTargetTemplate ()
	{
		return _template as GameTarget;
	}

	public override float getMaxHealth ()
	{
		return getTargetTemplate ().getMaxHealth ();
	}

	public override void notifyTargetKilled ()
	{
		
	}

	public override void doDie (Creature killer)
	{
		SwarmController.getInstance ().notifyTargetDestroyed ();

		Vector3 originalPos = _gameInstance.transform.position;

		GameObject.Destroy (_gameInstance);

		GameObject fractured = GameObject.Instantiate (
			                       Resources.Load ("Models/Target_Castle/Fractured/target_castle_fractured"), 
			                       World.getInstance ().getTerrainTransform ()) as GameObject;

		fractured.transform.position = originalPos;

		fractured.AddComponent<TowerCollapse> ().setFlags (true, true);

		GameObject ps = ParticleSystemTable.getInstance ().instantiateParticleSystem ("PS_Collapse");
		ps.transform.position = originalPos;
		ps.AddComponent<ParticleSystemCollector> ();
		ps.GetComponent<ParticleSystem> ().Play ();
		ps.AddComponent<AudioSource> ().loop = false;
		SoundTable.getInstance ().getAudioPlayer (ps.GetComponent<AudioSource> (), "sound_collapse");
		ps.GetComponent<AudioSource> ().Play ();

		CameraShake.INSTANCE.prepareShake ();

		LevelManager.getInstance ().notifyTargetDestroyed ();
	}
}
