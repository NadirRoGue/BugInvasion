using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */  
public sealed class CameraToTextureRenderer : MonoBehaviour {

	public string SavedFileName;
	public Camera Capturer;
	public RenderTexture Texture;

	void Start () {

		Capturer.targetTexture = Texture;
		Capturer.Render ();

		RenderTexture.active = Texture;
		Texture2D virtualPhoto = new Texture2D(Texture.width,Texture.height, TextureFormat.RGB24, false);
		virtualPhoto.ReadPixels(new Rect(0, 0, Texture.width,Texture.height), 0, 0);
		RenderTexture.active = null;

		byte [] bytes;
		bytes = virtualPhoto.EncodeToPNG();

		System.IO.File.WriteAllBytes ("./" + SavedFileName + ".png", bytes);
	}
	
	void Update () {
	}
}
