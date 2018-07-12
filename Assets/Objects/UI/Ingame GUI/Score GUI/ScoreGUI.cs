using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreGUI : MonoBehaviour {

	//TODO playerprefsobserver, observe highscore changes and reload gui and stuff
	//TODO potentially jsonified highscore-object that holds the three top highscores

		[Header("Components")]
	[SerializeField] Text scoreText;
	[SerializeField] Text multiplierText;
	[SerializeField] Text highscoreText;

		[Header("Settings")]
	[SerializeField] int minNumberOfFiguresPerScore;

	public Color color {
		set {
			scoreText.color = value;
			multiplierText.color = value;
			highscoreText.color = value;
		}
	}

	LevelLoader.Stage currentStage;
	ScoreSystem scoreSystem;

	public void Initialize (ScoreSystem scoreSystem, LevelLoader.Stage currentStage){
		this.scoreSystem = scoreSystem;
		this.currentStage = currentStage;
	}
	
	void Update () {
		if(Time.time > 0f){	//if not paused...
			scoreText.text = StringifyScore(scoreSystem.Score);
			multiplierText.text = scoreSystem.Multiplier + "x";
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
