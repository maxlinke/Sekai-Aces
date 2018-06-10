using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDualInput : PlayerInput{

	PlayerKeyboardInput keyboardInput;
	PlayerGamepadInput gamepadInput;

	public PlayerDualInput() : base(){
		keyboardInput = new PlayerKeyboardInput();
		gamepadInput = new PlayerGamepadInput(0);
	}

	public override Vector2 GetMoveInput(){
		Vector2 sum = keyboardInput.GetMoveInput() + gamepadInput.GetMoveInput();
		if(sum.magnitude > 1f){
			sum = sum.normalized;
		}
		return sum;
	}

	public override bool GetLeftDodgeInputDown(){
		return (keyboardInput.GetLeftDodgeInputDown() || gamepadInput.GetLeftDodgeInputDown());
	}

	public override bool GetRightDodgeInputDown(){
		return (keyboardInput.GetRightDodgeInputDown() || gamepadInput.GetRightDodgeInputDown());
	}

	public override bool GetFireInput(){
		return (keyboardInput.GetFireInput() || gamepadInput.GetFireInput());
	}

	public override bool GetSpecialInputDown(){
		return (keyboardInput.GetSpecialInputDown() || gamepadInput.GetSpecialInputDown());
	}

	public override bool GetPauseInputDown(){
		return (keyboardInput.GetPauseInputDown() || gamepadInput.GetPauseInputDown());
	}

}
