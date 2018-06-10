using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerInput{

	public enum InputType{
		KEYBOARD,
		GAMEPAD1,
		GAMEPAD2,
		DUAL
	}

	public static InputType ParseInputType(string name){
		return (InputType)(System.Enum.Parse(typeof(InputType), name));
	}

	public static PlayerInput Get(InputType inputType){
		switch(inputType){
		case InputType.KEYBOARD:
			return new PlayerKeyboardInput();
		case InputType.GAMEPAD1:
			return new PlayerGamepadInput(1);
		case InputType.GAMEPAD2:
			return new PlayerGamepadInput(2);
		case InputType.DUAL:
			return new PlayerDualInput();
		default:
			throw new UnityException("unknown input type");
		}
	}

	public abstract Vector2 GetMoveInput();

	public abstract bool GetLeftDodgeInputDown();

	public abstract bool GetRightDodgeInputDown();

	public abstract bool GetFireInput();

	public abstract bool GetSpecialInputDown();

	public abstract bool GetPauseInputDown();

}
