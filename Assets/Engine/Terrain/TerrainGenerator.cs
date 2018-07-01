using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */  
public sealed class TerrainGenerator
{
	 
	private int _seed;

	private float _vertexSpacing;
	private float _heightMultiplier = 1.0f;
	private float _defaultHeight = 0.1f;
	private bool _generatePlayableHeight = true;
	private Vector3[] _baseV;
	private int _worldWidth;
	private Mesh _generatedMesh;
	private float _lowestPosition;
	private bool _spawnTrees = true;

	private Material[] _generatedMaterials;
	private EnviromentProcessor _matTable;

	public TerrainGenerator (int seed, int worldWidth)
		: this (seed, worldWidth, Constants.VERTEX_SPACING)
	{
	}

	public TerrainGenerator (int seed, int worldWidth, float vertexSpacing)
	{
		_vertexSpacing = vertexSpacing;
		_worldWidth = worldWidth;
		_seed = seed == -1 ? Random.Range (0, int.MaxValue) : seed;
	}

	public void buildTerrain (bool[] mapBlueprint, Biome bi)
	{
		_baseV = new Vector3[_worldWidth * _worldWidth];
		Vector2[] UVs = new Vector2[_baseV.Length];
		float uvStep = 1.0f / (_worldWidth - 1);

		// Initialize the noise generator

		NoiseGenerator ns = new NoiseGenerator (_worldWidth, 8, true, mapBlueprint, _seed);
		ns.setInitialHeight (_defaultHeight);
		ns.generatePlayableHeight (_generatePlayableHeight);
		bi.configureAndInitNoiseGenrator (ns, _heightMultiplier, _worldWidth, _vertexSpacing, mapBlueprint);
		_matTable = bi.getEnviroment ();
		_matTable.shouldProcessTrees (_spawnTrees);
		_matTable.init ();
		_lowestPosition = ns.getBottomPlane ();

		// Build the base vertices and UV coordinates from the noise and our confinguration parameters
		for (int i = 0; i < _worldWidth; i++) {
			for (int j = 0; j < _worldWidth; j++) {
				
				int index = (i * _worldWidth) + j;
				float y = Constants.NOISE_HEIGHT_MULTIPLIER * _heightMultiplier * ns.getNoiseValue (i, j); 

				_baseV [index] = new Vector3 (_vertexSpacing * i, y, _vertexSpacing * j);
				UVs [index] = new Vector2 ((uvStep * j), 1 - (uvStep * i));
			}
		}

		// Terrain vertices (vertices will be duplicated to have a flat shading effect)
		List<Vector3> list = new List<Vector3> ();
		// UVs must be duplicated as well to ensure each vertex has his UV index
		List<Vector2> UV = new List<Vector2> ();

		int addedV = 0;
		int end = _worldWidth - 1;

		for (int i = 0; i < end; i++) {
			for (int j = 0; j < end; j++) {

				int upperIndex = (i * _worldWidth) + j;
				int lowerIndex = ((i + 1) * _worldWidth) + j;

				// Get the base vertices from where we will generate our duplicated vertices
				Vector3 a = _baseV [upperIndex];
				Vector2 aUV = UVs [upperIndex];
				Vector3 b = _baseV [lowerIndex];
				Vector2 bUV = UVs [lowerIndex];
				Vector3 c = _baseV [lowerIndex + 1];
				Vector2 cUV = UVs [lowerIndex + 1];
				Vector3 d = _baseV [upperIndex + 1];
				Vector2 dUV = UVs [upperIndex + 1];

				// Add the vertices to the list. Here is where the duplication occours
				list.Add (a);
				UV.Add (aUV);
				list.Add (c);
				UV.Add (bUV);
				list.Add (b);
				UV.Add (cUV);
				list.Add (a);
				UV.Add (aUV);
				list.Add (d);
				UV.Add (cUV);
				list.Add (c);
				UV.Add (dUV);

				// Build the triangle mesh into submeshes to allow for multiple materials
				_matTable.processTriangle (addedV, addedV + 1, addedV + 2, upperIndex, lowerIndex + 1, lowerIndex, _baseV);
				addedV += 3;
				_matTable.processTriangle (addedV, addedV + 1, addedV + 2, upperIndex, upperIndex + 1, lowerIndex + 1, _baseV);
				addedV += 3;
			}
		}

		// Create the Mesh object and configure it
		_generatedMesh = new Mesh ();

		_generatedMesh.vertices = list.ToArray ();
		_generatedMesh.uv = UV.ToArray ();

		int subMeshCount = _matTable.getSubMaterial ().Count;

		_generatedMesh.subMeshCount = subMeshCount;
		_generatedMaterials = new Material[subMeshCount];

		int subMeshIndex = 0;
		foreach (EnviromentProcessor.HeightMaterial hm in _matTable.getSubMaterial()) {
			_generatedMesh.SetTriangles (hm.getSubMesh (), subMeshIndex);
			_generatedMaterials [subMeshIndex] = hm.getMaterial ();
			subMeshIndex++;
		}

		_generatedMesh.RecalculateNormals ();
	}

	public Vector3 [] getBaseVertices ()
	{
		return _baseV;
	}

	public Mesh getGeneratedMesh ()
	{
		return _generatedMesh;
	}

	public Material[] getMaterials ()
	{
		return _generatedMaterials;
	}

	public int getSeed ()
	{
		return _seed;
	}

	public EnviromentProcessor getMaterialTable ()
	{
		return _matTable;
	}

	public void setDefaultTerrainHeight (float height)
	{
		_defaultHeight = height;
		_generatePlayableHeight = false;
	}

	public float getLowestPosition ()
	{
		return _lowestPosition;
	}

	public void setHeightMultiplier (float multiplier)
	{
		_heightMultiplier = Mathf.Max (multiplier, 0.0f);
	}

	public void setSpawnTrees (bool val)
	{
		_spawnTrees = val;
	}
}
