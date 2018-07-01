using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */  
public sealed class PathCollector : TextureObserver
{
	 
	private byte[] _grid;

	public override void init ()
	{
		_grid = new byte[_textureWidth * _textureHeight];
	}

	public override void processPixel (int x, int y, Color color)
	{
		byte colorValue = Pathfinder.OBSTACLE_CELL;
		if (color != Constants.TERRAIN_COLOR)
			colorValue = Pathfinder.WALKABLE_CELL;
		
		_grid [x * _textureWidth + y] = colorValue;
	}

	public byte[] getWalkableGrid ()
	{
		return _grid;
	}
}
