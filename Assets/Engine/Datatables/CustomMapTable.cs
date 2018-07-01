using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class CustomMapTable
{
	 
	static class Singleton
	{
		public static readonly CustomMapTable INSTANCE = new CustomMapTable ();
	}

	public static CustomMapTable getInstance ()
	{
		return Singleton.INSTANCE;
	}

	public class CustomMapTextureData
	{
		public Texture2D _mapBlueprint;
		public Sprite _uiSprite;

		public CustomMapTextureData (Texture2D blueprint, Sprite uiImg)
		{
			_mapBlueprint = blueprint;
			_uiSprite = uiImg;
		}
	}

	private Dictionary<string, CustomMapTextureData> _customMaps;

	private CustomMapTable ()
	{
		_customMaps = new Dictionary<string, CustomMapTextureData> ();
	}

	public void loadCustomMaps ()
	{
		_customMaps.Clear ();

		if (!Directory.Exists ("CustomMaps")) {
			Directory.CreateDirectory ("CustomMaps");
			return;
		}

		string[] files = Directory.GetFiles ("CustomMaps");
		foreach (string f in files) {

			if (!f.EndsWith (".png"))
				continue;

			string filename = f.Split ('\\') [1];

			if (!readAndProcessDataFile (filename, f))
				continue;

			byte[] buf = File.ReadAllBytes (f);

			Texture2D texture = new Texture2D (64, 64, TextureFormat.RGBA32, false, false);
			texture.LoadImage (buf);

			Sprite mapSprite = Sprite.Create (texture, new Rect (0.0f, 0.0f, texture.width, texture.height), new Vector2 (0.5f, 0.5f), 100.0f);

			CustomMapTextureData cmtd = new CustomMapTextureData (texture, mapSprite);

			_customMaps.Add (filename, cmtd);
		}
	}

	private bool readAndProcessDataFile (string name, string originalName)
	{
		string[] lines = File.ReadAllLines (originalName + ".data.txt");

		if (lines == null || lines.Length == 0)
			return false;

		Dictionary<string, string> levelProperties = new Dictionary<string, string> ();
		foreach (string ln in lines) {
			string[] split = ln.Split ('=');
			if (split.Length < 2)
				continue;

			levelProperties.Add (split [0], split [1]);
		}

		return LevelManager.getInstance ().processCustomLevel (name, levelProperties);
	}

	public Texture2D getCustomMap (string name)
	{
		if (_customMaps.ContainsKey (name)) {
			return _customMaps [name]._mapBlueprint;
		}

		return null;
	}

	public Sprite getCustomMapSprite (string name)
	{
		if (_customMaps.ContainsKey (name)) {
			return _customMaps [name]._uiSprite;
		}

		return null;
	}

	public Dictionary<string, CustomMapTextureData> getAllCustomMaps ()
	{
		return _customMaps;
	}
}
