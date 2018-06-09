﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

		[Header("Prefabs")]
	[SerializeField] GameObject playerPrefab;
	[SerializeField] List<GameObject> bulletPoolPrefabs;

		[Header("Scene References")]
	[SerializeField] PlayArea playArea;
	[SerializeField] IngameGUI gui;

		[Header("Game Settings")]
	[SerializeField] float gameplayModeTransitionDuration;
	[SerializeField] float respawnDelay;
	[SerializeField] float respawnDuration;
	[SerializeField] float afterRespawnGracePeriod;
	[SerializeField] float gameoverTimeFadeDuration;

		[Header("Level Settings")]
	[SerializeField] GameplayMode initialMode;	//TODO respawn points for the different things... fuck... OR just say screw it and do the best with the existing ones (flatten x when in side mode)

	GameDifficulty.DifficultyLevel difficulty;
	Player[] players;
	float[] playerOffsets;
	float[] playerRespawnStartTimes;
	bool[] playerGameover;
	int playerMaxLives;

	void Start(){
		LoadDifficulty();
		LoadBulletPools();
		LoadPlayers();
		//TODO wait for intro animation and stuff... how do i deal with that? control it from here?
		ResetLevel();
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.Alpha1)) TransitionToGameplayMode(GameplayMode.TOPDOWN);
		if(Input.GetKeyDown(KeyCode.Alpha2)) TransitionToGameplayMode(GameplayMode.SIDE);
		if(Input.GetKeyDown(KeyCode.Alpha3)) TransitionToGameplayMode(GameplayMode.BACK);
		if(Input.GetKeyDown(KeyCode.Keypad1)) Time.timeScale = 1f;
		if(Input.GetKeyDown(KeyCode.Keypad2)) Time.timeScale = 0.1f;
		if(Input.GetKeyDown(KeyCode.R)) ResetLevel();
	}

	public void ResetLevel(){
		StopAllCoroutines();
		gui.gameObject.SetActive(true);
		Time.timeScale = 1f;

		playArea.SetCamsToMode(initialMode);
		playArea.SetAreaToMode(initialMode);
		playArea.SetMode(initialMode);

		for(int i=0; i<players.Length; i++){
			Player player = players[i];
			player.LevelResetInit(playerMaxLives);
			player.SetRegularComponentsActive(true);
			player.Mode = initialMode;
			player.transform.parent = playArea.transform;
			playArea.PutPlayerObjectToSpawnPoint(player.gameObject, playerOffsets[i]);
			playerRespawnStartTimes[i] = 0f;
			playerGameover[i] = false;
		}

		//TODO reset all enemy stuff
		//reset the position of the foreground on the spline (reset the foreground script ?)
		//dont, i repeat DO NOT restart the music. that shit loops...
	}

	//TODO check regularly whether game should be over... OR let the player controllers message the gamecontroller that they are out of lives and if both are, then its game over

	public void TransitionToGameplayMode(GameplayMode newMode){
		//TODO disable enemy spawns
		StartCoroutine(WaitForRightConditionsAndTransition(newMode));
	}

	public void RequestRespawn(Player pc){
		int playerIndex = pc.PlayerNumber - 1;
		playerRespawnStartTimes[playerIndex] = Time.time + respawnDelay;
		StartCoroutine(WaitAndRespawnPlayerCoroutine(pc));
	}

	public void NotifyGameover(Player pc){
		int playerIndex = pc.PlayerNumber - 1;
		playerGameover[playerIndex] = true;
		bool totalGameover = true;
		for(int i=0; i<playerGameover.Length; i++){
			totalGameover &= playerGameover[i];
		}
		if(totalGameover){
			Debug.Log("TOTAL GAME OVER!!!");
			StartCoroutine(GameOverTimeFadeAndEnableMenu());
		}
	}

	IEnumerator GameOverTimeFadeAndEnableMenu(){
		float progress = 0f;
		float startTime = Time.unscaledTime;
		while(progress < 1f){
			progress = Mathf.Clamp01((Time.unscaledTime - startTime) / gameoverTimeFadeDuration);
			Time.timeScale = 1f - progress;
			yield return null;
		}
		Time.timeScale = 0f;
		Debug.Log("DING, MENU OPENED NOW");
		//TODO open gameover menu (cannot be closed...)
	}

	IEnumerator WaitForRightConditionsAndTransition(GameplayMode newMode){
		bool respawnInProgress = RespawnIsInProgress();
		while(respawnInProgress){
			yield return null;
			respawnInProgress = RespawnIsInProgress();
		}
		StartCoroutine(playArea.TransitionCamerasToMode(newMode, gameplayModeTransitionDuration));
		for(int i=0; i<players.Length; i++){
			Player player = players[i];
			if(player.IsDead){
				playerRespawnStartTimes[i] = Mathf.Infinity;
			}else{
				player.SetRegularComponentsActive(false);
				StartCoroutine(playArea.TransitionPlayerToMode(player.gameObject, newMode, gameplayModeTransitionDuration));
			}
		}
		yield return new WaitForSeconds(gameplayModeTransitionDuration);
		for(int i=0; i<players.Length; i++){
			Player player = players[i];
			if(player.IsDead){
				playerRespawnStartTimes[i] = Time.time;
			}else{
				player.SetRegularComponentsActive(true);
			}
			player.Mode = newMode;
		}
		playArea.SetAreaToMode(newMode);
		playArea.SetMode(newMode);
	}

	IEnumerator WaitAndRespawnPlayerCoroutine(Player pc){
		int playerIndex = pc.PlayerNumber - 1;
		while(Time.time < playerRespawnStartTimes[playerIndex]){
			yield return null;
		}
		pc.transform.parent = playArea.gameObject.transform;
		pc.gameObject.SetActive(true);
		pc.InitiateRespawn();
		StartCoroutine(playArea.TransitionPlayerWhileRespawning(pc.gameObject, playerOffsets[playerIndex], respawnDuration));
		yield return new WaitForSeconds(respawnDuration);
		pc.FinalizeRespawn();
		yield return new WaitForSeconds(afterRespawnGracePeriod);
		pc.FinishRespawn();
	}

	void LoadDifficulty(){
		difficulty = GameDifficulty.ParseGameDifficulty(PlayerPrefManager.GetString("game_difficulty"));
		playerMaxLives = GameDifficulty.GetPlayerLives(difficulty);
		Debug.Log(difficulty);
	}

	void LoadBulletPools(){
		foreach(GameObject bulletPoolPrefab in bulletPoolPrefabs){
			InstantiateToPlayArea(bulletPoolPrefab);
		}
	}

	void InstantiateToPlayArea(GameObject prefab){
		GameObject obj = Instantiate(prefab) as GameObject;
		obj.transform.parent = playArea.gameObject.transform;
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localRotation = Quaternion.identity;
	}

	void LoadPlayers(){
		int playerCount = PlayerPrefManager.GetInt("game_playercount");

		players = new Player[playerCount];
		playerOffsets = new float[playerCount];
		playerGameover = new bool[playerCount];
		playerRespawnStartTimes = new float[playerCount];

		if(playerCount == 1){
			playerOffsets[0] = 0;
		}else if(playerCount == 2){
			playerOffsets[0] = -1;
			playerOffsets[1] = 1;
		}else{
			throw new UnityException("unsupported playercount");
		}

		for(int i=0; i<players.Length; i++){
			int playerNumber = i+1;
			InstantiateAndInitializePlayer(playerNumber, out players[i]);
		}
	}

	void InstantiateAndInitializePlayer(int playerNumber, out Player player){
		GameObject playerObject = Instantiate(playerPrefab) as GameObject;
		char numberChar = playerNumber.ToString().ToCharArray()[0];
		string inputKey = "game_p#_input".Replace('#', numberChar);
		string planeKey = "game_p#_plane".Replace('#', numberChar);
		PlayerInput.InputType input = PlayerInput.ParseInputType(PlayerPrefManager.GetString(inputKey));
		Player.PlaneType plane = Player.ParsePlaneType(PlayerPrefManager.GetString(planeKey));
		playerObject.name = "Player " + playerNumber + " (" + plane.ToString() + ", " + input.ToString() + ")";
		player = playerObject.GetComponent<Player>();
		player.Initialize(input, plane);
		player.SetFurtherInitData(playerNumber, this, gui.ActivateAndGetPlayerGUI(playerNumber));
		player.transform.parent = playArea.transform;
	}

	bool RespawnIsInProgress(){
		for(int i=0; i<players.Length; i++){
			if(players[i].IsRespawning) return true;
		}
		return false;
	}

}