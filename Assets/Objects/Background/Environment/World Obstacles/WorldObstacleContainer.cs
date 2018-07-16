using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObstacleContainer : MonoBehaviour {

	[SerializeField] GameObject[] resettableObjects;

	public void LevelReset () {
		for(int i=0; i<resettableObjects.Length; i++){
			resettableObjects[i].GetComponent<ILevelResetee>().LevelReset();
		}
	}

}
