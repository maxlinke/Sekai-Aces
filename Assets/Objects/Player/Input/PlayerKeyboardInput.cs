using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeyboardInput : PlayerInput{

	KeyCode key_move_up;
	KeyCode key_move_down;
	KeyCode key_move_left;
	KeyCode key_move_right;

	KeyCode key_dodge_left;
	KeyCode key_dodge_right;
	KeyCode key_fire;
	KeyCode key_special;

	public PlayerKeyboardInput() : base(){
		ReloadControls();
	}

	void ReloadControls(){
		key_move_up = ParseKeyFromPlayerPrefs("key_move_up");
		key_move_down = ParseKeyFromPlayerPrefs("key_move_down");
		key_move_left = ParseKeyFromPlayerPrefs("key_move_left");
		key_move_right = ParseKeyFromPlayerPrefs("key_move_right");
		key_dodge_left = ParseKeyFromPlayerPrefs("key_dodge_left");
		key_dodge_right = ParseKeyFromPlayerPrefs("key_dodge_right");
		key_fire = ParseKeyFromPlayerPrefs("key_fire");
		key_special = ParseKeyFromPlayerPrefs("key_special");
	}

	KeyCode ParseKeyFromPlayerPrefs(string key){
		string keycode = PlayerPrefManager.GetString(key);
		return (KeyCode)System.Enum.Parse(typeof(KeyCode), keycode);
	}

	public override Vector2 GetMoveInput(){
		int xInput = 0;
		int yInput = 0;
		if(Input.GetKey(key_move_up)) yInput += 1;
		if(Input.GetKey(key_move_down)) yInput -= 1;
		if(Input.GetKey(key_move_left)) xInput -= 1;
		if(Input.GetKey(key_move_right)) xInput += 1;
		return new Vector2(xInput, yInput).normalized;
	}

	public override bool GetLeftDodgeInputDown(){
		return Input.GetKeyDown(key_dodge_left);
	}

	public override bool GetRightDodgeInputDown(){
		return Input.GetKeyDown(key_dodge_right);
	}

	public override bool GetFireInput(){
		return Input.GetKey(key_fire);
	}

	public override bool GetSpecialInputDown(){
		return Input.GetKeyDown(key_special);
	}

}
