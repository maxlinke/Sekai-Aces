using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreGUI : MonoBehaviour {

	//TODO playerprefsobserver, observe highscore changes and reload gui and stuff
	//TODO potentially jsonified highscore-object that holds the three top highscores

		[Header("Components")]
	[SerializeField] Text comboScoreText;
	[SerializeField] Text comboMultiplierText;
	[SerializeField] Text recordedScoreText;
	[SerializeField] Text highScoresText;

		[Header("Settings")]
	[SerializeField] int minNumberOfFiguresPerScore;

	StageManager.Stage currentStage;
	ScoreSystem scoreSystem;

//	void Start () {
//		scoreSystem = ScoreSystem.Instance;	//TODO eeeeeeeeehhhhhhhhhh, this could also be set via script
//		//even for enemies, they could be "initialized" with the score system, which the enemyspawner should know
//	}

	public void Initialize (ScoreSystem scoreSystem, StageManager.Stage currentStage){
		this.scoreSystem = scoreSystem;
		this.currentStage = currentStage;
	}
	
	void Update () {
		if(Time.time > 0f){	//if not paused...
			comboScoreText.text = StringifyScore(scoreSystem.ComboScore);
			comboMultiplierText.text = scoreSystem.ComboMultiplier + "x";
			recordedScoreText.text = StringifyScore(scoreSystem.RecordedScore);
		}
	}

	string StringifyScore (int score) {
		string scoreString = score.ToString();
		int difference = minNumberOfFiguresPerScore - scoreString.Length;
		for(int i=0; i<difference; i++){
			scoreString = "0" + scoreString;
		}
		return scoreString;
	}

	void ReloadHighscores () {
		int high1 = PlayerPrefManager.GetInt("highscore_1_" + currentStage.ToString());
		int high2 = PlayerPrefManager.GetInt("highscore_2_" + currentStage.ToString());
		int high3 = PlayerPrefManager.GetInt("highscore_3_" + currentStage.ToString());
		Debug.Log("TODO");
		//TODO loading (and also saving) highscores
	}
}
