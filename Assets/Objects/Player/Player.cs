using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public enum PlaneType{
		SPECTRE,
		WASP,
		GRIFFON,
		RAZORBACK,
		ARROWHEAD
	}

		[Header("Components")]
	[SerializeField] Collider hitbox;
	[SerializeField] PlayerMovementSystem playerMovementSystem;
	[SerializeField] PlayerWeaponSystem playerWeaponSystem;
	[SerializeField] PlayerHealthSystem playerHealthSystem;

		[Header("Plane Prefabs")]
	[SerializeField] GameObject spectrePlanePrefab;
	[SerializeField] GameObject waspPlanePrefab;
	[SerializeField] GameObject griffonPlanePrefab;
	[SerializeField] GameObject razorbackPlanePrefab;
	[SerializeField] GameObject arrowheadPlanePrefab;

		[Header("SPW Prefabs")]
	[SerializeField] GameObject spectreSPWPrefab;
	[SerializeField] GameObject waspSPWPrefab;
	[SerializeField] GameObject griffonSPWPrefab;
	[SerializeField] GameObject razorbackSPWPrefab;
	[SerializeField] GameObject arrowheadSPWPrefab;

		[Header("Debug Stuff")]
	[SerializeField] bool selfInitialize;
	[SerializeField] PlayerInput.InputType presetInputType;
	[SerializeField] Player.PlaneType presetPlaneType;
	[SerializeField] int presetPlayerNumber;
	[SerializeField] GameplayMode presetMode;


	public int PlayerNumber{get{return _playerNumber;}}
	public GameplayMode Mode{set{playerMovementSystem.Mode = value;playerWeaponSystem.Mode = value;}}
	public bool IsDead{get{return playerHealthSystem.IsDead();}}
	public bool IsRespawning{get{return isRespawning;}}
	public bool IsGameover{get{return (IsDead && (lives == 0));}}

	int lives;
	int _playerNumber;
	bool isRespawning;
	PlayerModel playerModel;
	GameController gameController;
	PlayerGUI gui;

	void Awake(){
		if(selfInitialize){
			Debug.Log("selfinit");
			Initialize(presetInputType, presetPlaneType);
			SetFurtherInitData(presetPlayerNumber, null, null);
			playerMovementSystem.Mode = presetMode;
		}
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.F) && PlayerNumber == 1){
			playerHealthSystem.WeaponDamage(0);
		}
		if(Input.GetKeyDown(KeyCode.G) && PlayerNumber == 1){
			playerHealthSystem.WeaponDamage(999);
		}
	}

	public void Initialize(PlayerInput.InputType inputType, PlaneType planeType){
		PlayerInput playerInput = PlayerInput.Get(inputType);
		playerMovementSystem.playerInput = playerInput;
		playerWeaponSystem.playerInput = playerInput;
		GameObject modelObject;
		GameObject SPWObject;
		switch(planeType){
		case PlaneType.SPECTRE:
			modelObject = InstantiatePrefabAsChild(spectrePlanePrefab);
			SPWObject = InstantiatePrefabAsChild(spectreSPWPrefab);
			break;
		case PlaneType.WASP:
			modelObject = InstantiatePrefabAsChild(waspPlanePrefab);
			SPWObject = InstantiatePrefabAsChild(waspSPWPrefab);
			break;
		case PlaneType.GRIFFON:
			modelObject = InstantiatePrefabAsChild(griffonPlanePrefab);
			SPWObject = InstantiatePrefabAsChild(griffonSPWPrefab);
			break;
		case PlaneType.RAZORBACK:
			modelObject = InstantiatePrefabAsChild(razorbackPlanePrefab);
			SPWObject = InstantiatePrefabAsChild(razorbackSPWPrefab);
			break;
		case PlaneType.ARROWHEAD:
			modelObject = InstantiatePrefabAsChild(arrowheadPlanePrefab);
			SPWObject = InstantiatePrefabAsChild(arrowheadSPWPrefab);
			break;
		default:
			throw new UnityException("unknown plane type");
		}
		playerModel = modelObject.GetComponent<PlayerModel>();
		playerMovementSystem.playerModel = playerModel;
		playerWeaponSystem.playerModel = playerModel;
		playerHealthSystem.playerModel = playerModel;
		PlayerSpecialWeapon playerSPW = SPWObject.GetComponent<PlayerSpecialWeapon>();
		playerWeaponSystem.specialWeapon = playerSPW;
	}

	public void SetInvulnerableForSeconds(float seconds){
		playerHealthSystem.SetInvulnerableForSeconds(seconds);
	}

	public void LevelResetInit(int maxLives){
		lives = maxLives;
		gui.SetLivesNumber(lives);
		gui.SetLivesDisplayState(true);
		isRespawning = false;
		hitbox.enabled = true;
		gameObject.layer = LayerMask.NameToLayer("Friendly");
		ResetAllComponents();
	}

	public void SetFurtherInitData(int playerNumber, GameController gameController, PlayerGUI gui){
		this._playerNumber = playerNumber;
		this.gameController = gameController;
		this.gui = gui;	//TODO weapon gui = gui, movement gui = gui
		playerMovementSystem.gui = gui;
		playerWeaponSystem.gui = gui;
	}

	public void SetRegularComponentsActive(bool value){
		playerMovementSystem.enabled = value;
		playerHealthSystem.enabled = value;
		playerWeaponSystem.enabled = value;
	}

	public void InitiateRespawn(){
		ResetAllComponents();
		SetRegularComponentsActive(false);
		playerModel.SetBlinking(true);
		gameObject.layer = LayerMask.NameToLayer("FriendlyRespawning");
		hitbox.enabled = false;
		isRespawning = true;
		lives--;
		gui.SetLivesNumber(lives);
		gui.SetLivesDisplayState(true);
	}

	public void FinalizeRespawn(){
		SetRegularComponentsActive(true);
		hitbox.enabled = true;
		isRespawning = false;
	}

	public void FinishRespawn(){
		gameObject.layer = LayerMask.NameToLayer("Friendly");
		playerModel.SetBlinking(false);
	}

	public void InitiateDeath(bool explode){
		SetRegularComponentsActive(false);

		gui.SetLivesDisplayState(false);
		gui.SetDodgeDisplayState(false);
		gui.SetSPWDisplayState(false);

		playerModel.Hide();
		if(lives > 0){
			gameController.RequestRespawn(this);
		}else{
			gameController.NotifyGameover(this);
		}
	}

	void ResetAllComponents(){
		playerModel.RespawnReset();
		playerMovementSystem.RespawnReset();
		playerWeaponSystem.RespawnReset();
		playerHealthSystem.RespawnReset();
	}

	GameObject InstantiatePrefabAsChild(GameObject prefab){
		GameObject instantiated = Instantiate(prefab, Vector3.zero, Quaternion.identity, this.transform) as GameObject;
		instantiated.transform.localPosition = Vector3.zero;
		instantiated.transform.localRotation = Quaternion.identity;
		return instantiated;
	}

	public static PlaneType ParsePlaneType(string name){
		return (PlaneType)(System.Enum.Parse(typeof(PlaneType), name));
	}

}
