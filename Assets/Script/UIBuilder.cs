using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class UIBuilder : MonoBehaviour {

	private const float POS_FIX_ABS = 10.0f;

	public Canvas MainCanvas;

	public Text CoinText;
	public Text WaveText;
	public Text EnemiesText;

	public static UIBuilder INSTANCE {
		get;
		set;
	}

	void Awake() {
		if (INSTANCE == null)
			INSTANCE = this;
	}

	public void setCoinText(int coinAmount) {
		CoinText.text = "" + coinAmount;
	}

	public void setWaveText(int wave, int totalWaves) {
		WaveText.text = wave + " / " + totalWaves;
	}

	public void setEnemyArmyText(int army) {
		EnemiesText.text = "" + army;
	}

	// Use this for initialization
	void Start () {
		RectTransform mainPanel = transform.Find ("Main_panel") as RectTransform;

		if (mainPanel != null) {

			Vector2 mainSize = mainPanel.rect.size;
			float mainW = mainSize.x;
			float mainH = mainSize.y;

			RectTransform coinPanel = mainPanel.Find ("Credit_panel") as RectTransform;
			RectTransform wavePanel = mainPanel.Find ("Wave_panel") as RectTransform;
			RectTransform enemyPanel = mainPanel.Find ("Enemy_Count_panel") as RectTransform;


			float vOffset = 0.0f;
			adjustInfoPanel (mainW, mainH, coinPanel, vOffset);
			vOffset += coinPanel.rect.size.y;
			adjustInfoPanel (mainW, mainH, wavePanel, vOffset);
			vOffset += wavePanel.rect.size.y;
			adjustInfoPanel (mainW, mainH, enemyPanel, vOffset);

			RectTransform controlPanel = mainPanel.Find ("Controls_panel") as RectTransform;
			Vector2 controlSize = controlPanel.rect.size;
			controlPanel.anchoredPosition = new Vector2(0.0f, mainH / 2.0f - controlSize.y / 2.0f - POS_FIX_ABS);
		}
	}

	private void adjustInfoPanel(float parentWidth, float parentHeight, RectTransform target, float verticalOffset) {

		Vector2 targetSize = target.rect.size;

		float xPos = parentWidth / 2.0f - targetSize.x / 2.0f - POS_FIX_ABS;
		float yPos = parentHeight / 2.0f - targetSize.y / 2.0f - POS_FIX_ABS;

		yPos -= verticalOffset;

		target.anchoredPosition = new Vector2 (xPos, yPos);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
