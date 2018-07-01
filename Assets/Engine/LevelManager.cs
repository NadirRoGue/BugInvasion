using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class LevelManager
{

	class Singleton
	{
		public static LevelManager INSTANCE = new LevelManager ();
	}

	public static LevelManager getInstance ()
	{
		return Singleton.INSTANCE;
	}

	public class WaveData
	{
		public Dictionary<string, int> _enemiesPerSubWave = new Dictionary<string, int> ();
	}

	public class LevelData
	{
		public bool _isCustom = false;
		public int _levelIndex;
		public int _unlockLevel;

		public int _startingCredits;

		public float _spawnPace;

		public BiomeEnum _biomeType;

		public int _waveCount;
		public WaveData[] _wavesData;

		public List<string> _unlockTowers = new List<string> ();
	}

	private Dictionary<string, LevelData> _levels;

	private string _currentLevel;
	private int _currentWave;
	private bool _gameActive;
	private int _currentCredits;

	private bool _gamePaused = false;

	private LevelManager ()
	{
		_levels = new Dictionary<string, LevelData> ();

		initialize ();
	}

	private void initialize ()
	{
		// Level 1
		LevelData lvl1 = createLevel ("map1", 0, 1, 4, 5.0f, BiomeEnum.BIOME_LAVA_VALLEY, 300);
		addSubWaveData (lvl1, 0, "Araña Exploradora", 5);
		addSubWaveData (lvl1, 1, "Araña Exploradora", 8);
		addSubWaveData (lvl1, 2, "Araña Exploradora", 10);
		addSubWaveData (lvl1, 3, "Araña Exploradora", 12);

		LevelData lvl2 = createLevel ("map2", 1, 2, 4, 5.0f, BiomeEnum.BIOME_SNOW, 300);
		addSubWaveData (lvl2, 0, "Araña Exploradora", 5);
		addSubWaveData (lvl2, 1, "Araña Exploradora", 8);
		addSubWaveData (lvl2, 2, "Araña Exploradora", 10);
		addSubWaveData (lvl2, 3, "Araña Exploradora", 12);

		LevelData lvl3 = createLevel ("map3", 2, 3, 5, 5.0f, BiomeEnum.BIOME_GREEN, 300);
		addSubWaveData (lvl3, 0, "Araña Exploradora", 5);
		addSubWaveData (lvl3, 1, "Araña Exploradora", 8);
		addSubWaveData (lvl3, 2, "Araña Exploradora", 10);
		addSubWaveData (lvl3, 3, "Araña Exploradora", 12);
		addSubWaveData (lvl3, 4, "Araña Exploradora", 15);

		LevelData lvl4 = createLevel ("map4", 3, 4, 5, 5.0f, BiomeEnum.BIOME_DESSERT, 300);
		addSubWaveData (lvl4, 0, "Araña Exploradora", 5);
		addSubWaveData (lvl4, 1, "Araña Exploradora", 8);
		addSubWaveData (lvl4, 2, "Araña Exploradora", 10);
		addSubWaveData (lvl4, 3, "Araña Exploradora", 12);
		addSubWaveData (lvl4, 4, "Araña Exploradora", 15);

		LevelData lvl5 = createLevel ("map5", 4, 5, 5, 5.0f, BiomeEnum.BIOME_LAVA_LAKE, 300);
		addSubWaveData (lvl5, 0, "Araña Exploradora", 5);
		addSubWaveData (lvl5, 1, "Araña Exploradora", 8);
		addSubWaveData (lvl5, 2, "Araña Exploradora", 10);
		addSubWaveData (lvl5, 3, "Araña Exploradora", 12);
		addSubWaveData (lvl5, 4, "Araña Exploradora", 15);

		LevelData lvl6 = createLevel ("map6", 5, 6, 5, 5.0f, BiomeEnum.BIOME_ICE_LAKE, 300);
		addSubWaveData (lvl6, 0, "Araña Exploradora", 5);
		addSubWaveData (lvl6, 1, "Araña Exploradora", 8);
		addSubWaveData (lvl6, 2, "Araña Exploradora", 10);
		addSubWaveData (lvl6, 3, "Araña Exploradora", 12);
		addSubWaveData (lvl6, 4, "Araña Exploradora", 15);

		LevelData lvl7 = createLevel ("map7", 6, 7, 5, 5.0f, BiomeEnum.BIOME_GREEN, 300);
		addSubWaveData (lvl7, 0, "Araña Exploradora", 5);
		addSubWaveData (lvl7, 1, "Araña Exploradora", 8);
		addSubWaveData (lvl7, 2, "Araña Exploradora", 10);
		addSubWaveData (lvl7, 3, "Araña Exploradora", 12);
		addSubWaveData (lvl7, 4, "Araña Exploradora", 15);

		LevelData lvl8 = createLevel ("map8", 7, 8, 5, 5.0f, BiomeEnum.BIOME_DESSERT, 300);
		addSubWaveData (lvl8, 0, "Araña Exploradora", 5);
		addSubWaveData (lvl8, 1, "Araña Exploradora", 8);
		addSubWaveData (lvl8, 2, "Araña Exploradora", 10);
		addSubWaveData (lvl8, 3, "Araña Exploradora", 12);
		addSubWaveData (lvl8, 4, "Araña Exploradora", 15);

		LevelData lvl9 = createLevel ("map9", 8, -1, 6, 5.0f, BiomeEnum.BIOME_RANDOM, 300);
		addSubWaveData (lvl9, 0, "Araña Exploradora", 5);
		addSubWaveData (lvl9, 1, "Araña Exploradora", 8);
		addSubWaveData (lvl9, 2, "Araña Exploradora", 10);
		addSubWaveData (lvl9, 3, "Araña Exploradora", 12);
		addSubWaveData (lvl9, 4, "Araña Exploradora", 15);
		addSubWaveData (lvl9, 5, "Araña Exploradora", 20);
	}

	private LevelData createLevel (string name, int levelIndex, int unlockLevel, int waveCount, float spawnPace, BiomeEnum biomeType, int startingCredits, bool isCustom = false)
	{
		LevelData d = new LevelData ();
		d._isCustom = isCustom;
		d._levelIndex = levelIndex;
		d._unlockLevel = unlockLevel;
		d._waveCount = waveCount;
		d._wavesData = new WaveData[waveCount];
		d._startingCredits = startingCredits;

		for (int i = 0; i < waveCount; i++) {
			d._wavesData [i] = new WaveData ();
		}

		d._spawnPace = spawnPace;
		d._biomeType = biomeType;

		_levels.Add (name, d);

		return d;
	}

	private void addSubWaveData (LevelData d, int wave, string enemy, int amount)
	{
		if (wave > -1 && wave < d._waveCount)
			d._wavesData [wave]._enemiesPerSubWave.Add (enemy, amount);
	}

	private void addUnlockedTowers (LevelData d, string unlockedTower)
	{
		d._unlockTowers.Add (unlockedTower);
	}

	public bool processCustomLevel (string name, Dictionary<string, string> rawData)
	{

		if (_levels.ContainsKey (name))
			return true;

		try {
			int levelIndex = -1;
			int unlockedLevel = -1;

			BiomeEnum biome = (BiomeEnum)System.Enum.Parse (typeof(BiomeEnum), rawData ["biome"]);

			float spawnPace = float.Parse (rawData ["mob_spawn_pace_seconds"]);
			int startingCredits = int.Parse (rawData ["starting_credits"]);
			int numWaves = int.Parse (rawData ["num_waves"]);

			LevelData lvl = createLevel (name, levelIndex, unlockedLevel, numWaves, spawnPace, biome, startingCredits, true);

			for (int i = 0; i < numWaves; i++) {
				string waveName = "wave_" + (i + 1) + "_enemy";
				string rawStr = rawData [waveName];

				string[] enemies = rawStr.Split (';');
				foreach (string s in enemies) {
					string[] enemyData = s.Split (',');
					if (enemyData.Length > 1)
						addSubWaveData (lvl, i, enemyData [0], int.Parse (enemyData [1]));
				}
			}
		} catch (System.Exception e) {
			Debug.Log (e);
			return false;
		}

		return true;
	}

	public Texture2D getMapTeamplate (string name, bool isCustom)
	{
		if (isCustom)
			return CustomMapTable.getInstance ().getCustomMap (name);

		return Resources.Load ("Maps/" + ChoosenLevel.INSTANCE.Level) as Texture2D;
	}

	public bool isGameActive ()
	{
		return _gameActive;
	}

	public int getNumLevels ()
	{
		return _levels.Count;
	}

	public LevelData getLevelData (string name)
	{
		if (_levels.ContainsKey (name)) {
			return _levels [name];
		}

		return null;
	}

	public void startGame (string map)
	{
		_currentLevel = map;
		_currentWave = 0;

		_currentCredits = _levels [_currentLevel]._startingCredits;
		UIBuilder.INSTANCE.setCoinText (_currentCredits);

		AssetHolderScript.INSTANCE.CountdownCanvas.enabled = true;
		CountDownController.INSTANCE.initializeCountdown ();
	}

	public void countDownFinished ()
	{

		LevelData data = _levels [_currentLevel];
		_gameActive = true;
		SwarmController.getInstance ().beginWave (data._spawnPace, data._wavesData [_currentWave]._enemiesPerSubWave);
		UIBuilder.INSTANCE.setEnemyArmyText (SwarmController.getInstance ().getCurrentArmySize ());
		UIBuilder.INSTANCE.setWaveText (_currentWave + 1, data._waveCount);
	}

	public void waveFinished ()
	{
		_gameActive = false;
		LevelData data = _levels [_currentLevel];
		if ((data._waveCount - 1) == _currentWave) {

			endLevelCleanup ();

			setEndGameMessage ("¡Has superado todas las oleadas de enemigos! ¡VICTORIA!");
			World.getInstance ().getPlayer ().setLevelCompleted (data._levelIndex, true);
			World.getInstance ().getPlayer ().setLevelUnlocked (data._unlockLevel, true);

		} else {
			ScreenMessageController.INSTANCE.showTempMessage ("Oleada " + (_currentWave + 1) + " superada!", 3.5f);
			_currentWave += 1;
			CountDownController.INSTANCE.initializeCountdown ();
		}
	}

	public void notifyEnemyKilled (GameEnemy template)
	{
		_currentCredits += template.getKillReward ();
		UIBuilder.INSTANCE.setCoinText (_currentCredits);
	}

	public void notifyTargetDestroyed ()
	{
		GameObject obj = new GameObject ();
		obj.transform.parent = World.getInstance ().getTerrainTransform (); // Ensure he is destroyed

		obj.AddComponent<SlowDefeat> ();
	}

	public void notifySlowDefeatEnd ()
	{
		endLevelCleanup ();
		setEndGameMessage ("¡El enemigo ha destruido tu base! ¡Has sido derrotado!");
	}

	private void setEndGameMessage (string msg)
	{
		Canvas canvas = AssetHolderScript.INSTANCE.EndGameCanvas;

		Text t = canvas.transform.Find ("Panel").Find ("End_text_panel").Find ("End_text").GetComponent<Text> ();
		t.text = msg;
		canvas.enabled = true;
	}

	public bool attemptToPay (int amount)
	{
		if (_currentCredits < amount)
			return false;

		_currentCredits -= amount;
		UIBuilder.INSTANCE.setCoinText (_currentCredits);
		return true;
	}

	public int getCurrentCredits ()
	{
		return _currentCredits;
	}

	public void endLevelCleanup ()
	{
		SoundTable.getInstance ().onMatchEnd ();
	}

	public void setGamePaused(bool val)
	{
		_gamePaused = val;
	}

	public bool isGamePaused()
	{
		return _gamePaused;
	}
}
