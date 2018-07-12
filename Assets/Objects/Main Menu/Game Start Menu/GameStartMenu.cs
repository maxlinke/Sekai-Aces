using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameStartMenu : MonoBehaviour {

		[Header("Singleplayer Components")]
	[SerializeField] GameObject singleplayerElementContainer;
	[SerializeField] GameObject singleplayerFirstSelected;
	[SerializeField] Text singleplayerPlaneText;

		[Header("Cooperative Components")]
	[SerializeField] GameObject cooperativeElementContainer;
	[SerializeField] GameObject cooperativeFirstSelected;
	[SerializeField] Text coopP1PlaneText;
	[SerializeField] Text coopP1InputText;
	[SerializeField] Text coopP2PlaneText;
	[SerializeField] Text coopP2InputText;

		[Header("Universal Components")]
	[SerializeField] Text difficultyText;
	[SerializeField] Text stageText;
	[SerializeField] Button startButton;
	[SerializeField] Text messageText;

		[Header("Sub Menus")]
	[SerializeField] GameObject planeSelectMenu;
	[SerializeField] GameObject levelSelectMenu;
	[SerializeField] GameObject difficultySelectMenu;
	[SerializeField] PlaneSelectRenderer planePreview;

	Player.PlaneType p1Plane;
	Player.PlaneType p2Plane;
	PlayerInput.InputType p1Input;
	PlayerInput.InputType p2Input;
	GameDifficulty.DifficultyLevel difficulty;
	LevelLoader.Stage stage;

	bool singleplayer;
	bool gameReady;
	EventSystem es;
	GameObject lastSelected;
	GameObject firstSelected;

	Player.PlaneType[] planes;
	PlayerInput.InputType[] inputs;
	LevelLoader.Stage[] stages;
	GameDifficulty.DifficultyLevel[] difficulties;

	//TODO
	//instead of the current style of menu do this
	//-

	void Awake(){
		p1Plane = Player.PlaneType.GRIFFON;
		p2Plane = Player.PlaneType.WASP;
		p1Input = PlayerInput.InputType.KEYBOARD;
		p2Input = PlayerInput.InputType.GAMEPAD1;
		difficulty = GameDifficulty.DifficultyLevel.NORMAL;
		stage = LevelLoader.Stage.DEBUG;
		SetVariablesToTextFields();
		planes = GetPlanes();
		inputs = GetInputs();
		stages = GetStages();
		difficulties = GetDifficulties();
	}
	
	void Update(){
		
	}

	void OnEnable(){
		es = EventSystem.current;
		SelectLast();
		UpdateGameReadiness();
		planePreview.ShowPlane(p1Plane, gameReady);
	}

	Player.PlaneType[] GetPlanes(){
		return (Player.PlaneType[])(System.Enum.GetValues(typeof(Player.PlaneType)));
	}

	PlayerInput.InputType[] GetInputs(){
		string[] gamepads = Input.GetJoystickNames();
		if(gamepads.Length > 1){
			return new PlayerInput.InputType[]{
				PlayerInput.InputType.KEYBOARD,
				PlayerInput.InputType.GAMEPAD1,
				PlayerInput.InputType.GAMEPAD2
			};
		}else if(gamepads.Length == 1){
			return new PlayerInput.InputType[]{
				PlayerInput.InputType.KEYBOARD,
				PlayerInput.InputType.GAMEPAD1,
			};
		}else{
			Debug.LogWarning("apparently no gamepads inserted (number : " + gamepads.Length + ")");
			return new PlayerInput.InputType[]{
				PlayerInput.InputType.KEYBOARD,
			};
		}
	}

	LevelLoader.Stage[] GetStages(){
		return (LevelLoader.Stage[])(System.Enum.GetValues(typeof(LevelLoader.Stage)));
	}

	GameDifficulty.DifficultyLevel[] GetDifficulties(){
		return (GameDifficulty.DifficultyLevel[])(System.Enum.GetValues(typeof(GameDifficulty.DifficultyLevel)));
	}

	public void StartGame(){
		string s;
		if(GetGameIsReady(out s)){
			if(singleplayer){
				PlayerPrefManager.SetInt("game_playercount", 1);
				PlayerPrefManager.SetString("game_p1_plane", p1Plane.ToString());
				PlayerPrefManager.SetString("game_p1_input", PlayerInput.InputType.DUAL.ToString());
			}else{
				PlayerPrefManager.SetInt("game_playercount", 2);
				PlayerPrefManager.SetString("game_p1_plane", p1Plane.ToString());
				PlayerPrefManager.SetString("game_p1_input", p1Input.ToString());
				PlayerPrefManager.SetString("game_p2_plane", p2Plane.ToString());
				PlayerPrefManager.SetString("game_p2_input", p2Input.ToString());
			}
			PlayerPrefManager.SetString("game_difficulty", difficulty.ToString());
			PlayerPrefManager.SetString("game_currentstage", stage.ToString());
			LevelLoader.current.LoadStage(stage);
		}else{
			Debug.LogError("this line shouldnt be reachable since the button should be disabled if the game cant be started");
		}
	}

	bool GetGameIsReady(out string message){
		message = "";
		gameReady = true;
		if(!singleplayer){
			if(p1Plane.Equals(p2Plane)){
				message = "Both players cannot choose the same plane";
				gameReady = false;
			}
			if(p1Input.Equals(p2Input)){
				message = "Both players cannot have the same input source";
				gameReady = false;
			}
		}
		return gameReady;
	}

	void UpdateGameReadiness(){
		string message;
		startButton.interactable = GetGameIsReady(out message);
		messageText.text = message;
	}

	void SetVariablesToTextFields(){
		difficultyText.text = difficulty.ToString();
		stageText.text = stage.ToString();
		singleplayerPlaneText.text = p1Plane.ToString();
		coopP1PlaneText.text = p1Plane.ToString();
		coopP2PlaneText.text = p2Plane.ToString();
		coopP1InputText.text = p1Input.ToString();
		coopP2InputText.text = p2Input.ToString();
	}

	void RememberLastSelected(){
		lastSelected = es.currentSelectedGameObject;
	}

	T NextFromArray<T>(T current, T[] array){
		int currentIndex = -1;
		for(int i=0; i<array.Length; i++){
			if(array[i].Equals(current)){
				currentIndex = i;
				break;
			}
		}
		int nextIndex = (currentIndex + 1) % array.Length;
		return array[nextIndex];
	}

	public void SelectLast(){
		if(lastSelected == null || (!lastSelected.activeSelf)){
			es.SetSelectedGameObject(firstSelected);
		}else{
			es.SetSelectedGameObject(lastSelected);
		}
	}

	public void InitializeForSingleplayer(){
		singleplayer = true;
		cooperativeElementContainer.SetActive(false);
		singleplayerElementContainer.SetActive(true);
		firstSelected = singleplayerFirstSelected;
	}

	public void InitializeForCooperative(){
		singleplayer = false;
		singleplayerElementContainer.SetActive(false);
		cooperativeElementContainer.SetActive(true);
		firstSelected = cooperativeFirstSelected;
	}

	public void CycleSPPlane(){
		p1Plane = NextFromArray<Player.PlaneType>(p1Plane, planes);
		singleplayerPlaneText.text = p1Plane.ToString();
		UpdateGameReadiness();
		planePreview.ShowPlane(p1Plane, gameReady);
	}

	public void CycleCPP1Plane(){
		p1Plane = NextFromArray<Player.PlaneType>(p1Plane, planes);
		coopP1PlaneText.text = p1Plane.ToString();
		UpdateGameReadiness();
		planePreview.ShowPlane(p1Plane, gameReady);
	}

	public void CycleCPP2Plane(){
		p2Plane = NextFromArray<Player.PlaneType>(p2Plane, planes);
		coopP2PlaneText.text = p2Plane.ToString();
		UpdateGameReadiness();
		planePreview.ShowPlane(p2Plane, gameReady);
	}

	public void CycleCPP1Input(){
		p1Input = NextFromArray<PlayerInput.InputType>(p1Input, inputs);
		coopP1InputText.text = p1Input.ToString();
		UpdateGameReadiness();
	}

	public void CycleCPP2Input(){
		p2Input = NextFromArray<PlayerInput.InputType>(p2Input, inputs);
		coopP2InputText.text = p2Input.ToString();
		UpdateGameReadiness();
	}

	public void CycleStage(){
		stage = NextFromArray<LevelLoader.Stage>(stage, stages);
		stageText.text = stage.ToString();
	}

	public void CycleDifficulty(){
		difficulty = NextFromArray<GameDifficulty.DifficultyLevel>(difficulty, difficulties);
		difficultyText.text = difficulty.ToString();
	}

}
