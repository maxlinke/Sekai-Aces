using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

	public enum Stage{
		DEBUG,
		FOREST
	}

	[SerializeField] string debugStage;
	[SerializeField] string forestStage;

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
		case(Stage.FOREST):
			sceneToLoad = forestStage;
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
