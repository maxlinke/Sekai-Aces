using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyBulletWeapon : GenericEnemyWeapon {

	//TODO this isn't the nicest code i've ever written but it works. refactor if i feel like it

		[Header("Components")]
	public GameObject aimOrigin;
	public GameObject[] bulletOrigins;

		[Header("Settings")]
	public BulletType bulletType;
	public FiringMode firingMode;
	public FiringDirection firingDirection;

	public Vector3 fixVector;
	public int numberOfBurstShots;
	public float burstDuration;
	public float sweepAngle;
	public float weaponSpread;

	public bool chargeUp;
	public float chargeTime;

	bool initialized;

	Vector3 bulletDirectionScale;
	SimpleBulletPool bulletPool;
	ParticleEffectPool chargeEffectPool;

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
		VECTOR,
		FORWARD,
		PLAYERDIRECT,
		PLAYERPREDICT
	}

	public override void Initialize(Player[] players, GameplayMode mode, PlayArea playArea){
		base.Initialize(players, mode, playArea);
		bulletDirectionScale = GetDirectionScaleVector();
		SetAppropriateBulletPool();
		chargeEffectPool = ParticleEffectPool.GetPool(ParticleEffectPool.EffectType.CHARGE_ENEMY);
		initialized = true;
	}

	public override void LevelReset(){
		base.LevelReset();
		StopAllCoroutines();
	}

	public void Shoot(){
		if(!initialized) throw new UnityException("weapon not initialized");
		if(chargeUp){
			StartCoroutine(ChargeAndShoot(chargeTime));
		}else{
			ActuallyShoot();
		}
	}

	void ActuallyShoot () {
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

	IEnumerator ChargeAndShoot (float chargeTime) {
		PooledParticleEffect[] chargeEffects = new PooledParticleEffect[bulletOrigins.Length];
		for(int i=0; i<bulletOrigins.Length; i++){
			chargeEffects[i] = chargeEffectPool.NewEffect();
			chargeEffects[i].gameObject.layer = LayerMask.NameToLayer("Default");
			chargeEffects[i].transform.parent = bulletOrigins[i].transform;
			chargeEffects[i].transform.localPosition = Vector3.zero;
			chargeEffects[i].transform.localRotation = Quaternion.identity;
		}
		yield return new WaitForSeconds(chargeTime);
		for(int i=0; i<chargeEffects.Length; i++){
			chargeEffects[i].Deactivate(true);
		}
		ActuallyShoot();
	}

	public Vector3 GetShootDirection(){
		return GetFiringDirectionForTargetingMode(firingDirection);
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

	void ShootInDirection(Vector3 direction){
		for(int i=0; i<bulletOrigins.Length; i++){
			Vector3 randomOffset = Random.insideUnitSphere * weaponSpread;
			Vector3 flattenedOffset = Vector3.Scale(randomOffset, bulletDirectionScale);
			bulletPool.NewBullet(bulletOrigins[i].transform.position, direction + flattenedOffset);
		}
	}

	Vector3 GetFiringDirectionForTargetingMode(FiringDirection mode){
		switch(mode){
		case FiringDirection.VECTOR:
			return fixVector.normalized;
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
			Vector3 positionThen = nearestLivingPlayer.transform.position + (nearestLivingPlayer.velocity * timeToArrival);
			return (positionThen - this.transform.position).normalized;
		}else{
			return Vector3.zero;
		}
	}

}
