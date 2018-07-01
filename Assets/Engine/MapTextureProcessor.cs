using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class MapTextureProcessor
{

	private List<TextureObserver> _observers;

	public MapTextureProcessor ()
	{
		_observers = new List<TextureObserver> ();
	}

	public void registerObserver (TextureObserver observer)
	{
		_observers.Add (observer);
	}

	public void processTexture (Texture2D texture)
	{

		int width = texture.width;
		int height = texture.height;

		foreach (TextureObserver o in _observers) {
			o.preInit (width, height);
		}

		for (int i = 0; i < height; i++) {
			for (int j = 0; j < width; j++) {

				foreach (TextureObserver o in _observers) {
					o.processPixel (i, j, texture.GetPixel (i, j));
				}
			}
		}
	}
}
