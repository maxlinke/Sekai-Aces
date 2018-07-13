using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySystem : MonoBehaviour {

	public void Initialize (Player[] players) {
		ActivatableGenericEnemyContainer[] agec = gameObject.GetComponentsInChildren<ActivatableGenericEnemyContainer>(true);
		foreach(ActivatableGenericEnemyContainer container in agec){
			container.Initialize(players);
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
