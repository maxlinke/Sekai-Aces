using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledParticleEffect : MonoBehaviour {

	[SerializeField] ParticleSystem mainParticleSystem;
	[SerializeField] float checkFrequency;

	[HideInInspector] public ParticleEffectPool pool;

	public ParticleSystem MainParticleSystem { get { return mainParticleSystem; } }

	void OnEnable () {
		mainParticleSystem.Play(true);
		StartCoroutine(CheckForReturn());
	}

	void OnDisable () {
		StopAllCoroutines();
	}

	IEnumerator CheckForReturn () {
		while(true){
			yield return new WaitForSeconds(checkFrequency);
			if(!mainParticleSystem.IsAlive(true)){
				pool.ReturnToInactivePool(this);
				yield break;
			}
		}
	}

	public void Deactivate (bool clearEmissions = true) {
		ParticleSystemStopBehavior stopBehavior = (clearEmissions ? ParticleSystemStopBehavior.StopEmittingAndClear : ParticleSystemStopBehavior.StopEmitting);
		mainParticleSystem.Stop(true, stopBehavior);
	}

	public void SetLayerIncludingAllChildren (GameObject obj, int layer) {
		obj.layer = layer;
		for(int i=0; i<obj.transform.childCount; i++){
			GameObject child = obj.transform.GetChild(i).gameObject;
			SetLayerIncludingAllChildren(child, layer);
		}
	}

	public void ScaleIncludingAllChildren (GameObject obj, Vector3 localScale) {
		obj.transform.localScale = localScale;
		for(int i=0; i<obj.transform.childCount; i++){
			GameObject child = obj.transform.GetChild(i).gameObject;
			ScaleIncludingAllChildren(child, localScale);
		}
	}

}
