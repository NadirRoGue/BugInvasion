using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class SpawnPointGatherer : TextureObserver
{
	 
	private Dictionary<int, List<int>> _enemySpawns;
	private Dictionary<int, List<int>> _towerSpawns;
	private Dictionary<int, List<int>> _targetSpawns;

	private Dictionary<int, int> _enemyPixelBelonging;
	private Dictionary<int, int> _towerPixelBelonging;
	private Dictionary<int, int> _spawnPixelBelonging;

	public override  void init ()
	{
		_enemySpawns = new Dictionary<int, List<int>> ();
		_towerSpawns = new Dictionary<int, List<int>> ();
		_targetSpawns = new Dictionary<int, List<int>> ();

		_enemyPixelBelonging = new Dictionary<int, int> ();
		_towerPixelBelonging = new Dictionary<int, int> ();
		_spawnPixelBelonging = new Dictionary<int, int> ();
	}

	public override void processPixel (int i, int j, Color color)
	{

		if (color == Constants.ENEMY_SPAWN_COLOR) {
			searchSiblings (i, j, _enemySpawns, _enemyPixelBelonging);
		} else if (color == Constants.TOWER_SPAWN_COLOR) {
			searchSiblings (i, j, _towerSpawns, _towerPixelBelonging);
		} else if (color == Constants.TARGET_SPAWN_COLOR) {
			searchSiblings (i, j, _targetSpawns, _spawnPixelBelonging);
		}
	}

	private void searchSiblings (int i, int j, Dictionary<int, List<int>> database, Dictionary<int, int> belonging)
	{

		int istart = Mathf.Max (i - 1, 0);
		int iend = Mathf.Min (i + 2, _textureWidth);
		int jstart = Mathf.Max (j - 1, 0);
		int jend = Mathf.Min (j + 2, _textureWidth);

		bool siblingFound = false;

		int currentIndex = i * _textureWidth + j;

		for (int k = istart; !siblingFound && k < iend; k++) {
			for (int z = jstart; !siblingFound && z < jend; z++) {

				int tempIndex = k * _textureWidth + z;
					
				if (belonging.ContainsKey (tempIndex)) {
					int parent = findParent (tempIndex, belonging);
					belonging.Add (currentIndex, parent);
					database [parent].Add (currentIndex);
					siblingFound = true;
				}
			}
		}

		if (!siblingFound) {
			belonging.Add (currentIndex, -1);
			List<int> newList = new List<int> ();
			newList.Add (currentIndex);
			database.Add (currentIndex, newList);
		}
	}

	private int findParent (int childIndex, Dictionary<int, int> belongin)
	{
		int found = belongin [childIndex];

		if (found == -1)
			return childIndex;

		return findParent (found, belongin);
	}

	public Dictionary<int, List<int>> getTowerSpawns ()
	{
		return _towerSpawns;
	}

	public Dictionary<int, List<int>> getEnemySpawns ()
	{
		return _enemySpawns;
	}

	public Dictionary<int, List<int>> getTagetSpawns ()
	{
		return _targetSpawns;
	}
}
