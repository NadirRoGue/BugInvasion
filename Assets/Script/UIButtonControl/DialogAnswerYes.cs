﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */  
public sealed class DialogAnswerYes : MonoBehaviour {

	public Canvas DialogCanvas;
	public Canvas MainCanvas;
	public Canvas MissionCanvas;

	// Use this for initialization
	void Start () {
		GetComponent<Button> ().onClick.AddListener (onClickEvt);	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void onClickEvt() {
		MainCanvas.enabled = false;
		DialogCanvas.enabled = false;

		Player p = World.getInstance ().getPlayer ();

		for (int i = 0; i < p.getUnlockedLevels ().Length; i++) {
			p.setLevelCompleted (i, false);
			p.setLevelUnlocked (i, false);
		}

		p.setLevelUnlocked (0, true);
		World.getInstance ().savePlayerData ();

		updateLocks(MissionCanvas.transform.Find("Mission_panel"));

		MissionCanvas.enabled = true;
	}

	private void updateLocks(Transform transformP) {
		Player p = World.getInstance ().getPlayer ();
		bool[] unlockedLevels = p.getUnlockedLevels ();
		bool[] completedLevels = p.getCompletedLevels ();
		int missionCount = unlockedLevels.Length;

		for (int i = 0; i < missionCount; i++) {

			string buttonWrapper = "Mission_" + (i + 1) + "_button";
			Transform button = transformP.Find (buttonWrapper);

			if (button != null) {
				Transform lockImage = button.Find ("Lock");

				if (lockImage != null && lockImage.GetComponent<Image>() != null) {
					lockImage.GetComponent<Image> ().enabled = !unlockedLevels [i];
				}

				Transform tickImage = button.Find ("Completed");
				if (tickImage != null && tickImage.GetComponent<Image> () != null) {
					tickImage.GetComponent<Image> ().enabled = completedLevels [i];
				}
			}
		}
	}
}
