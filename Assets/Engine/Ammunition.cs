using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class Ammunition
{
	 
	private string _loadPath;
	private Object _gameAsset;

	public float _launchSpeed {
		get;
		set;
	}

	public float _damageBoost = 1.0f;

	public Ammunition (string loadPath, float speed, float damageBoost = 1.0f)
	{
		_loadPath = loadPath;
		_launchSpeed = speed;
		_damageBoost = damageBoost;
		_gameAsset = Resources.Load ("Models/" + _loadPath);
	}

	public Object getGameAsset ()
	{
		return _gameAsset;
	}

	public GameObject instantiate ()
	{
		GameObject go = GameObject.Instantiate (_gameAsset, World.getInstance ().getTerrainTransform ()) as GameObject;
		go.AddComponent<AmmunitionController> ()._ammunition = this;
		go.AddComponent<SphereCollider> ().isTrigger = true;
		return go;
	}
}
