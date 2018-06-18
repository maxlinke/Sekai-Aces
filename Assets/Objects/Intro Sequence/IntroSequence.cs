using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IntroSequence : MonoBehaviour {

	float skipDelay = 2f;	//TODO having this hardcoded here is kinda meh
	float skipTime;
	bool anyKeyDownLastFrame;

	[HideInInspector] public GameController gameController;

	void Start () {
		
	}
	
	void Update () {
		SkipCheck();
	}

	void SkipCheck(){
		if(IsRunning()){
			if(Input.anyKey){
				if(!anyKeyDownLastFrame){
					skipTime = Time.time + skipDelay;
					//TODO show the skip-message on the ui AND update the timer shown
				}
				if(Time.time > skipTime){
					Debug.Log("skip");
					AbortIntroSequence();
					gameController.ResetLevel();
				}
			}else{
				//TODO hide the skip message
			}
			anyKeyDownLastFrame = Input.anyKey;
		}
	}

	public abstract bool IsRunning();

	public abstract void StartIntroSequence();

	public abstract void AbortIntroSequence();

}
