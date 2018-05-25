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
	[SerializeField] PlayerDeathSystem playerDeathSystem;

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

	public int playerNumber{get{return _playerNumber;}}
	int _playerNumber;

	PlayerModel playerModel;

	public static PlaneType ParsePlaneType(string name){
		return (PlaneType)(System.Enum.Parse(typeof(PlaneType), name));
	}

	void Start(){
		
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.F) && playerNumber == 1){
			playerHealthSystem.WeaponDamage(0);
		}
		if(Input.GetKeyDown(KeyCode.G) && playerNumber == 1){
			playerHealthSystem.WeaponDamage(999);
		}
		if(Input.GetKeyDown(KeyCode.Alpha1) && playerNumber == 1){
			Time.timeScale = 1f;
		}
		if(Input.GetKeyDown(KeyCode.Alpha2) && playerNumber == 1){
			Time.timeScale = 0.1f;
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
		playerDeathSystem.playerModel = playerModel;
		PlayerSpecialWeapon playerSPW = SPWObject.GetComponent<PlayerSpecialWeapon>();
		playerWeaponSystem.specialWeapon = playerSPW;
	}

	public void LevelResetInit(){
		hitbox.enabled = true;
		ResetAllComponents();
	}

	public void SetFurtherInitData(int playerNumber, BoxCollider playAreaCollider){
		this._playerNumber = playerNumber;
		playerMovementSystem.playAreaCollider = playAreaCollider;
	}

	public void SetRegularComponentsActive(bool value){
		playerMovementSystem.enabled = value;
		playerHealthSystem.enabled = value;
		playerWeaponSystem.enabled = value;
	}

	public void InitiateRespawn(){
		gameObject.layer = LayerMask.NameToLayer("FriendlyDodging");	//TODO other layer?
		hitbox.enabled = false;
		if(!playerDeathSystem.IsExploded()){
			playerDeathSystem.Explode(false);
		}
		SetRegularComponentsActive(false);
		ResetAllComponents();
		playerModel.SetBlinking(true);
	}

	public void FinishRespawn(){
		gameObject.layer = LayerMask.NameToLayer("Friendly");
		hitbox.enabled = true;
		SetRegularComponentsActive(true);
		playerHealthSystem.SetInvulnerableForSeconds(1f);	//HACK hardcoded. maybe find a better way
	}

	public void InitiateDeath(bool explode){
		gameObject.layer = LayerMask.NameToLayer("Background");
		SetRegularComponentsActive(false);
		if(explode){
			playerDeathSystem.Explode(true);
		}else{
			playerDeathSystem.Crash();
		}
		GameController.RequestRespawn(this);
	}

	void ResetAllComponents(){
		playerModel.Reset();
		playerMovementSystem.Reset();
		playerWeaponSystem.Reset();
		playerHealthSystem.Reset();
		playerDeathSystem.Reset();
	}

	GameObject InstantiatePrefabAsChild(GameObject prefab){
		GameObject instantiated = Instantiate(prefab, Vector3.zero, Quaternion.identity, this.transform) as GameObject;
		instantiated.transform.localPosition = Vector3.zero;
		instantiated.transform.localRotation = Quaternion.identity;
		return instantiated;
	}

}
