using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class EnviromentProcessor
{

	public sealed class HeightMaterial
	{
		 
		private float _rangeStart;
		private float _rangeEnd;
		private Material _material;
		private List<int> _subMesh = new List<int> ();
		private bool _spawnableTrees = true;

		private const string DEFAULT_MATERIAL = "default_mat";

		public HeightMaterial (float start, float end, string materialName) : this (start, end, materialName, true)
		{
		}

		public HeightMaterial (float start, float end, string materialName, bool spawnableTrees)
		{
			_rangeStart = start;
			_rangeEnd = end;
			_spawnableTrees = spawnableTrees;

			_material = Resources.Load ("Materials/" + materialName) as Material;

			if (_material == null) {
				Debug.LogError ("TerrainMaterialTable: Could not find material " + materialName + " within the resource folder");
				_material = Resources.Load ("Materials/" + DEFAULT_MATERIAL) as Material;
			}
		}

		public float getRangeStart ()
		{
			return _rangeStart;
		}

		public float getRangeEnd ()
		{
			return _rangeEnd;
		}

		public Material getMaterial ()
		{
			return _material;
		}

		public void addTriangle (int a, int b, int c)
		{
			_subMesh.Add (a);
			_subMesh.Add (b);
			_subMesh.Add (c);
		}

		public int[] getSubMesh ()
		{
			return _subMesh.ToArray ();
		}

		public bool canSpawnTrees ()
		{
			return _spawnableTrees;
		}
	}

	private int _mapWidth;

	private List<HeightMaterial> _materialTable = new List<HeightMaterial> ();
	private Material _farViewMaterial;

	private bool[] _blueprint;

	private string _trees;

	private float _vertexSpacing;

	private TreeSpawner _spawner;

	private bool _processTrees;

	public EnviromentProcessor (int mapWidth)
	{
		_mapWidth = mapWidth;
	}

	public void shouldProcessTrees (bool val)
	{
		_processTrees = val;
	}

	public void setMapBlueprint (bool[] blueprint)
	{
		_blueprint = blueprint;
	}

	public void setTreeName (string tree)
	{
		_trees = tree;
	}

	public void setVertexSpacing (float spacing)
	{
		_vertexSpacing = spacing;
	}

	public void setFarViewMaterial (string matName)
	{
		_farViewMaterial = Resources.Load ("Materials/" + matName) as Material;
	}

	public Material getFarViewMaterial ()
	{
		return _farViewMaterial;
	}

	public void addHeightMaterial (float start, float end, string material)
	{
		addHeightMaterial (start, end, material, true);
	}

	public void addHeightMaterial (float start, float end, string material, bool spawnTrees)
	{
		_materialTable.Add (new HeightMaterial (start, end, material, spawnTrees));
	}

	public void init ()
	{
		if (_processTrees)
			_spawner = new TreeSpawner (_trees, _blueprint, _mapWidth, _vertexSpacing);
	}

	public void processTriangle (int a, int b, int c, int originalA, int originalB, int originalC, Vector3[] baseV)
	{
		Vector3 aV = baseV [originalA];
		Vector3 bV = baseV [originalB];
		Vector3 cV = baseV [originalC];

		bool addNext = false;
		foreach (HeightMaterial hm in _materialTable) {

			float start = hm.getRangeStart ();
			float end = hm.getRangeEnd ();

			float aY = aV.y;
			float bY = bV.y;
			float cY = cV.y;

			if (addNext) {
				hm.addTriangle (a, b, c);
				if (_processTrees && hm.canSpawnTrees ()) {
					_spawner.processTriangle (originalA, originalB, originalC, aV, bV, cV);
				}
				break;
			} else if (aY >= start && aY <= end) {

				if (bY >= start && bY <= end) {

					if (cY >= start && cY <= end) {
						hm.addTriangle (a, b, c);
						if (_processTrees && hm.canSpawnTrees ()) {
							_spawner.processTriangle (originalA, originalB, originalC, aV, bV, cV);
						}
						break;
					} else {
						addNext = true;
					}
				} else {
					addNext = true;
				} 
			} else if ((bY >= start && bY <= end)
			           || (cY >= start && cY <= end)) {
				addNext = true;
			}
		}
	}

	public List<HeightMaterial> getSubMaterial ()
	{
		return _materialTable;
	}

	public void onFinishBuildingTerrain ()
	{
		_spawner.spawnTrees ();
	}
}
