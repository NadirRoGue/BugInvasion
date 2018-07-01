using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public abstract class CreatureTemplate
{

	protected string _name;
	protected string _resourceRelativePath;

	protected Object _assetSource;

	protected Bounds _bounds = new Bounds ();

	public CreatureTemplate (StatsSet set)
	{
		_name = set.getString ("name");
		_resourceRelativePath = set.getString ("relativeModelPath");

		_bounds.size = new Vector3 (0, 0, 0);

		_assetSource = Resources.Load ("Models/" + _resourceRelativePath);
	}

	public void setBounds (GameObject source)
	{
		if (_bounds.size.magnitude == 0.0f) {
			Renderer render = null;
			if ((render = source.transform.GetComponent<Renderer> ()) != null) {
				_bounds = render.bounds;
			} else {
				foreach (Transform t in source.transform) {
					if ((render = t.GetComponent<Renderer> ()) != null) {
						_bounds = render.bounds;
						break;
					}
				}
			}
		}
	}

	public Bounds getBounds ()
	{
		return _bounds;
	}

	public string getName ()
	{
		return _name;
	}

	public sealed override string ToString ()
	{
		return _name;
	}

	public string getRelativePath ()
	{
		return _resourceRelativePath;
	}

	public Object getGameAsset ()
	{
		return _assetSource;
	}
}
