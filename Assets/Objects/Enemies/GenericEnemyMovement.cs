using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

public abstract class GenericEnemyMovement : MonoBehaviour, IEnemyComponent {

		[Header("Components")]
	[SerializeField] protected Rigidbody rb;

		[Header("General Movement Parameters")]
	[SerializeField] protected float initialMaxVelocity;
	[SerializeField] protected float initialMaxAcceleration;

	private float _maxVelocity;
	private float _maxAcceleration;

	protected float maxVelocity {
		get { 
			return _maxVelocity;
		}
		set { 
			if(Mathf.Abs(value) == Mathf.Infinity) throw new UnityException("Please don't use Infinity as a value, instead just use a large number.");
			else _maxVelocity = value; 
		}
	}

	protected float maxAcceleration {
		get { 
			return _maxAcceleration;
		}
		set { 
			if(Mathf.Abs(value) == Mathf.Infinity) throw new UnityException("Please don't use Infinity as a value, instead just use a large number.");
			else _maxAcceleration = value; 
		}
	}

	protected Player[] players;
	protected GameplayMode gameplayMode;
	protected PlayArea playArea;

	//IEnemyComponent

	public virtual void Initialize (Player[] players, GameplayMode gameplayMode, PlayArea playArea) {
		this.players = players;
		this.gameplayMode = gameplayMode;
		this.playArea = playArea;
	}

	public virtual void LevelReset () {
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;

		maxVelocity = initialMaxVelocity;
		maxAcceleration = initialMaxAcceleration;
	}

	//other public stuff

	public void SetTranslationConstraintsFromMode (GameplayMode mode) {
		rb.constraints = RigidbodyConstraints.FreezeRotation & rb.constraints;	//only rotation constraints remain
		rb.constraints |= GetMovementConstraints(mode);
	}

	//protected stuff for actual movement behaviors

	protected void Accelerate (Vector3 acceleration) {
		rb.velocity += acceleration * Time.fixedDeltaTime;
	}

	protected RigidbodyConstraints GetMovementConstraints (GameplayMode mode){
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

	protected void MoveInDirection (Vector3 direction) {
		Vector3 desiredVelocity = direction.normalized * maxVelocity;
		Vector3 deltaV = desiredVelocity - rb.velocity;
		Vector3 acceleration = deltaV.normalized * maxAcceleration;
		if ((acceleration.magnitude * Time.fixedDeltaTime) > deltaV.magnitude) {
			acceleration = deltaV / Time.fixedDeltaTime;
		}
		Accelerate(acceleration);
	}

	protected void MoveToPlayer (Player targetPlayer) {
		Vector3 playerVelocity = targetPlayer.velocity;
		Vector3 playerPos = targetPlayer.transform.position;
		Vector3 deltaPos = playerPos - this.transform.position;
		if (deltaPos.z > 0) {
			MoveInDirection(Vector3.back);
		}else{
			MoveInDirection(deltaPos);
		}
	}

	protected void MoveToNearestPlayer () {
		Player nearestPlayer = players[0];
		float lowestDistance = (players[0].transform.position - this.transform.position).magnitude;
		for (int i=1; i<players.Length; i++) {
			float distance = (players[i].transform.position - this.transform.position).magnitude;
			if (distance < lowestDistance) {
				lowestDistance = distance;
				nearestPlayer = players[i];
			}
		}
		MoveToPlayer(nearestPlayer);
	}

	protected void MoveAlongSpline (BezierSpline spline, ref float pos, float speed) {
		Vector3 nextPos = spline.MoveAlongSpline(ref pos, speed * Time.fixedDeltaTime);
		rb.MovePosition(nextPos);
	}

	protected void MoveAlongSpline (BezierSpline spline, ref float pos) {
		MoveAlongSpline(spline, ref pos, maxVelocity);
	}

}
