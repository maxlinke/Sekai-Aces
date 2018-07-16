using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	[SerializeField] string mainMenuSceneName;

		[Header("Prefabs")]
	[SerializeField] GameObject playerPrefab;
	[SerializeField] List<GameObject> objectPoolPrefabs;

		[Header("Scene References")]
	[SerializeField] PlayArea playArea;
	[SerializeField] TrackFollower levelTrackFollower;
	[SerializeField] IngameGUI gui;
	[SerializeField] PauseMenu pauseMenu;
	[SerializeField] CameraShakeModule cameraShakeModule;
	[SerializeField] IntroSequence introSequence;
	[SerializeField] EnemySystem enemySystem;
	[SerializeField] ScoreSystem scoreSystem;
	[SerializeField] WorldObstacleContainer worldObstacleContainer;

		[Header("Game Settings")]
	[SerializeField] float levelResetPlayerControlDelay;
	[SerializeField] float gameplayModeTransitionDuration;
	[SerializeField] float respawnDelay;
	[SerializeField] float respawnDuration;
	[SerializeField] float afterRespawnGracePeriod;
	[SerializeField] float gameoverDelay;
	[SerializeField] float gameoverTimeFadeDuration;

		[Header("Level Settings")]
	[SerializeField] GameplayMode initialMode;

	LevelLoader.Stage currentStage;
	GameDifficulty.DifficultyLevel difficulty;
	List<ObjectPool> objectPools;
	Player[] players;
	float[] playerOffsets;
	float[] playerRespawnStartTimes;
	bool[] playerGameover;
	int playerMaxLives;

	void Start () {
		pauseMenu.gameController = this;
		introSequence.gameController = this;
		objectPools = new List<ObjectPool>();
		playArea.Initialize();
		LoadDifficulty();
		LoadCurrentStageName();
		LoadPools();
		LoadPlayers();
		enemySystem.Initialize(players);
		gui.InitScoreGUI(scoreSystem, currentStage);
		gui.gameObject.SetActive(false);
		pauseMenu.gameObject.SetActive(false);
		introSequence.StartIntroSequence();
		Debug.Log("TODO, prewarm pools");	//TODO prewarm pools
	}

	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1)) TransitionToGameplayMode(GameplayMode.TOPDOWN);
		if(Input.GetKeyDown(KeyCode.Alpha2)) TransitionToGameplayMode(GameplayMode.SIDE);
		if(Input.GetKeyDown(KeyCode.Alpha3)) TransitionToGameplayMode(GameplayMode.BACK);
	}

	public void ResetLevel () {
		StopAllCoroutines();
		Time.timeScale = 1f;

		scoreSystem.ResetScore();

		gui.gameObject.SetActive(true);		//TODO only enable the ui upon player control
		pauseMenu.gameObject.SetActive(false);
		cameraShakeModule.StopAllShaking();

		playArea.SetCamsToMode(initialMode);
		playArea.SetAreaToMode(initialMode);
		playArea.SetMode(initialMode);

		levelTrackFollower.LevelReset();

//		enemySpawner.LevelReset();
//		enemySpawner.SetMode(initialMode);

		enemySystem.LevelReset();
		worldObstacleContainer.LevelReset();

		foreach(ObjectPool pool in objectPools){
			pool.ResetPool();
		}

		for(int i=0; i<players.Length; i++){
			Player player = players[i];
			player.gameObject.SetActive(true);
			player.LevelResetInit(playerMaxLives);
			player.Mode = initialMode;
			player.transform.parent = playArea.transform;
			playArea.PutPlayerObjectToSpawnPoint(player.gameObject, playerOffsets[i]);
			playerRespawnStartTimes[i] = 0f;
			playerGameover[i] = false;
			player.enabled = false;
			player.SetRegularComponentsActive(false);

			StartCoroutine(playArea.TransitionPlayerWhileRespawning(player.gameObject, playerOffsets[i], levelResetPlayerControlDelay));
			StartCoroutine(WaitAndEnablePlayerControl(player, levelResetPlayerControlDelay));
		}

	}

	public void TransitionToTopdownMode () {
		TransitionToGameplayMode(GameplayMode.TOPDOWN);
	}

	public void TransitionToSideMode () {
		TransitionToGameplayMode(GameplayMode.SIDE);
	}

	public void TransitionToBackMode () {
		TransitionToGameplayMode(GameplayMode.BACK);
	}

	public void TogglePause () {
		bool totalGameover = GetTotalGameover();
		if(!totalGameover){
			bool paused = pauseMenu.gameObject.activeSelf;
			if(!paused){
				PauseGame();
			}else{
				UnpauseGame();
			}
		}
	}

	public void PauseGame () {
		Time.timeScale = 0f;
		pauseMenu.InitializeForPause();
		pauseMenu.gameObject.SetActive(true);
	}

	public void UnpauseGame () {
		Time.timeScale = 1f;
		pauseMenu.gameObject.SetActive(false);
	}

	public void ExitToMenu () {
		Time.timeScale = 1f;
		SceneManager.LoadScene(mainMenuSceneName);
	}

	public void TransitionToGameplayMode (GameplayMode newMode) {
//		enemySpawner.SetMode(newMode);
		StartCoroutine(WaitForRightConditionsAndTransition(newMode));
	}

	public void RequestRespawn (Player pc) {
		int playerIndex = pc.PlayerNumber - 1;
		playerRespawnStartTimes[playerIndex] = Time.time + respawnDelay;
		StartCoroutine(WaitAndRespawnPlayerCoroutine(pc));
	}

	public void NotifyGameover (Player pc) {
		int playerIndex = pc.PlayerNumber - 1;
		playerGameover[playerIndex] = true;
		bool totalGameover = GetTotalGameover();
		if(totalGameover){
			StartCoroutine(GameOverTimeFadeAndEnableMenu());
		}
	}

	bool GetTotalGameover () {
		bool totalGameover = true;
		for(int i=0; i<playerGameover.Length; i++){
			totalGameover &= playerGameover[i];
		}
		return totalGameover;
	}

	IEnumerator WaitAndEnablePlayerControl (Player pc, float delay) {
		yield return new WaitForSeconds(delay);
		pc.enabled = true;
		pc.SetRegularComponentsActive(true);
	}

	IEnumerator GameOverTimeFadeAndEnableMenu () {
		yield return new WaitForSeconds(gameoverDelay);
		float progress = 0f;
		float startTime = Time.unscaledTime;
		while(progress < 1f){
			progress = Mathf.Clamp01((Time.unscaledTime - startTime) / gameoverTimeFadeDuration);
			Time.timeScale = 1f - progress;
			yield return null;
		}
		Time.timeScale = 0f;
		pauseMenu.InitializeForGameover();
		pauseMenu.gameObject.SetActive(true);
		//TODO submit highscore
	}

	IEnumerator WaitForRightConditionsAndTransition (GameplayMode newMode) {
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

	IEnumerator WaitAndRespawnPlayerCoroutine (Player pc) {
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

	void LoadDifficulty () {
		difficulty = GameDifficulty.ParseGameDifficulty(PlayerPrefManager.GetString("game_difficulty"));
		playerMaxLives = GameDifficulty.GetPlayerLives(difficulty);
		Debug.Log(difficulty);
	}

	void LoadCurrentStageName () {
		currentStage = LevelLoader.ParseStage(PlayerPrefManager.GetString("game_currentstage"));
		Debug.Log(currentStage.ToString());
	}

	void LoadPools () {
		foreach(GameObject poolPrefab in objectPoolPrefabs){
			GameObject poolObject = InstantiateToPlayAreaAndGetObject(poolPrefab);
			ObjectPool pool = poolObject.GetComponent<ObjectPool>();
			pool.Initialize();
			objectPools.Add(pool);
		}
	}

	GameObject InstantiateToPlayAreaAndGetObject (GameObject prefab) {
		GameObject obj = Instantiate(prefab) as GameObject;
		obj.transform.parent = playArea.gameObject.transform;
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localRotation = Quaternion.identity;
		return obj;
	}

	void LoadPlayers () {
		int playerCount = PlayerPrefManager.GetInt("game_playercount");

		players = new Player[playerCount];
		playerOffsets = new float[playerCount];
		playerGameover = new bool[playerCount];
		playerRespawnStartTimes = new float[playerCount];

		if(playerCount == 1){
			playerOffsets[0] = 0;
		}else if(playerCount == 2){
			playerOffsets[0] = 1;
			playerOffsets[1] = -1;
		}else{
			throw new UnityException("unsupported playercount");
		}

		for(int i=0; i<players.Length; i++){
			int playerNumber = i+1;
			InstantiateAndInitializePlayer(playerNumber, out players[i]);
//			players[i].enabled = false;
//			players[i].SetRegularComponentsActive(false);
			players[i].gameObject.SetActive(false);
		}
	}

	void InstantiateAndInitializePlayer (int playerNumber, out Player player) {
		GameObject playerObject = Instantiate(playerPrefab) as GameObject;
		char numberChar = playerNumber.ToString().ToCharArray()[0];
		string inputKey = "game_p#_input".Replace('#', numberChar);
		string planeKey = "game_p#_plane".Replace('#', numberChar);
		PlayerInput.InputType input = PlayerInput.ParseInputType(PlayerPrefManager.GetString(inputKey));
		Player.PlaneType plane = Player.ParsePlaneType(PlayerPrefManager.GetString(planeKey));
		playerObject.name = "Player " + playerNumber + " (" + plane.ToString() + ", " + input.ToString() + ")";
		player = playerObject.GetComponent<Player>();
		player.Initialize(input, plane);
		player.SetFurtherInitData(playerNumber, this, gui.ActivateAndGetPlayerGUI(playerNumber), playArea, levelTrackFollower);
		player.transform.parent = playArea.transform;
	}

	bool RespawnIsInProgress () {
		for(int i=0; i<players.Length; i++){
			if(players[i].IsRespawning) return true;
		}
		return false;
	}

}
