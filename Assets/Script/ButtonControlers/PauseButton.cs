using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class PauseButton : ControlButton {

	public override void buttonClicked ()
	{
		Time.timeScale = 0.0f;
	}

	public override void buttonReleased ()
	{
		Time.timeScale = 1.0f * FastForwardButton.INSTANCE._timeScaleBoost;
	}
}
