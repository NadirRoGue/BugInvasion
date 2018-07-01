using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class SoundTable
{
	 
	private class Singleton
	{
		public static SoundTable INSTANCE = new SoundTable ();
	}

	public static SoundTable getInstance ()
	{
		return Singleton.INSTANCE;
	}

	public class SoundData
	{
		public AudioClip _clip = null;
		public float _volumeMultiplier = 1.0f;

		public SoundData (float volumeMultiplier)
		{
			_volumeMultiplier = volumeMultiplier;
		}
	}

	private Dictionary<string, SoundData> _soundCache;
	private List<AudioSource> _activePlayers;

	private SoundTable ()
	{
		_soundCache = new Dictionary<string, SoundData> ();
		_activePlayers = new List<AudioSource> ();

		registerSound ("sound_blizzard");
		registerSound ("sound_cannon", 0.5f);
		registerSound ("sound_collapse");
		registerSound ("sound_enemy_death");
		registerSound ("sound_enemy_hit");
		registerSound ("sound_energy_weapon");
		registerSound ("sound_lava");
		registerSound ("sound_nature");
		registerSound ("sound_welding", 0.7f);
	}

	private void registerSound (string name)
	{
		registerSound (name, 1.0f);
	}

	private void registerSound (string name, float volumeMultiplier)
	{
		if (!_soundCache.ContainsKey (name)) {
			_soundCache.Add (name, new SoundData (volumeMultiplier));
		}
	}

	public void scalePlayers ()
	{
		foreach (AudioSource audioS in _activePlayers) {
			if (audioS != null) {
				audioS.volume = Config.SOUND_VOLUME;
			}
		}
	}

	public AudioSource getAudioPlayer (AudioSource audioS, string audioName)
	{
		SoundData data = _soundCache [audioName];
		if (data._clip == null) {
			data._clip = (Resources.Load<AudioClip> ("Sounds/" + audioName));// as AudioClip);
			_soundCache [audioName] = data;
		}

		audioS.clip = data._clip;
		audioS.volume = Config.SOUND_VOLUME * data._volumeMultiplier;

		_activePlayers.Add (audioS);

		return audioS;
	}

	public void onMatchEnd ()
	{
		foreach (AudioSource audio in _activePlayers) {
			Component.Destroy (audio);
		}

		_activePlayers.Clear ();
	}
}
