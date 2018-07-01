using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class TowerSelection : MonoBehaviour {

	public string TowerName;
	public Text PriceText;
	private int _price;

	void Start () {
		GetComponent<Button> ().onClick.AddListener (onClickEvt);
	}

	public void initialize() {
		int currentCredits = LevelManager.getInstance ().getCurrentCredits ();
		GameTower template = TowerTable.getInstance ().getTemplateByName (TowerName);

		if (template != null) {
			_price = template.getPrice ();
			Color color = PriceText.color;
			if (currentCredits < _price)
				color = Color.red;

			PriceText.color = color;
			PriceText.text = "" + template.getPrice ();
		} else {
			GetComponent<Button> ().enabled = false;
			PriceText.enabled = false;
		}
	}
	
	void Update () {
		
	}

	public void onClickEvt() {

		if (LevelManager.getInstance ().attemptToPay (_price)) {
			lock (PickController.INSTANCE) {

				PickController.INSTANCE.TowerPlacementMenu.enabled = false;

				byte spawn = PickController.getPickedSpawn ();
				PickController.resetPickedSpawn ();

				GameTowerInstance tower = TowerTable.getInstance ().instantiateTower (TowerName, spawn);
				SpawnPoint sp = SpawnTable.getInstance ().getTowerSpawns () [spawn];

				if (sp != null && SpawnTable.getInstance().tryToUseSpawn(spawn)) {
					SwarmController.getInstance ().notifyTowerSpawn (spawn);
		
					tower.getGameInstance ().AddComponent<TowerPlacementAnim> ().setParameters (
						Formulas.getPositionInMap (sp.getAveragePosition ())
					);
				} else {
					World.getInstance ().unregisterTower (tower);
				}
			}
		}
	}
}
