using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSystem : MonoBehaviour {

	[SerializeField] int maxComboMultiplier;
	[SerializeField] int rawPointsPerComboLevel;
	[SerializeField] float timeUntilComboReset;

	int recordedScore;
	int comboScore;
	int comboMultiplier;

	int rawComboPoints;
	float comboEndTime;

	public int RecordedScore { get { return recordedScore; } }
	public int ComboScore { get { return comboScore; } }
	public int ComboMultiplier { get { return comboMultiplier; } }
	public int TotalScore { get { return recordedScore + comboScore; } }

	void Update () {
		if((Time.time > comboEndTime)){
			recordedScore += comboScore;
			comboScore = 0;
			rawComboPoints = 0;
		}
	}

	public void AddScore (int points) {
		ComboCheck(points);
		comboScore += points * comboMultiplier;
	}

	public void ResetScore () {
		recordedScore = 0;
		comboScore = 0;
		comboMultiplier = 0;
	}

	void ComboCheck (int points) {
		rawComboPoints += points;												// < 1x rppcl pts	: 1x
		int divided = rawComboPoints / rawPointsPerComboLevel;					// >= 1x rppcl pts	: 2x
		comboMultiplier = Mathf.Clamp(divided + 1, 1, maxComboMultiplier);		// >= 2x rppcl pts	: 3x etc...
	}

}
