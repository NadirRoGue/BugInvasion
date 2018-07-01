using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class TerrainGizmo : MonoBehaviour {

	public Texture2D Map;
	public int Seed;
	public bool GenerateOnlyTerrain;
	public BiomeEnum BiomeType;

	private Biome biome;

	void Awake() {

		if (GenerateOnlyTerrain) {
			generateOnlyTerrain ();
		} else {
			resetEngine ();
			generateWorld ();
		}
	}

	private void resetEngine() {
		Time.timeScale = 1.0f;
		World.getInstance ().reset ();
		SwarmController.getInstance ().reset ();
	}

	private void generateWorld() {

		IDFactory.reset ();

		if (ChoosenLevel.INSTANCE != null && ChoosenLevel.INSTANCE.Level != "none") {
			LevelManager.LevelData lvlData = LevelManager.getInstance ().getLevelData (ChoosenLevel.INSTANCE.Level);
			Map = LevelManager.getInstance ().getMapTeamplate (ChoosenLevel.INSTANCE.Level, lvlData._isCustom);
			GenerateOnlyTerrain = false;
			BiomeType = lvlData._biomeType;
		}

		World.getInstance ().setWorldSize (Map.width, Map.height);

		MapTextureProcessor processor = new MapTextureProcessor ();
		NoiseBlueprintMaker nbm = new NoiseBlueprintMaker ();
		PathCollector pc = new PathCollector ();
		SpawnPointGatherer spg = new SpawnPointGatherer ();
		processor.registerObserver(nbm);
		processor.registerObserver (pc);
		processor.registerObserver (spg);

		processor.processTexture (Map);

		biome = getBiome();

		TerrainGenerator tg = new TerrainGenerator (Seed, Map.width);
		tg.buildTerrain (nbm.getBlueprint (), biome);

		spawnSourrondingLandscape (biome);

		Vector3[] vertices = tg.getBaseVertices ();

		initializeMesh (tg);
		Vector3 currentPos = transform.position;
		currentPos.y = (Constants.Y_CORRECTION = -(tg.getLowestPosition () * Constants.NOISE_HEIGHT_MULTIPLIER));
		transform.position = currentPos;

		World.getInstance ().setTerrainTransform (transform);
		World.getInstance ().setWorldMaterial (tg.getMaterialTable ().getFarViewMaterial ());

		SpawnTable.getInstance ().gatherObserverResults (spg, vertices);

		initializeStaticPS (biome);

		TowerTable.getInstance ();
		EnemyTable.getInstance ();
		UpgradeTable.getInstance ();
		PlatfromSpawnTable.getInstance ().spawnPlatforms ();

		tg.getMaterialTable ().onFinishBuildingTerrain ();

		spawnCastle ();

		SwarmController.getInstance ().setPositionGrid (vertices, pc.getWalkableGrid ());
		SwarmController.getInstance ().preComputePaths ();

		if (GetComponent<AudioSource> () != null) {
			SoundTable.getInstance ().getAudioPlayer (GetComponent<AudioSource> (), biome.getBiomeAmbientSound ());
			GetComponent<AudioSource> ().loop = true;
		}
	}

	private void spawnSourrondingLandscape(Biome baseBiome) {
		bool[] dummyBlueprint = new bool[Map.width * Map.width];

		int startI = (Map.width / 3);
		int endI = startI + startI;

		startI += 5;
		endI -= 5;

		int startJ = startI;
		int endJ = endI;

		for (int i = startI; i < endI; i++) {
			for (int j = startJ; j < endJ; j++) {
				int index = i * Map.width + j;
				dummyBlueprint [index] = true;
			}
		}

		const float HEIGHT_MULTIPLIER = 3.0f;

		TerrainGenerator tg = new TerrainGenerator (Seed, Map.width, Constants.VERTEX_SPACING * 3);
		tg.setDefaultTerrainHeight (0.0f);
		tg.setHeightMultiplier (HEIGHT_MULTIPLIER);
		tg.setSpawnTrees (false);
		tg.buildTerrain (dummyBlueprint, baseBiome);

		GameObject go = new GameObject ();
		go.name = "sourrounding_landscape";
		go.AddComponent<MeshFilter> ().mesh = tg.getGeneratedMesh ();
		go.AddComponent<MeshRenderer> ().materials = tg.getMaterials ();

		Vector3 pos = new Vector3 (-(Constants.VERTEX_SPACING * 64.0f), -(tg.getLowestPosition() * HEIGHT_MULTIPLIER * Constants.NOISE_HEIGHT_MULTIPLIER), -(Constants.VERTEX_SPACING * 64.0f));
		go.transform.position = pos;
	}

	private void spawnCastle() {

		SpawnPoint sp = SpawnTable.getInstance ().getTargetSpawns () [0];

		GameTargetInstance gti = GameTargetInstance.instantiate ();
		gti.getGameInstance ().transform.position = Formulas.getPositionInMap (sp.getAveragePosition ());

		GameObject empty = new GameObject ();
		empty.name = "target_ground_collider";
		empty.transform.parent = World.getInstance ().getTerrainTransform ();
		empty.transform.position = gti.getGameInstance ().transform.position;

		Bounds b = gti.getTemplate ().getBounds ();
		Vector3 size = b.size;
		size.y = 0.0f;
		empty.AddComponent<BoxCollider> ().size = size;
	}

	private void generateOnlyTerrain() {
		World.getInstance ().setWorldSize (Map.width, Map.height);

		MapTextureProcessor processor = new MapTextureProcessor ();
		NoiseBlueprintMaker nbm = new NoiseBlueprintMaker ();
		processor.registerObserver (nbm);
		processor.processTexture (Map);

		biome = getBiome();

		TerrainGenerator tg = new TerrainGenerator (Seed, Map.width);
		tg.setSpawnTrees (false);
		tg.buildTerrain (nbm.getBlueprint (), biome);

		initializeMesh (tg);

		initializeStaticPS (biome);

		if (GetComponent<AudioSource> () != null) {
			SoundTable.getInstance ().getAudioPlayer (GetComponent<AudioSource> (), biome.getBiomeAmbientSound ());
			GetComponent<AudioSource> ().loop = true;
		}
	}

	private void initializeMesh(TerrainGenerator tg) {
		GetComponent<MeshFilter> ().mesh = tg.getGeneratedMesh ();
		GetComponent<MeshRenderer> ().materials = tg.getMaterials ();
		GetComponent<MeshRenderer> ().receiveShadows = true;
		GetComponent<MeshRenderer> ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
	}

	private void initializeStaticPS(Biome biome) {
		if (biome.getCameraParticleSystem () != "none") {

			GameObject ps = ParticleSystemTable.getInstance ().instantiateParticleSystem (
				biome.getCameraParticleSystem(), Camera.main.transform);

			ps.name = biome.getCameraParticleSystem ();

			ps.GetComponent<ParticleSystem> ().Play ();
		}
	}
		
	private Biome getBiome() {

		// We only want specific biomes for artistic purposes
		if (GenerateOnlyTerrain) {
			List<BiomeEnum> biomesToChoose = new List<BiomeEnum> ();
			biomesToChoose.Add (BiomeEnum.BIOME_DESSERT);
			biomesToChoose.Add (BiomeEnum.BIOME_GREEN);
			biomesToChoose.Add (BiomeEnum.BIOME_LAVA_VALLEY);
			biomesToChoose.Add (BiomeEnum.BIOME_SNOW);

			if (!biomesToChoose.Contains (BiomeType)) {
				BiomeType = biomesToChoose.ToArray () [Random.Range (0, biomesToChoose.Count)];
				return Biome.intantiateBiome (BiomeType, -1);
			}
		}

		return Biome.intantiateBiome (BiomeType, Seed);
	}

	public Biome getCurrentBiome() {
		return biome;
	}

	void Start () {
		if(GetComponent<AudioSource>() != null)
			GetComponent<AudioSource> ().Play ();

		if (!GenerateOnlyTerrain)
			LevelManager.getInstance ().startGame (ChoosenLevel.INSTANCE.Level);

		Transform f = null;
		if ((f = Camera.main.transform.Find(biome.getCameraParticleSystem())) != null) {
			if(!f.GetComponent<ParticleSystem>().isPlaying)
				f.GetComponent<ParticleSystem>().Play();
		}
	}
	
	void Update () {
		
	}
}
