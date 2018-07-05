using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

	[SerializeField] PlayArea playArea;

	GameplayMode mode;

	public void SetMode (GameplayMode newMode) {
		this.mode = newMode;
	}

	public void LevelReset () {
		//TODO reset all pools
	}

}
