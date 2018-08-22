using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySystem : MonoBehaviour {

	[SerializeField] PlayArea playArea;

	public void Initialize (Player[] players) {
		ActivatableGenericEnemyContainer[] agec = gameObject.GetComponentsInChildren<ActivatableGenericEnemyContainer>(true);
		foreach(ActivatableGenericEnemyContainer container in agec){
			container.Initialize(players, playArea);
		}
		LevelReset();
	}

	public void LevelReset () {
		ActivatableContainer[] ac = gameObject.GetComponentsInChildren<ActivatableContainer>(true);
		foreach(ActivatableContainer container in ac){
			container.gameObject.SetActive(true);
			container.LevelReset();
		}
	}
}
