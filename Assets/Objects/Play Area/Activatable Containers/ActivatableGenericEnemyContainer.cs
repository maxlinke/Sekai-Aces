using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableGenericEnemyContainer : ActivatableContainer {

		[Header("Settings")]
	[SerializeField] GameplayMode mode;
	[SerializeField] GenericEnemy[] enemies;

	Vector3[] initialPositions;
	Quaternion[] initialRotations;

	public void Initialize (Player[] players) {
		initialPositions = new Vector3[enemies.Length];
		initialRotations = new Quaternion[enemies.Length];
		for(int i=0; i<enemies.Length; i++){
			initialPositions[i] = enemies[i].transform.position;
			initialRotations[i] = enemies[i].transform.rotation;
			enemies[i].Initialize(players, mode);
		}
	}

	public override void LevelReset () {
		StopAllCoroutines();
		for(int i=0; i<enemies.Length; i++){
			enemies[i].LevelReset();
			enemies[i].gameObject.SetActive(false);
			enemies[i].transform.position = initialPositions[i];
			enemies[i].transform.rotation = initialRotations[i];
		}
	}

	protected override void ActivateAllAtOnce () {
		foreach(GenericEnemy enemy in enemies){
			enemy.gameObject.SetActive(true);
		}
	}

	protected override void ActivateStaggered (){
		StartCoroutine(StaggeredActivationCoroutine());
	}

	IEnumerator StaggeredActivationCoroutine () {
		for(int i=0; i<enemies.Length; i++){
			enemies[i].gameObject.SetActive(true);
			yield return new WaitForSeconds(staggeredActivationInterval);
		}
	}

}
