using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	static GameController instance;

		[Header("Prefabs")]
	[SerializeField] GameObject playerPrefab;
	[SerializeField] List<GameObject> bulletPoolPrefabs;

		[Header("Scene References")]
	[SerializeField] GameObject foreground;
	[SerializeField] BoxCollider playArea;
	[SerializeField] GameObject playerSpawn;
	[SerializeField] GameObject playerRespawn;

		[Header("Game Settings")]
	[SerializeField] float respawnDelay;
	[SerializeField] float respawnDuration;

	GameDifficulty.DifficultyLevel difficulty;
	Player[] players;
	int[] playerLives;

	int playerMaxLives;

	void Start(){
		if(instance != null) throw new UnityException("singleton violation");
		instance = this;

		LoadDifficulty();
		LoadBulletPools();
		LoadPlayers();
		playerLives = new int[players.Length];

		ResetLevel();
	}

	//TODO load coroutines for textures maybe? and only start game upon successfully finished all that stuff

	void Update(){
		
	}

	public void ResetLevel(){
		for(int i=0; i<players.Length; i++){
			Player pc = players[i];
			pc.LevelResetInit();
			pc.SetRegularComponentsActive(true);
			playerLives[i] = playerMaxLives;	//TODO update gui
		}
		//TODO reset all enemy stuff
		//reset the position of the foreground on the spline (reset the foreground script ?)
		//dont, i repeat DO NOT restart the music. that shit loops...
	}

	public static void RequestRespawn(Player pc){
		instance.RespawnRequested(pc);
	}

	void RespawnRequested(Player pc){
		int totalPlayerLives = GetTotalPlayerLives();
		if(totalPlayerLives == 0){
			Debug.Log("game over");
		}else{
			int playerIndex = pc.playerNumber - 1;
			if(playerLives[playerIndex] > 0){
				playerLives[playerIndex]--;
				StartCoroutine(RespawnPlayer(pc));
			}else{
				Debug.Log("player " + pc.playerNumber + " has no lives left");
			}
		}
	}

	int GetTotalPlayerLives(){
		int totalPlayerLives = 0;
		for(int i=0; i<playerLives.Length; i++){
			totalPlayerLives += playerLives[i];
		}
		return totalPlayerLives;
	}

	IEnumerator RespawnPlayer(Player pc){
		yield return new WaitForSeconds(respawnDelay);
		pc.gameObject.SetActive(true);
		pc.InitiateRespawn();
		pc.transform.parent = foreground.transform;
		pc.gameObject.transform.localPosition = playerRespawn.transform.localPosition;
		pc.gameObject.transform.localRotation = Quaternion.identity;
		float progress = 0f;
		float respawnStart = Time.time;
		while(progress < 1f){
			//pc.gameObject.transform.localPosition = (progress * playerSpawn.transform.localPosition) + ((1f - progress) * playerRespawn.transform.localPosition);
			//progress = ((Time.time - respawnStart) / respawnDuration);
			pc.gameObject.transform.localPosition = Vector3.Lerp(playerRespawn.transform.localPosition, playerSpawn.transform.localPosition, progress);
			progress += (Time.deltaTime / respawnDuration);
			yield return null;
		}
		pc.gameObject.transform.localPosition = playerSpawn.transform.localPosition;
		pc.FinishRespawn();
	}

	void LoadDifficulty(){
		difficulty = GameDifficulty.ParseGameDifficulty(PlayerPrefManager.GetString("game_difficulty"));
		playerMaxLives = GameDifficulty.GetPlayerLives(difficulty);
		Debug.Log(difficulty);
	}

	void LoadBulletPools(){
		foreach(GameObject bulletPoolPrefab in bulletPoolPrefabs){
			InstantiateToForeground(bulletPoolPrefab);
		}
	}

	void InstantiateToForeground(GameObject prefab){
		GameObject bulletPool = Instantiate(prefab) as GameObject;
		bulletPool.transform.parent = foreground.transform;
		bulletPool.transform.localPosition = Vector3.zero;
		bulletPool.transform.localRotation = Quaternion.identity;
	}

	void LoadPlayers(){
		int playerCount = PlayerPrefManager.GetInt("game_playercount");
		if(playerCount != 1 && playerCount != 2){
			throw new UnityException("unsupported playercount");
		}
		players = new Player[playerCount];
		for(int i=0; i<players.Length; i++){
			int playerNumber = i+1;
			players[i] = InstantiatePlayer(playerNumber);
			players[i].SetFurtherInitData(playerNumber, playArea);
		}
		if(playerCount == 2){
			players[0].transform.localPosition += Vector3.left;
			players[1].transform.localPosition += Vector3.right;
		}
	}



	Player InstantiatePlayer(int playerNumber){
		GameObject player = Instantiate(playerPrefab) as GameObject;
		player.transform.parent = foreground.transform;
		player.transform.localRotation = Quaternion.identity;
		player.transform.localPosition = playerSpawn.transform.localPosition;
		Player pc = player.GetComponent<Player>();
		char numberChar = playerNumber.ToString().ToCharArray()[0];
		string inputKey = "game_p#_input".Replace('#', numberChar);
		string planeKey = "game_p#_plane".Replace('#', numberChar);
		PlayerInput.InputType input = PlayerInput.ParseInputType(PlayerPrefManager.GetString(inputKey));
		Player.PlaneType plane = Player.ParsePlaneType(PlayerPrefManager.GetString(planeKey));
		pc.Initialize(input, plane);
		player.name = "Player " + playerNumber + " (" + plane.ToString() + ", " + input.ToString() + ")";
		return pc;
	}

}
