using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

	public enum Stage{
		DEBUG,
		CITY
	}

	[SerializeField] string cityStage;
	[SerializeField] string debugStage;

	public static LevelLoader current;

	public static Stage ParseStage(string name){
		return (Stage)(System.Enum.Parse(typeof(Stage), name));
	}

	void Start(){
		current = this;
	}

	public void LoadStage(Stage stage){
		string sceneToLoad = null;
		switch(stage){
		case(Stage.CITY):
			Debug.Log("todo");
			break;
		case(Stage.DEBUG):
			sceneToLoad = debugStage;
			break;
		default:
			throw new UnityException("unknown stage (" + stage.ToString() + ")");
		}
		Time.timeScale = 1f;
		SceneManager.LoadScene(sceneToLoad);
	}

}
