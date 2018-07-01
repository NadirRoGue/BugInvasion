using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */  
public sealed class PickController : MonoBehaviour
{

	public Camera SourceCamera;
	public Canvas TowerPlacementMenu;
	public Canvas TowerManagementMenu;

	public byte pickedSpawn;

	public static PickController INSTANCE {
		get;
		set;
	}

	public static byte getPickedSpawn ()
	{
		if (INSTANCE != null)
			return INSTANCE.pickedSpawn;

		return 0;
	}

	public static void resetPickedSpawn ()
	{
		if (INSTANCE != null) {
			INSTANCE.pickedSpawn = 0;
		}
	}

	public static void hidePlacementMenu ()
	{
		if (INSTANCE != null)
			INSTANCE.TowerPlacementMenu.enabled = false;
	}

	void Awake ()
	{
		if (INSTANCE == null) {
			INSTANCE = this;
		}
	}

	void Start ()
	{
		
	}

	void Update ()
	{
		if (LevelManager.getInstance ().isGamePaused ())
			return;
		
		if (Input.GetMouseButtonDown (0)) {

			Vector3 mousePos = Input.mousePosition;
			Vector2 screenPos = normalizeScreenCoords (new Vector2 (mousePos.x, mousePos.y));

			Ray ray = SourceCamera.ScreenPointToRay (mousePos);

			RaycastHit hit;

			if (Physics.Raycast (ray, out hit)) {

				if (hit.transform == null)
					return;
				
				if (hit.transform.GetComponent<SpawnIndex> () != null) {

					TowerManagementMenu.GetComponent<TowerManagement> ().disable ();
					TowerManagementMenu.enabled = false;

					if (hit.transform.GetComponent<SpawnIndex> ()._spawnIndex == pickedSpawn)
						return;
						
					pickedSpawn = hit.transform.GetComponent<SpawnIndex> ()._spawnIndex;

					if (SpawnTable.getInstance ().isSpawnInUse (pickedSpawn)) {
						return;
					}

					TowerPlacementMenu.enabled = true;
					updateTowerPlacementMenu ();
					RectTransform panel = TowerPlacementMenu.transform.Find ("Tower_panel") as RectTransform;
					panel.anchoredPosition = new Vector2 (screenPos.x + panel.rect.size.x / 2, screenPos.y + panel.rect.size.y / 2);
				} else if (hit.transform.GetComponent<TowerController> () != null) {
					pickedSpawn = 0;
					TowerPlacementMenu.enabled = false;
					TowerManagementMenu.enabled = false;
					TowerManagementMenu.GetComponent<TowerManagement> ().disable ();
					TowerManagementMenu.GetComponent<TowerManagement> ()
						.initialize (hit.transform.GetComponent<TowerController> ().getTowerInstance ());
					TowerManagementMenu.enabled = true;
					RectTransform panel = TowerManagementMenu.transform.Find ("Tower_panel") as RectTransform;
					panel.anchoredPosition = new Vector2 (screenPos.x + panel.rect.size.x / 2, screenPos.y + panel.rect.size.y / 2);
				}
			} 
		} else if (Input.GetMouseButtonDown (1)) {
			pickedSpawn = 0;
			TowerPlacementMenu.enabled = false;
			TowerManagementMenu.GetComponent<TowerManagement> ().disable ();
			TowerManagementMenu.enabled = false;
		}
	}

	private void updateTowerPlacementMenu ()
	{
		Transform t = TowerPlacementMenu.transform.Find ("Tower_panel").Find ("Selection_panel");
		foreach (Transform sub in t) {
			if (sub.GetComponent<TowerSelection> () != null)
				sub.GetComponent<TowerSelection> ().initialize ();
		}
	}

	private Vector2 normalizeScreenCoords (Vector2 pos)
	{
		pos.x = pos.x - (Screen.currentResolution.width / 2);
		pos.y = pos.y - (Screen.currentResolution.height / 2);
		return pos;
	}
}
