using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableGameObjectContainer : ActivatableContainer {

		[Header("Objects")]
	[SerializeField] GameObject[] objects;

	protected override void ResetContainer () {
		StopAllCoroutines();
		foreach(GameObject obj in objects){
			obj.SetActive(false);
		}
	}

	protected override void ActivateAllAtOnce () {
		foreach(GameObject obj in objects){
			obj.SetActive(true);
		}
	}

	protected override void ActivateStaggered (){
		StartCoroutine(StaggeredActivationCoroutine());
	}

	IEnumerator StaggeredActivationCoroutine () {
		for(int i=0; i<objects.Length; i++){
			objects[i].SetActive(true);
			yield return new WaitForSeconds(staggeredActivationInterval);
		}
	}

}
