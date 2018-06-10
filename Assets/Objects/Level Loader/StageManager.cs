using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class StageManager : MonoBehaviour {

	public enum Stage{
		CITY, 
		SKY, 
		ORBIT,
		ENDLESS
	}

	[SerializeField] SceneAsset cityStage;
	[SerializeField] SceneAsset skyStage;
	[SerializeField] SceneAsset orbitStage;
	[SerializeField] SceneAsset endlessStage;

	public static StageManager current;

	public static Stage ParseStage(string name){
		return (Stage)(System.Enum.Parse(typeof(Stage), name));
	}

	void Start(){
		current = this;
	}

	public void LoadStage(Stage stage){
		SceneAsset sceneToLoad = null;
		switch(stage){
		case(Stage.CITY):
			Debug.Log("todo");
			break;
		case(Stage.SKY):
			Debug.Log("todo");
			break;
		case(Stage.ORBIT):
			Debug.Log("todo");
			break;
		case(Stage.ENDLESS):
			sceneToLoad = endlessStage;
			break;
		default:
			throw new UnityException("unknown stage (" + stage.ToString() + ")");
		}
		Time.timeScale = 1f;
		SceneManager.LoadScene(sceneToLoad.name);
	}

}
