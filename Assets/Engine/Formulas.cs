using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */  
public sealed class Formulas
{

	public static Vector3 getPositionInMap (Vector3 virtualPos)
	{
		int lenght = World.getInstance ().getWorldWidth ();
		return getPositionInMap (virtualPos, lenght, lenght);
	}

	public static Vector3 getPositionInMap (Vector3 virtualPos, int terrainWidth, int terrainHeight)
	{

		float mapXCenter = (terrainHeight / 2.0f) * Constants.VERTEX_SPACING;
		float mapZCenter = (terrainWidth / 2.0f) * Constants.VERTEX_SPACING;

		Vector3 realPos = new Vector3 (mapXCenter, virtualPos.y, mapZCenter);
		realPos.x += (virtualPos.x - mapXCenter);
		realPos.z += (virtualPos.z - mapZCenter);
		realPos.y += Constants.Y_CORRECTION;

		return realPos;
	}

	public static Vector3 lerpVector (Vector3 a, Vector3 b, float alpha)
	{

		Vector3 result = new Vector3 ();
		result.x = a.x + (b.x - a.x) * alpha;
		result.y = a.y + (b.y - a.y) * alpha;
		result.z = a.z + (b.z - a.z) * alpha;

		return result;
	}
}
