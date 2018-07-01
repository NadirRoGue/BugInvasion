using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class Pathfinder
{

	class Node : FastPriorityQueueNode
	{
		public int _i;
		public int _j;
		public Node _prev = null;
	}

	public static readonly byte WALKABLE_CELL = 0;
	public static readonly byte OBSTACLE_CELL = 1;

	private byte[] _grid;
	private SpawnPoint _spawnArea;
	private Vector3 _mainTarget;
	private SwarmController _swarm;

	private Dictionary<byte, int> _towerPositions;

	public Pathfinder (SpawnPoint spawnArea, Vector3 mainTarget)
	{
		_swarm = SwarmController.getInstance ();
		_spawnArea = spawnArea;
		_grid = _swarm._obstacleGrid;
		_mainTarget = mainTarget;

		_towerPositions = new Dictionary<byte, int> ();
	}

	public PathSolution computePaths ()
	{

		// 1 - Build the path from Spawn to Final target
		int initialSpawnPoint = findBestSpawnPoint ();

		PathSolution ps = new PathSolution (initialSpawnPoint);

		Node fromStartToEnd = findPath (initialSpawnPoint, _mainTarget, true);
		Stack<int> mainPath = buildSolution (fromStartToEnd);
		ps.setMainPath (mainPath);

		int width = World.getInstance ().getWorldWidth ();

		// 2 - Loop over towers in the path from the spawn to the target
		foreach (byte towerSpawn in _towerPositions.Keys) {
			
			// 2.1 Compute path from spawn to tower X
			int index = _towerPositions [towerSpawn];
			Vector3 worldPos = _swarm._positions [index];
			Node endNode = findPath (initialSpawnPoint, worldPos, false);

			int lastValidIndex = endNode._i * width + endNode._j;

			Stack<int> pathToTower = buildSolution (endNode);
			ps.setTowerPath (towerSpawn, pathToTower);

			// 2.2 Create custom solution from tower X itself
			PathSolution towerSolution = new PathSolution (lastValidIndex);

			// 2.2.1 Compute path from tower X to main target
			endNode = findPath (lastValidIndex, _mainTarget, false);
			towerSolution.setMainPath (buildSolution (endNode));

			float distanceFromTowerToTarget = (_mainTarget - worldPos).sqrMagnitude;

			// 2.2.2 Compute path from tower X to the rest of towers
			foreach (byte innerTowerSpawn in _towerPositions.Keys) {
				if (innerTowerSpawn == towerSpawn)
					continue;

				// Do NOT add a tower which is not in the way to the target
				Vector3 nextTowerPosition = _swarm._positions [_towerPositions [innerTowerSpawn]];
				if ((_mainTarget - nextTowerPosition).sqrMagnitude >= distanceFromTowerToTarget)
					continue;

				Node solutionForTower = findPath (lastValidIndex, nextTowerPosition, false);
				towerSolution.setTowerPath (innerTowerSpawn, buildSolution (solutionForTower));
			}

			// 2.3 Set custom tower solution within main solution
			ps.setTowerCustomSolution (towerSpawn, towerSolution);
		}

		return ps;
	}

	private int findBestSpawnPoint ()
	{
		Vector3 avg = _spawnArea.getAveragePosition ();

		Dictionary<int, Vector3> spawnPoints = _spawnArea.getBounds ();

		int smallestIndex = 0;
		float smallestDeltaMagnitude = Mathf.Infinity;

		foreach (int vIndex in spawnPoints.Keys) {
			Vector3 pos = spawnPoints [vIndex];

			float posMag = (pos - avg).sqrMagnitude;
			if (posMag < smallestDeltaMagnitude) {
				smallestDeltaMagnitude = posMag;
				smallestIndex = vIndex;
			}
		}

		return smallestIndex;
	}

	private Node findPath (int initialPosition, Vector3 target, bool gatherTowers)
	{

		Node solutionPath = null;

		Vector3 currentDirection = target - _swarm._positions [initialPosition];
		float distance = currentDirection.magnitude;

		int width = World.getInstance ().getWorldWidth ();

		Node root = new Node ();
		root._i = initialPosition / width;
		root._j = initialPosition % width;
		root._prev = null;

		FastPriorityQueue<Node> queue = new FastPriorityQueue<Node> (_swarm.getMaxPossiblePositions ());
		queue.Enqueue (root, 0);

		bool solution = false;

		bool[] evaluated = new bool[width * width];
		evaluated [initialPosition] = true;

		float minDistanceToTarget = Constants.VERTEX_SPACING + (Constants.VERTEX_SPACING * 0.5f);
		while (!solution && queue.Count > 0) {
			Node next = queue.Dequeue ();

			int currentIndex = next._i * width + next._j;

			Vector3 currentPosition = _swarm._positions [currentIndex];
			float currentDistanceToTarget = (target - currentPosition).magnitude;

			if (currentDistanceToTarget <= minDistanceToTarget) {
				solutionPath = next;
				solution = true;
				break;
			}

			int istart = Mathf.Max (next._i - 1, 0);
			int jstart = Mathf.Max (next._j - 1, 0);
			int iend = Mathf.Min (next._i + 2, width);
			int jend = Mathf.Min (next._j + 2, width);

			for (int i = istart; i < iend; i++) {

				for (int j = jstart; j < jend; j++) {

					int tempIndex = i * width + j;

					if (evaluated [tempIndex])
						continue;

					evaluated [tempIndex] = true;

					if (_grid [tempIndex] == WALKABLE_CELL) {
						Vector3 possiblePos = _swarm._positions [tempIndex];

						Vector3 direction = target - possiblePos;

						int nodeIStart = Mathf.Max (i - 1, 0);
						int nodeJStart = Mathf.Max (j - 1, 0);
						int nodeIEnd = Mathf.Min (i + 2, width);
						int nodeJEnd = Mathf.Min (j + 2, width);

						int badNeighbours = 0;
						for (int k = nodeIStart; k < nodeIEnd; k++) {
							for (int z = nodeJStart; z < nodeJEnd; z++) {
								
								byte neighbourValue = _grid [k * width + z];
								if (neighbourValue == OBSTACLE_CELL)
									badNeighbours++;
								else if (gatherTowers && neighbourValue != WALKABLE_CELL && !_towerPositions.ContainsKey (neighbourValue)) {
									_towerPositions.Add (neighbourValue, k * width + z);
								}
							}
						}

						Node newNode = new Node ();
						newNode._i = i;
						newNode._j = j;
						newNode._prev = next;
						queue.Enqueue (newNode, direction.magnitude + (badNeighbours * 10));
					}
				}
			}
		}

		return solutionPath;
	}

	private Stack<int> buildSolution (Node solution)
	{
		Stack<int> solutionPath = new Stack<int> ();
		int mapWidth = World.getInstance ().getWorldWidth ();
		Node temp = solution;
		int i = 0;
		while (temp != null) {
			if (i % 2 == 0 || temp._prev == null) {
				solutionPath.Push (temp._i * mapWidth + temp._j);
			}
			i++;
			temp = temp._prev;
		}

		return solutionPath;
	}
}
