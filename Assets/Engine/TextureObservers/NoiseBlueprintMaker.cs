using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class NoiseBlueprintMaker : TextureObserver
{
	private bool[] _blueprint;

	public override void init ()
	{
		_blueprint = new bool[_textureWidth * _textureHeight];
	}

	public override void processPixel (int x, int y, Color color)
	{
		int index = x * _textureWidth + y;
		_blueprint [index] = (color != Constants.TERRAIN_COLOR);
	}

	public bool[] getBlueprint ()
	{
		return _blueprint;
	}
}
