using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */  
public sealed class MissionLockController : MonoBehaviour {

	public void checkLocks() {
		Player p = World.getInstance ().getPlayer ();
		bool[] unlockedLevels = p.getUnlockedLevels ();
		bool[] completedLevels = p.getCompletedLevels ();
		int missionCount = unlockedLevels.Length;

		for (int i = 0; i < missionCount; i++) {

			string buttonWrapper = "Mission_" + (i + 1) + "_button";
			Transform button = transform.Find (buttonWrapper);

			if (button != null) {
				Transform lockImage = button.Find ("Lock");
				if (lockImage != null && GetComponent<Image>() != null) {
					GetComponent<Image> ().enabled = !unlockedLevels [i];
				}

				Transform tickImage = button.Find ("Completed");
				if (tickImage != null && GetComponent<Image> () != null) {
					GetComponent<Image> ().enabled = completedLevels [i];
				}
			}
		}
	}

	// Use this for initialization
	void Start () {
		checkLocks ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
