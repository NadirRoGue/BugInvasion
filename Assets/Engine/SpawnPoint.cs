using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */  
public sealed class SpawnPoint
{

	private Dictionary<int, Vector3> _bounds = new Dictionary<int, Vector3> ();
	private bool _occupied = false;

	public SpawnPoint ()
	{
	}

	public void addBound (int index, Vector3 v)
	{
		_bounds.Add (index, v);
	}

	public Dictionary<int, Vector3> getBounds ()
	{
		return _bounds;
	}

	public Vector3 getAveragePosition ()
	{
		Vector3 avg = new Vector3 (0, 0, 0);
		foreach (Vector3 v in _bounds.Values) {
			avg += v;
		}

		avg /= _bounds.Count;

		return avg;
	}

	public bool isOccupied ()
	{
		return _occupied;
	}

	public void setOccupied (bool value)
	{
		_occupied = value;
	}
}
