using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameGUI : MonoBehaviour {

		[Header("Components")]
	[SerializeField] PlayerGUI p1GUI;
	[SerializeField] PlayerGUI p2GUI;
	[SerializeField] ScoreGUI scoreGUI;

		[Header("Settings")]
	[SerializeField] Color activeColor;
	[SerializeField] Color inactiveColor;

	void Awake () {
		p1GUI.activatedColor = activeColor;
		p1GUI.deactivatedColor = inactiveColor;
		p1GUI.gameObject.SetActive(false);
		p2GUI.activatedColor = activeColor;
		p2GUI.deactivatedColor = inactiveColor;
		p2GUI.gameObject.SetActive(false);
		scoreGUI.color = activeColor;
	}

	public PlayerGUI ActivateAndGetPlayerGUI(int playerNumber){
		switch(playerNumber){
		case 1:
			p1GUI.gameObject.SetActive(true);
			return p1GUI;
		case 2:
			p2GUI.gameObject.SetActive(true);
			return p2GUI;
		default:
			throw new UnityException("Unsupported playernumber \"" + playerNumber.ToString() + "\"");
		}
	}

	public void InitScoreGUI (ScoreSystem scoreSystem, LevelLoader.Stage currentStage) {
		scoreGUI.Initialize(scoreSystem, currentStage);
	}

	public void LevelReset () {
		scoreGUI.LevelReset();
	}

}
