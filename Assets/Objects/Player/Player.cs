﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public enum PlaneType{
		SPECTRE,
		WASP,
		GRIFFON,
		RAZORBACK,
	}

		[Header("Components")]
	[SerializeField] Collider hitbox;
	[SerializeField] Rigidbody rb;
	[SerializeField] PlayerMovementSystem playerMovementSystem;
	[SerializeField] PlayerWeaponSystem playerWeaponSystem;
	[SerializeField] PlayerHealthSystem playerHealthSystem;
	[SerializeField] PlayerDeathSystem playerDeathSystem;

		[Header("Plane Prefabs")]
	[SerializeField] GameObject spectrePlanePrefab;
	[SerializeField] GameObject waspPlanePrefab;
	[SerializeField] GameObject griffonPlanePrefab;
	[SerializeField] GameObject razorbackPlanePrefab;

		[Header("SPW Prefabs")]
	[SerializeField] GameObject spectreSPWPrefab;
	[SerializeField] GameObject waspSPWPrefab;
	[SerializeField] GameObject griffonSPWPrefab;
	[SerializeField] GameObject razorbackSPWPrefab;

	public Vector3 velocity { get { return rb.velocity;	} }
	public int PlayerNumber{get{return _playerNumber;}}
	public bool IsDead { get { return playerHealthSystem.IsDead(); } }
	public bool IsRespawning { get { return isRespawning; } }
	public bool IsGameover { get { return (IsDead && (lives == 0)); } }

	public GameplayMode Mode { set { 
			playerMovementSystem.Mode = value;
			playerWeaponSystem.Mode = value;
		}
	}


	int lives;
	int _playerNumber;
	bool isRespawning;
	PlayerModel playerModel;
	GameController gameController;
	PlayerGUI gui;
	PlayerInput playerInput;
	PlaneType planeType;

	void Start () {
		
	}

	void Update () {
		if(Time.timeScale > 0f){
			if(Input.GetKeyDown(KeyCode.F) && PlayerNumber == 1){
				playerHealthSystem.WeaponDamage(1);
			}
			if(Input.GetKeyDown(KeyCode.G) && PlayerNumber == 1){
				playerHealthSystem.WeaponDamage(999);
			}
			if(Input.GetKeyDown(KeyCode.H) && PlayerNumber == 1){
				ScoreSystem.Instance.AddScore(20);
			}
		}
		if(playerInput.GetPauseInputDown()){
			gameController.TogglePause();
		}
	}

	void OnCollisionEnter (Collision collision) {
		if(collision.gameObject.layer == LayerMask.NameToLayer("Obstacle")){
			if(playerHealthSystem.enabled) playerHealthSystem.ObstacleCollision();
		}
		IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
		if(damageable != null){
			playerWeaponSystem.DealCollisionDamage(damageable);
		}
	}

	void OnTriggerStay (Collider otherCollider) {
		if(otherCollider.gameObject.layer == LayerMask.NameToLayer("PowerUp")){
			PowerUp powerUp = otherCollider.GetComponent<PowerUp>();
			DealWithPowerup(powerUp);
		}
	}

	public void Initialize(PlayerInput.InputType inputType, PlaneType planeType){
		playerInput = PlayerInput.Get(inputType);
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
		default:
			throw new UnityException("unknown plane type");
		}
		this.planeType = planeType;
		playerModel = modelObject.GetComponent<PlayerModel>();
		playerMovementSystem.playerModel = playerModel;
		playerWeaponSystem.playerModel = playerModel;
		playerHealthSystem.playerModel = playerModel;
		playerDeathSystem.playerModel = playerModel;
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
		//gameObject.layer = LayerMask.NameToLayer("Friendly");
		SetLayerIncludingAllChildren(this.gameObject, LayerMask.NameToLayer("Friendly"));
		ResetAllComponents();
	}

	public void SetFurtherInitData(int playerNumber, GameController gameController, PlayerGUI gui, PlayArea playArea, TrackFollower levelTrackFollower){
		this._playerNumber = playerNumber;
		this.gameController = gameController;
		this.gui = gui;
		playerMovementSystem.gui = gui;
		playerWeaponSystem.gui = gui;
		gui.Initialize(planeType);
		playerMovementSystem.playArea = playArea;
		playerDeathSystem.playArea = playArea;
		playerDeathSystem.levelTrackFollower = levelTrackFollower;
	}

	public void SetRegularComponentsActive(bool value){
		playerMovementSystem.enabled = value;
		playerHealthSystem.enabled = value;
		playerWeaponSystem.enabled = value;
	}

	public void InitiateRespawn(){
		if(!playerDeathSystem.IsExploded()) playerDeathSystem.Explode();
		ResetAllComponents();
		SetRegularComponentsActive(false);
		playerModel.SetBlinking(true);
//		gameObject.layer = LayerMask.NameToLayer("FriendlyRespawning");
		SetLayerIncludingAllChildren(this.gameObject, LayerMask.NameToLayer("FriendlyRespawning"));
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
//		gameObject.layer = LayerMask.NameToLayer("Friendly");
		if(!IsDead){
			SetLayerIncludingAllChildren(this.gameObject, LayerMask.NameToLayer("Friendly"));
			playerModel.SetBlinking(false);
		}
	}

	public void InitiateDeath(bool explode){
		SetRegularComponentsActive(false);

		gui.SetLivesDisplayState(false);
		gui.SetDodgeDisplayState(false);
		gui.SetSPWDisplayState(false);

		if(explode){
			playerDeathSystem.Explode();
		}else{
			playerDeathSystem.InitiateCrash();
		}

		if(lives > 0){
			gameController.RequestRespawn(this);
		}else{
			gameController.NotifyGameover(this);
		}
	}

	void DealWithPowerup(PowerUp powerUp){
		PowerUp.PowerUpType powerUpType = powerUp.Type;
		//TODO shine in powerups color instead of custom
		switch(powerUpType){
		case PowerUp.PowerUpType.WEAPON:
			if(playerWeaponSystem.WeaponCanBeUpgraded()){
				playerWeaponSystem.UpgradeWeapon();
				powerUp.ReturnToPool();
			}
			break;
		case PowerUp.PowerUpType.RELOAD:
			if(playerWeaponSystem.SpecialWeaponCanBeReloaded()){
				playerWeaponSystem.ReloadSpecialWeapon();
				powerUp.ReturnToPool();
			}
			break;
		case PowerUp.PowerUpType.REPAIR:
			if(playerHealthSystem.CanBeHealed()){
				playerHealthSystem.Heal();
				powerUp.ReturnToPool();
			}
			break;
		default:
			throw new UnityException("Unknown PowerUp-Type \"" + powerUpType.ToString() + "\"");
		}
	}

	void ResetAllComponents(){
		playerModel.RespawnReset();
		playerMovementSystem.RespawnReset();
		playerWeaponSystem.RespawnReset();
		playerHealthSystem.RespawnReset();
		playerDeathSystem.RespawnReset();
	}

	public void SetLayerIncludingAllChildren(GameObject obj, int layer){
		obj.layer = layer;
		for(int i=0; i<obj.transform.childCount; i++){
			GameObject child = obj.transform.GetChild(i).gameObject;
			SetLayerIncludingAllChildren(child, layer);
		}
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
