using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefManager{

	/*
	 * README:
	 *
	 * Convention : 
	 * > Use underscores to separate words
	 * > Always write lowercase (not even camelcase)
	 * > Boolean values are to be saved as ints (1=true and 0=false)
	 * 
	 */

	private static List<StringKeyValuePair> strings;
	private static List<FloatKeyValuePair> floats;
	private static List<IntKeyValuePair> ints;

	private static List<IPlayerPrefObserver> observers;

	static PlayerPrefManager(){
		strings = new List<StringKeyValuePair>();
		floats = new List<FloatKeyValuePair>();
		ints = new List<IntKeyValuePair>();
		observers = new List<IPlayerPrefObserver>();

		InitializePlayerControls();
		InitializeGameSettings();
	}

	public static void AddObserver(IPlayerPrefObserver observer){
		if(!observers.Contains(observer)) observers.Add(observer);
	}

	public static void MessageKeybindObservers(){
		foreach(IPlayerPrefObserver observer in observers){
			if(observer is IPlayerPrefKeybindObserver){
				((IPlayerPrefKeybindObserver)observer).NotifyKeybindsChanged();
			}
		}
	}

	public static void MessagePlayerSettingsObservers(){
		foreach(IPlayerPrefObserver observer in observers){
			if(observer is IPlayerPrefSettingsObserver){
				((IPlayerPrefSettingsObserver)observer).NotifyPlayerSettingsChanged();
			}
		}
	}

	private static void InitializeGameSettings(){
		ints.Add(new IntKeyValuePair("game_playercount", 2));
		strings.Add(new StringKeyValuePair("game_currentstage", LevelLoader.Stage.DEBUG.ToString()));
		strings.Add(new StringKeyValuePair("game_difficulty", GameDifficulty.DifficultyLevel.NORMAL.ToString()));
		strings.Add(new StringKeyValuePair("game_p1_input", PlayerInput.InputType.KEYBOARD.ToString()));
		strings.Add(new StringKeyValuePair("game_p2_input", PlayerInput.InputType.GAMEPAD1.ToString()));
		strings.Add(new StringKeyValuePair("game_p1_plane", Player.PlaneType.GRIFFON.ToString()));
		strings.Add(new StringKeyValuePair("game_p2_plane", Player.PlaneType.WASP.ToString()));

		floats.Add(new FloatKeyValuePair("game_screenshakemultiplier", 1f));
	}

	private static void InitializePlayerControls(){
		strings.Add(new StringKeyValuePair("key_move_up", "W"));
		strings.Add(new StringKeyValuePair("key_move_down", "S"));
		strings.Add(new StringKeyValuePair("key_move_left", "A"));
		strings.Add(new StringKeyValuePair("key_move_right", "D"));
		strings.Add(new StringKeyValuePair("key_dodge_left", "J"));
		strings.Add(new StringKeyValuePair("key_dodge_right", "L"));
		strings.Add(new StringKeyValuePair("key_fire", "Space"));
		strings.Add(new StringKeyValuePair("key_special", "K"));
		strings.Add(new StringKeyValuePair("key_pause", "Escape"));

		strings.Add(new StringKeyValuePair("axis_move_x", "Joystick#LX"));
		strings.Add(new StringKeyValuePair("axis_move_y", "Joystick#LY"));
		strings.Add(new StringKeyValuePair("button_dodge_left", "Joystick#Button4"));
		strings.Add(new StringKeyValuePair("button_dodge_right", "Joystick#Button5"));
		strings.Add(new StringKeyValuePair("button_fire", "Joystick#Button0"));
		strings.Add(new StringKeyValuePair("button_special", "Joystick#Button2"));
		strings.Add(new StringKeyValuePair("button_pause", "Joystick#Button7"));
	}

	public static void ResetKey(string key){
		if(HasKey(key)){
			PlayerPrefs.DeleteKey(key);
		}
		else throw new UnityException ("No such key is known to the PlayerPrefManager");
	}

	public static bool HasKey(string key){
		foreach(StringKeyValuePair p in strings){
			if(p.key.Equals(key)) return true;
		}
		foreach(FloatKeyValuePair p in floats){
			if(p.key.Equals(key)) return true;
		}
		foreach(IntKeyValuePair p in ints){
			if(p.key.Equals(key)) return true;
		}
		return false;
	}

	public static string GetString(string key){
		StringKeyValuePair pair = new StringKeyValuePair(null, null);
		foreach(StringKeyValuePair p in strings){
			if(p.key.Equals(key)){
				pair = p;
				break;
			}
		}
		if(pair.key == null) throw new UnityException("The key \"" + key + "\" is not registered as a STRING-key in the PlayerPrefManager");
		return PlayerPrefs.GetString(pair.key, pair.defaultValue);
	}

	public static float GetFloat(string key){
		FloatKeyValuePair pair = new FloatKeyValuePair(null, 0f);
		foreach(FloatKeyValuePair p in floats){
			if(p.key.Equals(key)){
				pair = p;
				break;
			}
		}
		if(pair.key == null) throw new UnityException("The key \"" + key + "\" is not registered as a FLOAT-key in the PlayerPrefManager");
		return PlayerPrefs.GetFloat(pair.key, pair.defaultValue);
	}

	public static int GetInt(string key){
		IntKeyValuePair pair = new IntKeyValuePair(null, 0);
		foreach(IntKeyValuePair p in ints){
			if(p.key.Equals(key)){
				pair = p;
				break;
			}
		}
		if(pair.key == null) throw new UnityException("The key \"" + key + "\" is not registered as a INTEGER-key in the PlayerPrefManager");
		return PlayerPrefs.GetInt(pair.key, pair.defaultValue);
	}

	public static void SetString(string key, string value){
		foreach(StringKeyValuePair p in strings){
			if(p.key.Equals(key)){
				PlayerPrefs.SetString(key, value);
				//Debug.LogWarning("attempt to set \"" + key + "\" to \"" + value + "\" in playerprefs");
				return;
			}
		}
		throw new UnityException("The key \"" + key + "\" is not registered as a STRING-key in the PlayerPrefManager");
	}

	public static void SetFloat(string key, float value){
		foreach(FloatKeyValuePair p in floats){
			if(p.key.Equals(key)){
				PlayerPrefs.SetFloat(key, value);
				//Debug.LogWarning("attempt to set \"" + key + "\" to \"" + value + "\" in playerprefs");
				return;
			}
		}
		throw new UnityException("The key \"" + key + "\" is not registered as a FLOAT-key in the PlayerPrefManager");
	}

	public static void SetInt(string key, int value){
		foreach(IntKeyValuePair p in ints){
			if(p.key.Equals(key)){
				PlayerPrefs.SetInt(key, value);
				//Debug.LogWarning("attempt to set \"" + key + "\" to \"" + value + "\" in playerprefs");
				return;
			}
		}
		throw new UnityException("The key \"" + key + "\" is not registered as a INTEGER-key in the PlayerPrefManager");
	}

}

struct StringKeyValuePair{
	public string key;
	public string defaultValue;
	public StringKeyValuePair(string key, string defaultValue){
		this.key = key;
		this.defaultValue = defaultValue;
	}
}

struct FloatKeyValuePair{
	public string key;
	public float defaultValue;
	public FloatKeyValuePair(string key, float defaultValue){
		this.key = key;
		this.defaultValue = defaultValue;
	}
}

struct IntKeyValuePair{
	public string key;
	public int defaultValue;
	public IntKeyValuePair(string key, int defaultValue){
		this.key = key;
		this.defaultValue = defaultValue;
	}
}
