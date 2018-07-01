using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class ParticleSystemTable
{

	static class Singleton
	{
		public static readonly ParticleSystemTable INSTANCE = new ParticleSystemTable ();
	}

	public static ParticleSystemTable getInstance ()
	{
		return Singleton.INSTANCE;
	}

	private Dictionary<string, Object> _particleSystems;

	private ParticleSystemTable ()
	{
		_particleSystems = new Dictionary<string, Object> ();

		registerPS ("PS_Snow");
		registerPS ("PS_CannonBurst");
		registerPS ("PS_Collapse");
		registerPS ("PS_Sparks");
	}

	private void registerPS (string name)
	{
		Object ps = Resources.Load ("ParticleSystem/" + name);

		if (ps != null && !_particleSystems.ContainsKey (name)) {
			_particleSystems.Add (name, ps);
		} else {
			Debug.Log ("ParticleSystemTable: Duplicate particle system name " + name);
		}
	}

	public GameObject instantiateParticleSystem (string name)
	{
		return instantiateParticleSystem (name, World.getInstance ().getTerrainTransform ());
	}

	public GameObject instantiateParticleSystem (string name, Transform parent)
	{
		if (_particleSystems.ContainsKey (name)) {
			Object asset = _particleSystems [name];

			GameObject go = GameObject.Instantiate (asset, parent) as GameObject;
			return go;
		} 

		return null;
	}
}
