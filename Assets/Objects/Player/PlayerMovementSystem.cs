using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementSystem : MonoBehaviour {

		[Header("Components")]
	[SerializeField] Rigidbody rb;

		[Header("Variables")]
	[SerializeField] float moveSpeed;
	[SerializeField] float dodgeDistance;
	[SerializeField] float dodgeDuration;
	[SerializeField] float dodgeCooldown;
	[SerializeField] float dodgeLerpPower;

	[HideInInspector] public PlayerInput playerInput;
	[HideInInspector] public PlayerModel playerModel;
	[HideInInspector] public PlayerGUI gui;

	GameplayMode mode;
	public GameplayMode Mode {
		set {
			mode = value;
			UpdateRBConstraints();
		}
	}

	public PlayArea playArea {
		set {
			topDownMoveSpeed = moveSpeed * value.topAreaDimensions;
			sideMoveSpeed = moveSpeed * value.sideAreaDimensions;
			backMoveSpeed = moveSpeed * value.backAreaDimensions;
		}
	}


	Vector3 topDownMoveSpeed;
	Vector3 sideMoveSpeed;
	Vector3 backMoveSpeed;

	Vector3 dodgeVelocity;
	Vector3 dodgePos;
	Vector3 dodgeStartPos;
	Vector3 dodgeEndPos;
	float pointInDodge;
	bool canDodge;
	bool isDodging;
	bool wasDodging;
	float nextDodge;
	float dodgeStartTime;

	bool wantToDodgeLeft;
	bool wantToDodgeRight;

	void Start(){
		RespawnReset();
	}

	void Update(){
		if(Time.timeScale > 0f){
			wantToDodgeLeft |= playerInput.GetLeftDodgeInputDown();
			wantToDodgeRight |= playerInput.GetRightDodgeInputDown();
		}
	}

	void FixedUpdate(){
		DodgeManager();
		Vector3 newLocalPos = transform.localPosition;
		Vector3 modelEulerAngles = Vector3.zero;
		Vector3 velocity;
		if(!isDodging){
			Vector2 inputVector = playerInput.GetMoveInput();
			Vector3 transformedInput = TransformInput(inputVector);
			velocity = GetDesiredVelocity(transformedInput);
			modelEulerAngles = new Vector3(0f, 0f, -transformedInput.x * 45f);
		}else{
			velocity = dodgeVelocity;
			modelEulerAngles = new Vector3(0f, 0f, 360f * pointInDodge * Mathf.Sign((dodgeStartPos - dodgeEndPos).x));
		}
		wasDodging = isDodging;
		wantToDodgeLeft = false;
		wantToDodgeRight = false;
		playerModel.SetLocalEulerAngles(modelEulerAngles);
		rb.velocity = velocity;
	}

	void OnDisable(){
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		playerModel.SetLocalEulerAngles(Vector3.zero);
	}

	void OnEnable(){
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		if(playerModel != null) playerModel.SetLocalEulerAngles(Vector3.zero);
	}

	public void RespawnReset(){
		rb.useGravity = false;
		rb.velocity = Vector3.zero;
		rb.interpolation = RigidbodyInterpolation.Interpolate;
		transform.rotation = Quaternion.identity;
		UpdateRBConstraints();
		canDodge = true;
		isDodging = false;
		wasDodging = false;
		nextDodge = Mathf.NegativeInfinity;
		wantToDodgeLeft = false;
		wantToDodgeRight = false;
		gui.SetDodgeDisplayState(true);
	}

	void DodgeManager(){
		ManageAbilityToDodge();
		if(canDodge && (wantToDodgeLeft || wantToDodgeRight) && (gameObject.layer == LayerMask.NameToLayer("Friendly"))){	
			InitiateDodge();
		}
		if(isDodging){
			ManageDodgeVelocity();
		}
		if(isDodging != wasDodging){
			if(isDodging) gameObject.layer = LayerMask.NameToLayer("FriendlyDodging");
			else gameObject.layer = LayerMask.NameToLayer("Friendly");
		}
	}

	void ManageAbilityToDodge(){
		if(!isDodging && (Time.time > nextDodge)){
			if(!canDodge){
				playerModel.Shine(Color.grey);		//TODO blip sound
				gui.SetDodgeDisplayState(true);
				canDodge = true;
			}
		}else{
			canDodge = false;
		}
	}

	void InitiateDodge(){
		isDodging = true;
		dodgeStartTime = Time.time;
		nextDodge = Time.time + dodgeDuration + dodgeCooldown;
		dodgeStartPos = transform.localPosition;
		dodgeEndPos = transform.localPosition;
		//the += allows in-place dodging... high level tactics n stuff...
		if(wantToDodgeLeft) dodgeEndPos += (GetLeftDodgeVector() * dodgeDistance);
		if(wantToDodgeRight) dodgeEndPos -= (GetLeftDodgeVector() * dodgeDistance);

		gui.SetDodgeDisplayState(false);
	}

	void ManageDodgeVelocity(){
		pointInDodge = (Time.time - dodgeStartTime) / dodgeDuration;
		Vector3 startPos = transform.localPosition;
		if(pointInDodge <= 1f){
			dodgePos = Vector3.Lerp(dodgeStartPos, dodgeEndPos, Mathf.Pow(pointInDodge, dodgeLerpPower));
		}else{
			isDodging = false;
			dodgePos = dodgeEndPos;
		}
		dodgeVelocity = (dodgePos - startPos) / Time.fixedDeltaTime;
	}

	void UpdateRBConstraints(){
		rb.constraints = RigidbodyConstraints.FreezeRotation;
		rb.constraints |= GetMovementConstraints();
	}

	Vector3 GetLeftDodgeVector(){
		switch(mode){
		case GameplayMode.TOPDOWN:
			return Vector3.left;
		case GameplayMode.SIDE:
			return Vector3.up;
		case GameplayMode.BACK:
			return Vector3.left;
		default:
			throw new UnityException("Unknown GameplayMode \"" + mode.ToString() + "\"");
		}
	}

	Vector3 TransformInput(Vector2 input){
		switch(mode){
		case GameplayMode.TOPDOWN:
			return new Vector3(input.x, 0f, input.y);
		case GameplayMode.SIDE:
			return new Vector3(0f, input.y, input.x);
		case GameplayMode.BACK:
			return new Vector3(input.x, input.y, 0f);
		default:
			throw new UnityException("Unknown GameplayMode \"" + mode.ToString() + "\"");
		}
	}

	Vector3 GetDesiredVelocity(Vector3 transformedInput){
		switch(mode){
		case GameplayMode.TOPDOWN:
			return Vector3.Scale(transformedInput, topDownMoveSpeed);
		case GameplayMode.SIDE:
			return Vector3.Scale(transformedInput, sideMoveSpeed);
		case GameplayMode.BACK:
			return Vector3.Scale(transformedInput, backMoveSpeed);
		default:
			throw new UnityException("Unknown GameplayMode \"" + mode.ToString() + "\"");
		}
	}

	RigidbodyConstraints GetMovementConstraints(){
		switch(mode){
		case GameplayMode.TOPDOWN:
			return RigidbodyConstraints.FreezePositionY;
		case GameplayMode.SIDE:
			return RigidbodyConstraints.FreezePositionX;
		case GameplayMode.BACK:
			return RigidbodyConstraints.FreezePositionZ;
		default :
			throw new UnityException("Unknown GameplayMode \"" + mode.ToString() + "\"");
		}
	}

}
