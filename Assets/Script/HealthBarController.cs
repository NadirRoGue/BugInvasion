using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class HealthBarController : MonoBehaviour {

	public Creature _creature;
	private GameObject _instance;
	private RectTransform _panel;

	private float fullWidth;
	private float fullHeight;
	private float screenHalfWidth;
	private float screenHalfHeight;

	private float _lastUpdatedHealth;

	private bool _hidden = true;

	private float _yFix = 0.0f;

	void Start () {
		_instance = Instantiate (Resources.Load ("UI/Health_panel"), UIBuilder.INSTANCE.MainCanvas.transform) as GameObject;
		_panel = _instance.transform as RectTransform;

		Bounds b = _creature.getTemplate ().getBounds ();
		_yFix += b.max.y + 1.0f;

		fullWidth = Camera.main.pixelWidth;
		fullHeight = Camera.main.pixelHeight;
		screenHalfWidth = fullWidth / 2;
		screenHalfHeight = fullHeight / 2;

		_lastUpdatedHealth = _creature.getCurrentHealth ();

		_panel.GetComponent<Image> ().enabled = false;
		_panel.Find ("Image").GetComponent<Image>().enabled = false;
	}
	
	void Update () {

		Vector3 creaturePos = transform.position;
		creaturePos.y += _yFix;

		Vector3 viewportPos = Camera.main.WorldToViewportPoint (creaturePos);

		viewportPos.x = (viewportPos.x * fullWidth - screenHalfWidth);
		viewportPos.y = (viewportPos.y * fullHeight - screenHalfHeight);

		_panel.anchoredPosition = viewportPos;

		if (_creature.getCurrentHealth () != _lastUpdatedHealth) {

			if (_creature.getCurrentHealth () >= _creature.getMaxHealth ()) {
				_panel.GetComponent<Image> ().enabled = false;
				_panel.Find ("Image").GetComponent<Image> ().enabled = false;
				_hidden = true;
			} else {
				if (_hidden) {
					_panel.GetComponent<Image> ().enabled = true;
					_panel.Find ("Image").GetComponent<Image> ().enabled = true;
					_hidden = false;
				}

				_lastUpdatedHealth = _creature.getCurrentHealth ();
				Transform imageTrans = _panel.Find ("Image");
				Image img = imageTrans.GetComponent<Image> ();
				img.fillAmount = (_lastUpdatedHealth / _creature.getMaxHealth ());
			}
		}
	}

	public void destroy() {
		_panel.GetComponent<Image> ().enabled = false;
		_panel.Find ("Image").GetComponent<Image> ().enabled = false;
		Destroy (_instance);
		enabled = false;
	}
}
