using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class FastForwardButton : ControlButton
{
	public float _timeScaleBoost;

	public static FastForwardButton INSTANCE {
		get;
		set;
	}

	public override void onAwake ()
	{
		if (INSTANCE == null) {
			INSTANCE = this;
		}
	}

	public override void buttonClicked ()
	{
		_timeScaleBoost = 2.0f;
		//_prevTimeScale = Time.timeScale;
		Time.timeScale *= _timeScaleBoost;
	}

	public override void buttonReleased ()
	{
		//Time.timeScale = _prevTimeScale;
		Time.timeScale /= _timeScaleBoost;
		_timeScaleBoost = 1.0f;
	}
}
