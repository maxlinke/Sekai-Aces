using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantaneousIntroSequence : IntroSequence {

	public override bool IsRunning (){
		return false;
	}

	public override void StartIntroSequence (){
		//nothing happens
	}

	public override void AbortIntroSequence (){
		//nothing happens
	}
}
