using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @auhtor Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class TowerRapair : MonoBehaviour
{

	private static float repairPSPerSecond = 50.0f;

	private GameTowerInstance _instance;
	private GameObject _particleSystem;

	private float _toHeal = 0.0f;

	private GameObject _weldingSoundHolder;

	void Start ()
	{
		_instance = GetComponent<TowerController> ().getTowerInstance ();
		lock (_instance) {
			_toHeal = _instance.getMaxHealth () - _instance.getCurrentHealth ();
			if (_toHeal > 0.0f) {
				_instance.setRepairing (true);
				_particleSystem = ParticleSystemTable.getInstance ().instantiateParticleSystem ("PS_Sparks", transform);
				_particleSystem.GetComponent<ParticleSystem> ().Play ();

				_weldingSoundHolder = new GameObject ();
				_weldingSoundHolder.transform.parent = transform;
				_weldingSoundHolder.AddComponent<AudioSource> ();
				SoundTable.getInstance ().getAudioPlayer (_weldingSoundHolder.GetComponent<AudioSource> (), "sound_welding");
				_weldingSoundHolder.GetComponent<AudioSource> ().loop = true;
				_weldingSoundHolder.GetComponent<AudioSource> ().Play ();

			}
		}
	}

	void Update ()
	{

		if (_toHeal > 0.0f) {
			lock (_instance) {
				float repair = repairPSPerSecond * Time.deltaTime;
				_instance.updateHealth (repair);
				_toHeal -= repair;
			}
		} else {
			lock (_instance) {
				_instance.setRepairing (false);
			}
			_weldingSoundHolder.GetComponent<AudioSource> ().Stop ();
			GameObject.Destroy (_weldingSoundHolder);
			if (_particleSystem != null) {
				_particleSystem.GetComponent<ParticleSystem> ().Stop ();
				GameObject.Destroy (_particleSystem);
				enabled = false;
			}
		}
	}
}
