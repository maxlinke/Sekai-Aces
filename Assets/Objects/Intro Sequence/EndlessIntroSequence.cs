using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndlessIntroSequence : IntroSequence {

	[SerializeField] float introLength;
	[SerializeField] float levelResetLeadBeforeIntroEnd;
	[SerializeField] GameObject introUIContainer;
	[SerializeField] Image backgroundImage;
	[SerializeField] Text textObject;

	bool running;

	public override bool IsRunning (){
		return running;
	}

	public override void StartIntroSequence (){
		running = true;
		StartCoroutine(IntroCoroutine());
	}

	public override void AbortIntroSequence (){
		running = false;
		introUIContainer.SetActive(false);
		StopAllCoroutines();
	}

	IEnumerator IntroCoroutine(){
		introUIContainer.SetActive(true);
		backgroundImage.color = new Color(0,0,0,0.5f);
		float endTime = Time.time + introLength;
		float levelResetTime = endTime - levelResetLeadBeforeIntroEnd;
		bool levelReset = false;
		while(Time.time < endTime){
			float timeToGo = endTime - Time.time;
			textObject.text = string.Format("INTRO_{0:F3}", timeToGo);
			if(Time.time > levelResetTime && !levelReset){
				levelReset = true;
				backgroundImage.color = new Color(1,0,0,0.5f);
				gameController.ResetLevel();
				running = false;
			}
			yield return null;
		}
		if(!levelReset){
			levelReset = true;
			gameController.ResetLevel();
		}
		running = false;
		introUIContainer.SetActive(false);
	}

}
