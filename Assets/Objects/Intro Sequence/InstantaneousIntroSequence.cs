using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantaneousIntroSequence : IntroSequence {

	public override bool IsRunning (){
		return false;
	}

	public override void StartIntroSequence (){
		gameController.ResetLevel();
	}

	public override void AbortIntroSequence (){
		gameController.ResetLevel();
	}
}
