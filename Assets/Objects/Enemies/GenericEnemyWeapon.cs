using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEnemyWeapon : MonoBehaviour {

	//TODO this isn't the nicest code i've ever written but it works. refactor if i feel like it

		[Header("Components")]
	[SerializeField] GameObject aimOrigin;
	[SerializeField] GameObject[] bulletOrigins;

		[Header("Settings")]
	[SerializeField] BulletType bulletType;
	[SerializeField] FiringMode firingMode;
	[SerializeField] FiringDirection firingDirection;

	public int numberOfBurstShots;
	public float burstDuration;
	public float sweepAngle;
	public float weaponSpread;

	GameplayMode gameplayMode;
	Vector3 bulletDirectionScale;

	SimpleBulletPool bulletPool;

	Player[] players;

	public enum BulletType{
		SLOW,
		NORMAL,
		FAST
	}

	public enum FiringMode{
		SINGLESHOT,
		BURST,
		SWEEPBURST
	}

	public enum FiringDirection{
		NEGATIVEZ,
		FORWARD,
		PLAYERDIRECT,
		PLAYERPREDICT
	}

	public void Initialize(Player[] players, GameplayMode mode){
		this.players = players;
		this.gameplayMode = mode;
		bulletDirectionScale = GetDirectionScaleVector();
		SetAppropriateBulletPool();
	}

	public void RespawnReset(){
		StopAllCoroutines();
	}

	public void Shoot(){
		switch(firingMode){
		case FiringMode.SINGLESHOT:
			SingleShotFiringAction();
			break;
		case FiringMode.BURST:
			StartCoroutine(BurstFiringAction());
			break;
		case FiringMode.SWEEPBURST:
			StartCoroutine(SweepingBurstFiringAction());
			break;
		}
	}

	public Vector3 GetShootDirection(){
		return GetFiringDirectionForTargetingMode(firingDirection);
	}

	Vector3 GetDirectionScaleVector(){
		switch(gameplayMode){
		case GameplayMode.TOPDOWN:
			return new Vector3(1,0,1);
		case GameplayMode.SIDE:
			return new Vector3(0,1,1);
		case GameplayMode.BACK:
			return new Vector3(1,1,0);
		default :
			throw new UnityException("Unknown GameplayMode \"" + gameplayMode.ToString() + "\"");
		}
	}

	void SetAppropriateBulletPool(){
		switch(bulletType){
		case BulletType.SLOW:
			bulletPool = SimpleBulletPool.GetPool(SimpleBulletPool.BulletPoolType.ENEMY_SLOW);
			break;
		case BulletType.NORMAL:
			bulletPool = SimpleBulletPool.GetPool(SimpleBulletPool.BulletPoolType.ENEMY_NORMAL);
			break;
		case BulletType.FAST:
			bulletPool = SimpleBulletPool.GetPool(SimpleBulletPool.BulletPoolType.ENEMY_FAST);
			break;
		default:
			throw new UnityException("unknown bullet type " + bulletType.ToString());
		}
	}

	void SingleShotFiringAction(){
		Vector3 direction = GetFiringDirectionForTargetingMode(firingDirection);
		if(direction != Vector3.zero){
			ShootInDirection(direction);
		}
	}

	IEnumerator BurstFiringAction(){
		Vector3 direction = GetFiringDirectionForTargetingMode(firingDirection);
		if(direction == Vector3.zero){
			yield break;
		}else{
			for(int i=0; i<numberOfBurstShots; i++){
				ShootInDirection(direction);
				yield return new WaitForSeconds(burstDuration / numberOfBurstShots);
			}
		}
	}

	IEnumerator SweepingBurstFiringAction(){
		Vector3 initialDirection = GetFiringDirectionForTargetingMode(firingDirection);
		float initialAngle;

		if(gameplayMode.Equals(GameplayMode.BACK)) throw new UnityException("sweeping not supported for back to front mode");
		else initialAngle = Vec2AngleWithMode(initialDirection);

		if(initialDirection != Vector3.zero){
			for(int i=0; i<numberOfBurstShots; i++){
				float angleOffset = sweepAngle * (((float)i / (numberOfBurstShots - 1)) - 0.5f);
				float newAngle = initialAngle + (Mathf.Deg2Rad * angleOffset);
				Vector3 newDirection = Angle2VecWithMode(newAngle);
				ShootInDirection(newDirection);
				yield return new WaitForSeconds(burstDuration / numberOfBurstShots);
			}
		}
	}

	float Vec2AngleWithMode(Vector3 vec){
		switch(gameplayMode){
		case GameplayMode.TOPDOWN:
			return Vec2Angle(new Vector2(vec.x, vec.z));
		case GameplayMode.SIDE:
			return Vec2Angle(new Vector2(vec.z, vec.y));
		default:
			throw new UnityException("unsupported " + gameplayMode.ToString());
		}
	}

	float Vec2Angle(Vector2 vec){
		return Mathf.Atan2(vec.y, vec.x);
	}

	Vector3 Angle2VecWithMode(float angle){
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

	Vector2 Angle2Vec(float angle){
		return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
	}

	void ShootInDirection(Vector3 direction){
		for(int i=0; i<bulletOrigins.Length; i++){
			Vector3 randomOffset = Random.insideUnitSphere * weaponSpread;
			Vector3 flattenedOffset = Vector3.Scale(randomOffset, bulletDirectionScale);
			bulletPool.NewBullet(bulletOrigins[i].transform.position, direction + flattenedOffset);
		}
	}

	Vector3 GetFiringDirectionForTargetingMode(FiringDirection mode){
		switch(mode){
		case FiringDirection.NEGATIVEZ:
			return Vector3.back;
		case FiringDirection.FORWARD:
			return transform.forward;
		case FiringDirection.PLAYERDIRECT:
			return GetVectorToNearestLivingPlayer();
		case FiringDirection.PLAYERPREDICT:
			return GetPrecictVectorToNearestLivingPlayer();
		default:
			throw new UnityException("unsupported targetingmode \"" + mode.ToString() + "\"");
		}
	}

	Player GetNearestLivingPlayer(){
		Player nearestLivingPlayer = null;
		float minSqrDist = Mathf.Infinity;
		for(int i=0; i<players.Length; i++){
			Player player = players[i];
			if(!player.IsDead){
				float sqrDist = (player.transform.position - aimOrigin.transform.position).sqrMagnitude;
				if(sqrDist < minSqrDist){
					minSqrDist = sqrDist;
					nearestLivingPlayer = player;
				}
			}
		}
		return nearestLivingPlayer;
	}
	
	Vector3 GetVectorToNearestLivingPlayer(){
		Player nearestLivingPlayer = GetNearestLivingPlayer();
		if(nearestLivingPlayer != null){
			return (nearestLivingPlayer.transform.position - aimOrigin.transform.position).normalized;
		}else{
			return Vector3.zero;
		}
	}

	Vector3 GetPrecictVectorToNearestLivingPlayer(){
		Player nearestLivingPlayer = GetNearestLivingPlayer();
		if(nearestLivingPlayer != null){
			Vector3 difference = (nearestLivingPlayer.transform.position - aimOrigin.transform.position);
			float timeToArrival = difference.magnitude / bulletPool.BulletSpeed;
			Vector3 positionThen = nearestLivingPlayer.transform.position + (nearestLivingPlayer.GetVelocity() * timeToArrival);
			return (positionThen - this.transform.position).normalized;
		}else{
			return Vector3.zero;
		}
	}

}
