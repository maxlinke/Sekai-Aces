using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDifficulty{

	public enum DifficultyLevel{
		EASY,
		NORMAL,
		HARD
	}

	const int lives_easy = 2;
	const int lives_normal = 1;
	const int lives_hard = 0;

	public static DifficultyLevel ParseGameDifficulty(string name){
		return (DifficultyLevel)(System.Enum.Parse(typeof(DifficultyLevel), name));
	}

	public static int GetPlayerLives(DifficultyLevel diff){
		switch(diff){
		case DifficultyLevel.EASY:
			return lives_easy;
		case DifficultyLevel.NORMAL:
			return lives_normal;
		case DifficultyLevel.HARD:
			return lives_hard;
		default:
			throw new UnityException("Unknown difficulty level (" + diff.ToString() + ")");
		}
	}

	//TODO enemy count and stuff

}
