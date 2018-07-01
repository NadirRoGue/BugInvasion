using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class StatsSet
{
	 
	private Dictionary<string, object> _stats;

	public StatsSet ()
	{
		_stats = new Dictionary<string, object> ();
	}

	public void set (string name, bool value)
	{
		if (_stats.ContainsKey (name)) {
			_stats [name] = value;
		} else {
			_stats.Add (name, value);
		}
	}

	public bool getBool (string name)
	{
		if (_stats.ContainsKey (name)) {
			return (bool)_stats [name];
		}

		return false;
	}

	public void set (string name, int value)
	{
		if (_stats.ContainsKey (name)) {
			_stats [name] = value;
		} else {
			_stats.Add (name, value);
		}
	}

	public int getInt (string name)
	{
		if (_stats.ContainsKey (name)) {
			return (int)_stats [name];
		}

		return 0;
	}

	public void set (string name, float value)
	{
		if (_stats.ContainsKey (name)) {
			_stats [name] = value;
		} else {
			_stats.Add (name, value);
		}
	}

	public float getFloat (string name)
	{
		if (_stats.ContainsKey (name)) {
			return (float)_stats [name];
		}

		return 0.0f;
	}

	public void set (string name, string value)
	{
		if (_stats.ContainsKey (name)) {
			_stats [name] = value;
		} else {
			_stats.Add (name, value);
		}
	}

	public string getString (string name)
	{
		if (_stats.ContainsKey (name)) {
			return (string)_stats [name];
		}

		return "";
	}

	public void set (string name, object obj)
	{
		if (_stats.ContainsKey (name)) {
			_stats [name] = obj;
		} else {
			_stats.Add (name, obj);
		}
	}

	public object getObject (string name)
	{
		if (_stats.ContainsKey (name)) {
			return _stats [name];
		}

		return null;
	}
}
