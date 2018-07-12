using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSystem : MonoBehaviour {

	[SerializeField] int maxMultiplier;
	[SerializeField] int rawPointsPerComboLevel;
	[SerializeField] float timeUntilComboReset;

	int score;
	int multiplier;

	int rawComboPoints;
	float comboEndTime;

	public int Score { get { return score; } }
	public int Multiplier { get { return multiplier; } }

	void Update () {
		if((Time.time > comboEndTime)){
			rawComboPoints = 0;
		}
	}

	public void AddScore (int points) {
		ComboCheck(points);
		score += points * multiplier;
	}

	public void ResetScore () {
		score = 0;
		rawComboPoints = 0;
		multiplier = 1;
	}

	void ComboCheck (int points) {
		rawComboPoints += points;
		int level = rawComboPoints / rawPointsPerComboLevel;	//level 0 = 1x, level 1 = 2x, ...
		multiplier = Mathf.Clamp(level + 1, 1, maxMultiplier);
	}

}
