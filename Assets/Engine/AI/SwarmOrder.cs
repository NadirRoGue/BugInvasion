using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmailcom
 */ 
public sealed class SwarmOrder
{

	private byte _targetSpawn;
	private Vector3 _target;
	private Queue<Vector3> _pathToTarget;

	public SwarmOrder (byte target, Vector3 targetPos, Queue<Vector3> path)
	{
		_targetSpawn = target;
		_target = targetPos;
		_pathToTarget = path;
	}

	public void recalculatePath (Vector3 currentPosition)
	{
		float sqrtDist = (currentPosition - _target).sqrMagnitude;
		Queue<Vector3> newPath = new Queue<Vector3> ();
		bool startCopying = false;
		while (_pathToTarget.Count > 0) {
			Vector3 nextPos = _pathToTarget.Dequeue ();

			if (!startCopying) {
				float nextDist = (_target - nextPos).sqrMagnitude;
				if (nextDist < sqrtDist) {
					newPath.Enqueue (nextPos);
					startCopying = true;
				}
			} else {
				newPath.Enqueue (nextPos);
			}
		}

		_pathToTarget.Clear ();
		_pathToTarget = newPath;
	}

	public Queue<Vector3> getPath ()
	{
		return _pathToTarget;
	}

	public byte getTargetId ()
	{
		return _targetSpawn;
	}

	public Vector3 nextPosition ()
	{
		return _pathToTarget.Dequeue ();
	}
}
