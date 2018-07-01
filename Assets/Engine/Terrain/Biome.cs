using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public class Biome
{
	 
	protected float _frequencyMod;
	protected float _step;
	protected float _amplitude;

	protected bool _hasBottomPlane;
	protected float _heightMultiplier = 1.0f;

	protected float _playableHeightPercent;
	protected float _bottomHeightPercent;

	protected EnviromentProcessor _table;

	protected string _biomeAmbientSound;

	protected string _cameraPS = "none";

	protected string _biomeTree = "none";

	private int _mapWidth;

	private float _vertexSpacing;

	private bool[] _blueprint;

	public static Biome intantiateBiome (BiomeEnum biomeType, int seed = 0)
	{

		if (biomeType == BiomeEnum.BIOME_RANDOM) {
			List<BiomeEnum> biomesToChoose = new List<BiomeEnum> ();
			biomesToChoose.Add (BiomeEnum.BIOME_DESSERT);
			biomesToChoose.Add (BiomeEnum.BIOME_GREEN);
			biomesToChoose.Add (BiomeEnum.BIOME_ICE_LAKE);
			biomesToChoose.Add (BiomeEnum.BIOME_LAVA_LAKE);
			biomesToChoose.Add (BiomeEnum.BIOME_LAVA_VALLEY);
			biomesToChoose.Add (BiomeEnum.BIOME_SNOW);

			seed = seed == -1 ? Random.Range (0, int.MaxValue) : seed;

			Random.InitState (seed);
			biomeType = biomesToChoose.ToArray () [Random.Range (0, biomesToChoose.Count)];
		}

		switch (biomeType) {
		case BiomeEnum.BIOME_DESSERT:
			return new DessertBiome ();
		//case BiomeEnum.BIOME_GREEN:
		//	return new GreenBiome ();
		case BiomeEnum.BIOME_ICE_LAKE:
			return new FrozenLakeBiome ();
		case BiomeEnum.BIOME_LAVA_LAKE:
			return new LavaLakeBiome ();
		case BiomeEnum.BIOME_LAVA_VALLEY:
			return new LavaValleyBiome ();
		case BiomeEnum.BIOME_SNOW:
			return new SnowBiome ();
		}

		// Default biome
		return new GreenBiome ();
	}

	public Biome ()
	{
		initializeNoiseParameters ();
	}

	protected virtual void initializeNoiseParameters ()
	{
	}

	protected virtual void initializeBiomeParameters (float bottomHeight, float peakHeight, float bottomPlane, float playablePlane)
	{
	}

	public EnviromentProcessor getEnviroment ()
	{
		return _table;
	}

	public string getBiomeAmbientSound ()
	{
		return _biomeAmbientSound;
	}

	public string getBiomeTree ()
	{
		return _biomeTree;
	}

	public void configureAndInitNoiseGenrator (NoiseGenerator noise, float heightMultiplier, int mapWidth, float vertexSpacing, bool[] blueprint)
	{
		_heightMultiplier = heightMultiplier;
		_mapWidth = mapWidth;
		_vertexSpacing = vertexSpacing;
		_blueprint = blueprint;

		noise.setAmplitude (_amplitude);
		noise.setFrequencyMod (_frequencyMod);
		noise.setStep (_step);

		noise.setCreateBottom (_hasBottomPlane);

		noise.setPlayableHeightPercent (_playableHeightPercent);
		noise.setBottomHeightPercent (_bottomHeightPercent);

		noise.generateNoiseMap ();

		initializeBiomeParameters (
			noise.getBottomHeight (),
			noise.getPeakHeight (),
			noise.getBottomPlane (),
			noise.getPlayableHeight ()
		);
	}

	private EnviromentProcessor getEnvProcessor ()
	{
		EnviromentProcessor ep = new EnviromentProcessor (_mapWidth);
		ep.setMapBlueprint (_blueprint);
		ep.setVertexSpacing (_vertexSpacing);

		return ep;
	}

	public string getCameraParticleSystem ()
	{
		return _cameraPS;
	}

	public class GreenBiome : Biome
	{

		protected override void initializeNoiseParameters ()
		{

			// frequency = 1.0 / (period / frequencyMod)
			_frequencyMod = 4.0f;
			_step = 0.5f;
			_amplitude = 1.0f;

			_hasBottomPlane = true;
			_bottomHeightPercent = 0.025f;	// 2.5%
			_playableHeightPercent = 0.05f; // 5%
		}

		protected override void initializeBiomeParameters (float bottomHeight, float peakHeight, float bottomPlane, float playablePlane)
		{

			_table = getEnvProcessor ();

			float mult = Constants.NOISE_HEIGHT_MULTIPLIER * _heightMultiplier;

			float dirtStart = bottomHeight + ((peakHeight - bottomHeight) * 0.6f);
			float snowStart = bottomHeight + ((peakHeight - bottomHeight) * 0.8f);
			float playableStart = playablePlane * mult;

			_table.addHeightMaterial (bottomPlane * mult, bottomPlane * mult, "water", false);
			_table.addHeightMaterial (bottomPlane * mult, playableStart - (playableStart * 0.1f), "sand");
			_table.addHeightMaterial (playableStart, playableStart, "path", false);
			_table.addHeightMaterial (playableStart - (playableStart * 0.1f), dirtStart * mult, "grass");
			_table.addHeightMaterial (dirtStart * mult, snowStart * mult, "dirt");
			_table.addHeightMaterial (snowStart * mult, peakHeight * mult, "snow");

			_table.setFarViewMaterial ("water");

			_biomeAmbientSound = "sound_nature";

			_biomeTree = "tree_1";
			_table.setTreeName (_biomeTree);
		}
	}

	public class SnowBiome : Biome
	{

		protected override void initializeNoiseParameters ()
		{
			// frequency = 1.0 / (period / frequencyMod)
			_frequencyMod = 4.0f;
			_step = 0.5f;
			_amplitude = 1.0f;

			//_hasBottomPlane = true;
			//_bottomHeightPercent = 0.025f;	// 2.5%
			_playableHeightPercent = 0.05f; // 5%
		}

		protected override void initializeBiomeParameters (float bottomHeight, float peakHeight, float bottomPlane, float playablePlane)
		{
			_table = getEnvProcessor ();

			float mult = Constants.NOISE_HEIGHT_MULTIPLIER * _heightMultiplier;

			float playableStart = playablePlane * mult;

			_table.addHeightMaterial (bottomHeight * mult, playableStart - (playableStart * 0.01f), "snow");
			_table.addHeightMaterial (playableStart, playableStart, "dirt", false);
			_table.addHeightMaterial (playableStart - (playableStart * 0.1f), peakHeight * mult, "snow");

			_table.setFarViewMaterial ("snow");

			_biomeAmbientSound = "sound_blizzard";

			_cameraPS = "PS_Snow";

			_biomeTree = "tree_3";
			_table.setTreeName (_biomeTree);
		}
	}

	public class FrozenLakeBiome : Biome
	{

		protected override void initializeNoiseParameters ()
		{
			// frequency = 1.0 / (period / frequencyMod)
			_frequencyMod = 4.0f;
			_step = 0.5f;
			_amplitude = 1.0f;

			_hasBottomPlane = true;
			_bottomHeightPercent = 0.50f;	// 2.5%
			_playableHeightPercent = 0.55f; // 5%
		}

		protected override void initializeBiomeParameters (float bottomHeight, float peakHeight, float bottomPlane, float playablePlane)
		{
			_table = getEnvProcessor ();

			float mult = Constants.NOISE_HEIGHT_MULTIPLIER * _heightMultiplier;

			float playableStart = playablePlane * mult;

			_table.addHeightMaterial (bottomPlane * mult, bottomPlane * mult, "ice", false);
			_table.addHeightMaterial (bottomPlane * mult, playableStart - (playableStart * 0.1f), "snow");
			_table.addHeightMaterial (playableStart, playableStart, "dirt", false);
			_table.addHeightMaterial (playableStart - (playableStart * 0.1f), peakHeight * mult, "snow");

			_table.setFarViewMaterial ("ice");

			_biomeAmbientSound = "sound_blizzard";

			_cameraPS = "PS_Snow";

			_biomeTree = "tree_3";
			_table.setTreeName (_biomeTree);
		}
	}

	public class LavaLakeBiome : Biome
	{

		protected override void initializeNoiseParameters ()
		{
			// frequency = 1.0 / (period / frequencyMod)
			_frequencyMod = 4.0f;
			_step = 0.5f;
			_amplitude = 1.0f;

			_hasBottomPlane = true;
			_bottomHeightPercent = 0.50f;	// 2.5%
			_playableHeightPercent = 0.55f; // 5%
		}

		protected override void initializeBiomeParameters (float bottomHeight, float peakHeight, float bottomPlane, float playablePlane)
		{
			_table = getEnvProcessor ();

			float mult = Constants.NOISE_HEIGHT_MULTIPLIER * _heightMultiplier;

			float playableStart = playablePlane * mult;

			_table.addHeightMaterial (bottomPlane * mult, bottomPlane * mult, "lava", false);
			_table.addHeightMaterial (bottomPlane * mult, playableStart - (playableStart * 0.1f), "rock");
			_table.addHeightMaterial (playableStart, playableStart, "dark_dirt", false);
			_table.addHeightMaterial (playableStart - (playableStart * 0.1f), peakHeight * mult, "rock");

			_table.setFarViewMaterial ("lava");

			_biomeAmbientSound = "sound_lava";

			_biomeTree = "tree_2";
			_table.setTreeName (_biomeTree);
		}
	}

	public class LavaValleyBiome : Biome
	{

		protected override void initializeNoiseParameters ()
		{

			// frequency = 1.0 / (period / frequencyMod)
			_frequencyMod = 4.0f;
			_step = 0.5f;
			_amplitude = 1.0f;

			_hasBottomPlane = true;
			_bottomHeightPercent = 0.025f;	// 2.5%
			_playableHeightPercent = 0.05f; // 5%
		}

		protected override void initializeBiomeParameters (float bottomHeight, float peakHeight, float bottomPlane, float playablePlane)
		{

			_table = getEnvProcessor ();

			float mult = Constants.NOISE_HEIGHT_MULTIPLIER * _heightMultiplier;

			float playableStart = playablePlane * mult;

			_table.addHeightMaterial (bottomPlane * mult, bottomPlane * mult, "lava", false);
			_table.addHeightMaterial (bottomPlane * mult, playableStart - (playableStart * 0.1f), "rock");
			_table.addHeightMaterial (playableStart, playableStart, "dark_dirt", false);
			_table.addHeightMaterial (playableStart - (playableStart * 0.1f), peakHeight * mult, "rock");

			_table.setFarViewMaterial ("lava");

			_biomeAmbientSound = "sound_lava";

			_biomeTree = "tree_2";
			_table.setTreeName (_biomeTree);
		}
	}

	public class DessertBiome : Biome
	{

		protected override void initializeNoiseParameters ()
		{

			// frequency = 1.0 / (period / frequencyMod)
			_frequencyMod = 4.0f;
			_step = 0.5f;
			_amplitude = 1.0f;

			_hasBottomPlane = true;
			_bottomHeightPercent = 0.025f;	// 2.5%
			_playableHeightPercent = 0.05f; // 5%
		}

		protected override void initializeBiomeParameters (float bottomHeight, float peakHeight, float bottomPlane, float playablePlane)
		{

			_table = getEnvProcessor ();

			float mult = Constants.NOISE_HEIGHT_MULTIPLIER * _heightMultiplier;

			float playableStart = playablePlane * mult;

			_table.addHeightMaterial (bottomPlane * mult, bottomPlane * mult, "water", false);
			_table.addHeightMaterial (bottomPlane * mult, playableStart - (playableStart * 0.1f), "sand");
			_table.addHeightMaterial (playableStart, playableStart, "path", false);
			_table.addHeightMaterial (playableStart - (playableStart * 0.1f), peakHeight * mult, "sand");

			_table.setFarViewMaterial ("sand");

			_biomeAmbientSound = "sound_nature";

			_biomeTree = "tree_4";
			_table.setTreeName (_biomeTree);
		}
	}
}
