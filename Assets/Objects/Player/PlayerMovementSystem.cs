using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementSystem : MonoBehaviour {

		[Header("Components")]
	[SerializeField] Rigidbody rb;

		[Header("Variables")]
	[SerializeField] float moveSpeedHorizontal;
	[SerializeField] float moveSpeedVertical;
	[SerializeField] float dodgeDistance;
	[SerializeField] float dodgeDuration;
	[SerializeField] float dodgeCooldown;
	[SerializeField] float dodgeLerpPower;

	[HideInInspector] public BoxCollider playAreaCollider;
	[HideInInspector] public PlayerInput playerInput;
	[HideInInspector] public PlayerModel playerModel;

	Vector3 dodgePos;
	Vector3 dodgeStartPos;
	Vector3 dodgeEndPos;
	float pointInDodge;
	bool canDodge;
	bool isDodging;
	bool wasDodging;
	float lastDodge;

	void Start(){
		Reset();
	}

	void Update(){
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		DodgeManager();
		Vector3 newLocalPos = transform.localPosition;
		Vector3 modelEulerAngles = Vector3.zero;
		if(!isDodging){
			Vector2 moveInput = playerInput.GetMoveInput();
			Vector3 movement = new Vector3(moveInput.x * moveSpeedHorizontal, 0f, moveInput.y * moveSpeedVertical) * Time.deltaTime;
			newLocalPos = transform.localPosition + movement;
			modelEulerAngles = new Vector3(0f, 0f, -moveInput.x * 45f);
		}else{
			newLocalPos = dodgePos;
			modelEulerAngles = new Vector3(0f, 0f, 360f * pointInDodge * Mathf.Sign((dodgeStartPos - dodgeEndPos).x));
		}
		Vector3 clipped;
		if(playAreaCollider != null){
			clipped = ClipToPlayArea(newLocalPos, playAreaCollider);
		}else{
			clipped = newLocalPos;
			Debug.LogWarning("warning, play area not set, movement not restricted for " + gameObject.name);
		}
		wasDodging = isDodging;
		transform.localPosition = Flatten(clipped);
		playerModel.SetLocalEulerAngles(modelEulerAngles);
	}

	public void Reset(){
		rb.useGravity = false;
		rb.velocity = Vector3.zero;
		rb.constraints = RigidbodyConstraints.FreezeRotation;
		rb.interpolation = RigidbodyInterpolation.None;
		canDodge = true;
		isDodging = false;
		wasDodging = false;
		lastDodge = Mathf.NegativeInfinity;
	}

	void DodgeManager(){
		if(!isDodging && (Time.time > (lastDodge + dodgeCooldown))){
			if(!canDodge){
				//TODO blip sound
				playerModel.Shine(Color.grey);
				canDodge = true;
			}
		}else{
			canDodge = false;
		}
		bool leftDodge = playerInput.GetLeftDodgeInputDown();
		bool rightDodge = playerInput.GetRightDodgeInputDown();
		if(canDodge && (leftDodge || rightDodge)){	
			lastDodge = Time.time;
			isDodging = true;
			dodgeStartPos = transform.localPosition;
			dodgeEndPos = transform.localPosition;								//because of the += you can (if you're skilled enough) dodge in place, getting the i-frames but not moving
			if(leftDodge) dodgeEndPos += (Vector3.left * dodgeDistance);
			if(rightDodge) dodgeEndPos += (Vector3.right * dodgeDistance);
		}
		if(isDodging){
			pointInDodge = (Time.time - lastDodge) / dodgeDuration;
			if(pointInDodge <= 1f){
				dodgePos = Vector3.Lerp(dodgeStartPos, dodgeEndPos, Mathf.Pow(pointInDodge, dodgeLerpPower));
			}else{
				isDodging = false;
				dodgePos = dodgeEndPos;
			}
		}
		if(isDodging != wasDodging){
			if(isDodging) gameObject.layer = LayerMask.NameToLayer("FriendlyDodging");
			else gameObject.layer = LayerMask.NameToLayer("Friendly");
		}
	}

	Vector3 ClipToPlayArea(Vector3 localPosition, BoxCollider playArea){
		/*
		if(box.size != Vector3.one || box.center != Vector3.zero) throw new UnityException("please use a default box (center (0,0,0), size (1,1,1))");	//should be better but wobbles at the edges
		Vector3 boxSpacePoint = box.transform.InverseTransformPoint(transform.TransformPoint(localPosition));
		Vector3 clampedBoxSpacePoint = new Vector3(Mathf.Clamp(boxSpacePoint.x, -1f, 1f), Mathf.Clamp(boxSpacePoint.y, -1f, 1f), Mathf.Clamp(boxSpacePoint.z, -1f, 1f));
		return transform.InverseTransformPoint(box.transform.TransformPoint(clampedBoxSpacePoint));
		*/
		if(playArea.transform.parent != this.transform.parent) throw new UnityException("play area's parent is different than player's parent");
		if(playArea.transform.localPosition != Vector3.zero) throw new UnityException("please position the play area at localPosition (0,0,0)");
		if(playArea.size != Vector3.one || playArea.center != Vector3.zero) throw new UnityException("please use a default boxcollider (center (0,0,0), size (1,1,1))");
		float maxX = playArea.transform.localScale.x / 2f;
		float maxY = playArea.transform.localScale.y / 2f;
		float maxZ = playArea.transform.localScale.z / 2f;
		return new Vector3(Mathf.Clamp(localPosition.x, -maxX, maxX), Mathf.Clamp(localPosition.y, -maxY, maxY), Mathf.Clamp(localPosition.z, -maxZ, maxZ));
	}

	Vector3 Flatten(Vector3 input){
		return new Vector3(input.x, 0f, input.z);
	}

}
