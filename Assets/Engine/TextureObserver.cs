using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public abstract class TextureObserver
{

	protected int _textureWidth;
	protected int _textureHeight;

	public TextureObserver ()
	{
	}

	public void preInit (int width, int height)
	{
		_textureWidth = width;
		_textureHeight = height;

		init ();
	}

	public abstract void init ();

	public abstract void processPixel (int x, int y, Color color);
}
