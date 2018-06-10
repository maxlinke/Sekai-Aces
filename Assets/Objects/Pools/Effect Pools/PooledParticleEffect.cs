using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledParticleEffect : MonoBehaviour {

	[SerializeField] ParticleSystem mainParticleSystem;
	[SerializeField] float checkFrequency;

	[HideInInspector] public ParticleEffectPool pool;

	void Start () {
		
	}
	
	void Update () {
		
	}

	void OnEnable(){
		mainParticleSystem.Play(true);
		StartCoroutine(CheckForReturn());
	}

	IEnumerator CheckForReturn(){
		while(true){
			yield return new WaitForSeconds(checkFrequency);
			if(!mainParticleSystem.IsAlive(true)){
				Deactivate();
				pool.ReturnToInactivePool(this);
			}
		}
	}

	public void Deactivate(){
		StopAllCoroutines();
		mainParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}

}
