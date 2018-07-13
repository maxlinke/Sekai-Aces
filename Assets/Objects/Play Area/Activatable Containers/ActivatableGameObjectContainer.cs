﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableGameObjectContainer : ActivatableContainer {

	GameObject[] objects;

	public override void LevelReset () {
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
