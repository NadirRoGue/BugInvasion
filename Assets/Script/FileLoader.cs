using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class FileLoader : MonoBehaviour {

	public RectTransform ScrollPane;

	void Awake() {
		CustomMapTable.getInstance ().loadCustomMaps ();
	}

	void Start () {
		Object uiPrefab = Resources.Load ("UI/Custom_Map_Panel");

		Dictionary<string, CustomMapTable.CustomMapTextureData> data = CustomMapTable.getInstance ().getAllCustomMaps ();
		float accumulatedPos = 0;
		foreach (string name in data.Keys) {
			CustomMapTable.CustomMapTextureData mapData = data [name];

			GameObject panel = GameObject.Instantiate (uiPrefab, ScrollPane) as GameObject;

			panel.transform.Find ("Image").GetComponent<Image> ().sprite = mapData._uiSprite;
			panel.transform.Find ("Text").GetComponent<Text> ().text = name;
			panel.transform.Find ("Button").gameObject.AddComponent<LevelSelection> ().Level = name;

			Vector3 pos = panel.transform.position;
			pos.y += accumulatedPos;
			panel.transform.position = pos;

			accumulatedPos += 162;
		}
	}
	
	void Update () {
	}
}
