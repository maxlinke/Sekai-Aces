using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericEnemyWeapon : MonoBehaviour, IEnemyComponent {

	protected Player[] players;
	protected GameplayMode gameplayMode;
	protected PlayArea playArea;

	//IEnemyComponent

	public virtual void Initialize (Player[] players, GameplayMode mode, PlayArea playArea) {
		this.players = players;
		this.gameplayMode = mode;
		this.playArea = playArea;
	}

	public virtual void LevelReset () {
		//this is so generic, there is nothing to reset...
	}

	//abstract stuff

	//shooting should be implemented per weapon. maybe some have parameters, i dont know..
//	public abstract void Shoot ();

	//utility

	protected Vector3 PredictTargetPosition (Vector3 targetPosition, Vector3 targetVelocity, float projectileSpeed, Vector3 aimOrigin, float maxPredictTime = 1f) {
		Vector3 delta = targetPosition - aimOrigin;
		float timeToTarget = delta.magnitude / projectileSpeed;
		float predictTime = Mathf.Min(timeToTarget, maxPredictTime);
		Vector3 extrapolatedPosition = targetPosition + (targetVelocity * predictTime);
		return extrapolatedPosition;
	}

	protected Player GetNearestLivingPlayer () {
		Player nearestLivingPlayer = null;
		float minSqrDist = Mathf.Infinity;
		for(int i=0; i<players.Length; i++){
			Player player = players[i];
			if(!player.IsDead){
				float sqrDist = (player.transform.position - this.transform.position).sqrMagnitude;
				if(sqrDist < minSqrDist){
					minSqrDist = sqrDist;
					nearestLivingPlayer = player;
				}
			}
		}
		return nearestLivingPlayer;
	}

	protected Vector3 GetDirectionScaleVector () {
		switch(gameplayMode){
		case GameplayMode.TOPDOWN:
			return new Vector3(1,0,1);
		case GameplayMode.SIDE:
			return new Vector3(0,1,1);
		case GameplayMode.BACK:
			throw new UnityException("No direction scaling in back mode allowed. A 3D Vector is fine here");
		default :
			throw new UnityException("Unknown GameplayMode \"" + gameplayMode.ToString() + "\"");
		}
	}

	protected float Vec2Angle(Vector2 vec){
		return Mathf.Atan2(vec.y, vec.x);
	}

	protected Vector2 Angle2Vec(float angle){
		return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
	}

	protected float Vec2AngleWithMode(Vector3 vec){
		switch(gameplayMode){
		case GameplayMode.TOPDOWN:
			return Vec2Angle(new Vector2(vec.x, vec.z));
		case GameplayMode.SIDE:
			return Vec2Angle(new Vector2(vec.z, vec.y));
		default:
			throw new UnityException("unsupported " + gameplayMode.ToString());
		}
	}

	protected Vector3 Angle2VecWithMode(float angle){
		Vector2 temp = Angle2Vec(angle);
		switch(gameplayMode){
		case GameplayMode.TOPDOWN:
			return new Vector3(temp.x, 0f, temp.y);
		case GameplayMode.SIDE:
			return new Vector3(0f, temp.y, temp.x);
		default:
			throw new UnityException("unsupported " + gameplayMode.ToString());
		}
	}

}
