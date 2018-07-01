using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */  
public sealed class Player
{
	 
	// Serializable class to load/save game stats to filesystem
	[Serializable]
	public class GameSaveData
	{
		public List<string> _unlockedTowers = new List<string> ();
		public List<int> _completedLevels = new List<int> ();
		public List<int> _unlockedLevels = new List<int> ();

		public float _soundVolume;
		public float _mouseSensitivity;
	}

	private bool[] _compledtedLevels;
	private bool[] _unlockedLevels;
	private List<string> _unlockedTowers = new List<string> ();

	public Player ()
	{
		int numLevels = LevelManager.getInstance ().getNumLevels ();
		_compledtedLevels = new bool[numLevels];
		_unlockedLevels = new bool[numLevels];

		_unlockedLevels [0] = true;

		loadSavedData ();
	}

	public void loadSavedData ()
	{

		try {
			if (File.Exists (Application.persistentDataPath + Constants.SAVE_GAME_RELATIVE_PATH)) {
				BinaryFormatter bf = new BinaryFormatter ();
				FileStream fs = File.Open (Application.persistentDataPath + Constants.SAVE_GAME_RELATIVE_PATH, FileMode.Open);

				GameSaveData gsd = (GameSaveData)(bf.Deserialize (fs));
				fs.Close ();

				foreach (string towerName in gsd._unlockedTowers) {
					_unlockedTowers.Add (towerName);
				}

				foreach (int levelCompleted in gsd._completedLevels) {
					_compledtedLevels [levelCompleted] = true;
				}

				foreach (int unlockedLevel in gsd._unlockedLevels) {
					_unlockedLevels [unlockedLevel] = true;
				}

				Config.SOUND_VOLUME = gsd._soundVolume;
				Config.MOUSE_SENSITIVITY = gsd._mouseSensitivity;
			} 
		} catch (Exception e) {
			Debug.Log ("There was a problem loading player data");
			Debug.Log (e);
		}
	}

	public void saveData ()
	{
		try {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream fs = File.Open (Application.persistentDataPath + Constants.SAVE_GAME_RELATIVE_PATH, FileMode.OpenOrCreate);

			GameSaveData gsd = new GameSaveData ();

			foreach (string gt in _unlockedTowers) {
				gsd._unlockedTowers.Add (gt);
			}

			if (_unlockedLevels != null && _compledtedLevels != null) {
				int length = _unlockedLevels.Length;
				for (int i = 0; i < length; i++) {

					if (_unlockedLevels [i])
						gsd._unlockedLevels.Add (i);

					if (_compledtedLevels [i])
						gsd._completedLevels.Add (i);
				}
			}

			gsd._soundVolume = Config.SOUND_VOLUME;
			gsd._mouseSensitivity = Config.MOUSE_SENSITIVITY;

			bf.Serialize (fs, gsd);

			fs.Close ();
		} catch (Exception e) {
			Debug.Log ("There was a problem saving player data");
			Debug.Log (e);
		}
	}

	public List<string> getUnlockedTowers ()
	{
		return _unlockedTowers;
	}

	public bool[] getUnlockedLevels ()
	{
		return _unlockedLevels;
	}

	public bool[] getCompletedLevels ()
	{
		return _compledtedLevels;
	}

	public void setLevelUnlocked (int index, bool val)
	{
		if (index > -1 && index < _unlockedLevels.Length)
			_unlockedLevels [index] = val;
	}

	public void setLevelCompleted (int index, bool val)
	{
		if (index > -1 && index < _compledtedLevels.Length)
			_compledtedLevels [index] = val;
	}
}
