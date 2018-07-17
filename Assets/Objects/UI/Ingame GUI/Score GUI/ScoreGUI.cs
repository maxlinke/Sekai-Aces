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
	[SerializeField] Image comboTimerBar;

		[Header("Settings")]
	[SerializeField] int minNumberOfFiguresPerScore;

	public Color color {
		set {
			scoreText.color = value;
			multiplierText.color = value;
			highscoreText.color = value;
			comboTimerBar.color = value;
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
			comboTimerBar.transform.localScale = new Vector3(scoreSystem.comboTimeLeft, 1f, 1f);
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

	void ReloadHighscore () {
		//only load the highest score and display that
		//for the endscreen check if it's a new highscore and stuff...
		//3 letter input thing.
		//if keyboard input (ugh, that's going to be a bitch to do) use the key, otherwise if it's a direction (via input manager, horizontal i guess ? )
//		int high1 = PlayerPrefManager.GetInt("highscore_1_" + currentStage.ToString());
//		int high2 = PlayerPrefManager.GetInt("highscore_2_" + currentStage.ToString());
//		int high3 = PlayerPrefManager.GetInt("highscore_3_" + currentStage.ToString());
		int topScore = PlayerPrefs.GetInt("test_highscore", 5000);
		highscoreText.text = "HI " + StringifyScore(topScore);
		//TODO loading (and also saving) highscores
	}

	public void LevelReset () {
		ReloadHighscore();
	}
}
