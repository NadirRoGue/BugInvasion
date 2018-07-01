using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerManagement : MonoBehaviour {

	public static TowerManagement INSTANCE;

	public Image TowerIcon;
	public Text Tower;
	public Transform UpgradeButton;
	public Transform RepairButton;

	private bool _initialized = false;

	private GameTowerInstance _instance;

	private int _repairCost;

	void Awake() {
		if (INSTANCE == null)
			INSTANCE = this;
	}

	void Start () {
		if (INSTANCE == null)
			INSTANCE = this;
	}

	public void initialize(GameTowerInstance instance) {
		_instance = instance;

		if (!_initialized) {
			RepairButton.GetComponent<Button> ().enabled = true;
			RepairButton.GetComponent<Button> ().onClick.AddListener (repairButtonEvt);

			UpgradeButton.GetComponent<Button> ().enabled = true;
			UpgradeButton.GetComponent<Button> ().onClick.AddListener (upgradeButtonEvt);
			_initialized = true;
		}

		TowerIcon.GetComponent<Image> ().sprite = instance.getTower ().getTowerIcon ();
		Tower.text = instance.getTower ().getName ();

		_repairCost = 0;

		checkRepairButton ();
		checkUpgradeButton ();
	}

	private void checkRepairButton() {
		if (_instance == null)
			return;
		
		if (_instance.isBeingRepaired ())
			return;

		int currency = LevelManager.getInstance ().getCurrentCredits ();
		int newRepairCost = (int)(Mathf.Round(_instance.getTower ().getRepairCostPerPS () * (_instance.getMaxHealth () - _instance.getCurrentHealth ())));

		if (newRepairCost != _repairCost) {
			_repairCost = newRepairCost;
			RepairButton.Find ("Text").GetComponent<Text> ().text = "Reparar " + _repairCost;
			if (_repairCost == 0) {
				RepairButton.GetComponent<Button> ().enabled = false;
			} else if (_repairCost > currency) {
				RepairButton.GetComponent<Button> ().enabled = false;
				RepairButton.Find ("Text").GetComponent<Text> ().color = Color.red;
			} else {
				RepairButton.GetComponent<Button> ().enabled = true;
				RepairButton.Find ("Text").GetComponent<Text> ().color = Color.black;
			}
		}
	}

	private  void checkUpgradeButton() {
		if (_instance == null)
			return;
		
		if (_instance.canBeUpgraded ()) {

			int nextLevel = _instance.getCurrentUpgradeLvl () + 1;
			Upgrade u = UpgradeTable.getInstance ().getUpgrade (_instance.getTower ().getName (), nextLevel);
			int savings = LevelManager.getInstance ().getCurrentCredits ();

			if (savings >= u.getPriceForUpgrade ()) {
				UpgradeButton.GetComponent<Button> ().enabled = true;
				UpgradeButton.Find("Text").GetComponent<Text> ().color = Color.black;
			} else {
				UpgradeButton.GetComponent<Button> ().enabled = false;
				UpgradeButton.Find("Text").GetComponent<Text> ().color = Color.red;
			}

			UpgradeButton.Find("Text").GetComponent<Text> ().text = "Mejorar " + u.getPriceForUpgrade ();
		} else {
			UpgradeButton.GetComponent<Button> ().enabled = false;
			UpgradeButton.Find("Text").GetComponent<Text> ().text = "No hay mejoras";
		}
	}

	public void disable() {
		_instance = null;
	}
	
	void Update () {
		if (_instance != null) {
			checkRepairButton ();
		}
	}

	public void repairButtonEvt() {
		if (LevelManager.getInstance ().attemptToPay (_repairCost)) {
			RepairButton.GetComponent<Button> ().enabled = false;
			_instance.getGameInstance ().AddComponent<TowerRapair> ();
		}
	}

	public void upgradeButtonEvt() {
		if (_instance == null)
			return;
		
		if (_instance.canBeUpgraded ()) {
			int nextLevel = _instance.getCurrentUpgradeLvl () + 1;
			Upgrade u = UpgradeTable.getInstance ().getUpgrade (_instance.getTower ().getName (), nextLevel);

			if(LevelManager.getInstance().attemptToPay(u.getPriceForUpgrade())) {
				_instance.increaseUpgradeLvl ();
				checkUpgradeButton ();
			}
		}
	}
}
