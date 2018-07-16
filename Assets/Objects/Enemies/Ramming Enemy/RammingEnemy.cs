using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RammingEnemy : GenericEnemy, IDamageable {

	[SerializeField] float minSpeed;
	[SerializeField] float maxSpeed;
	[SerializeField] int points;
	[SerializeField] int maxHealth;
	[SerializeField] float maxAngleToPlayerForTargeting;
	[SerializeField] float maxAccel;
	[SerializeField] bool dropPowerUponDeath;
	[SerializeField] PowerUp.PowerUpType powerUpType;

	Player[] players;

	int health;
	bool lastDamageWasWeaponDamage;

	void Start () {
		
	}
	
	void Update () {
		DeathCheck();
	}

	void OnEnable () {
		health = maxHealth;
	}

	void FixedUpdate () {
		Vector3 targetVector = GetTargetVector(GetTargetPlayer());
		float dot = Mathf.Clamp01(Vector3.Dot(targetVector.normalized, rb.velocity.normalized));
		float speed = (dot * maxSpeed) + ((1f - dot) * minSpeed);
		Vector3 targetVelocity = targetVector.normalized * speed;
		Vector3 deltaV = targetVelocity - rb.velocity;
		if(deltaV.magnitude > maxAccel * Time.fixedDeltaTime){
			deltaV = deltaV.normalized * maxAccel * Time.fixedDeltaTime;
		}
		rb.velocity += deltaV;
		transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
	}

	public override void Initialize (Player[] players, GameplayMode mode) {
		this.players = players;
		SetRBTranslationConstraints(mode);
	}

	public override void LevelReset () {
		health = maxHealth;
	}

	public void WeaponDamage (int amount) {	//TODO all this could be generic
		health -= amount;
		lastDamageWasWeaponDamage = true;
	}

	public void CollisionDamage (int amount) {
		health -= amount;
		lastDamageWasWeaponDamage = false;
	}

	public void Kill (bool instantly) {		//i dont like this. 
		health = 0;
		lastDamageWasWeaponDamage = false;
	}

	void OnCollisionEnter (Collision collision) {	//TODO also this could be generic
		IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
		if(damageable != null){
			damageable.CollisionDamage(1);
			health = 0;
		}
	}

	Player GetTargetPlayer () {
		//distance, angle, etc
		float minAllowedDot = Mathf.Cos(maxAngleToPlayerForTargeting);

//		float minSqrDist = Mathf.Infinity;
//		Player minDistPlayer = null;
		float maxDot = Mathf.NegativeInfinity;
		Player maxDotPlayer = null;
		for(int i=0; i<players.Length; i++){
			if(!players[i].IsDead){
				Vector3 delta = (players[i].transform.position - this.transform.position);
				//			float sqrDist = delta.sqrMagnitude;
				float dot = Vector3.Dot(Vector3.back, delta.normalized);
				//			if(sqrDist < minSqrDist){
				//				minSqrDist = sqrDist;
				//				minDistPlayer = players[i];
				//			}
				if((dot > maxDot) && (dot >= minAllowedDot)){
					maxDot = dot;
					maxDotPlayer = players[i];
				}
			}
		}
		return maxDotPlayer;
	}

	Vector3 GetTargetVector (Player targetPlayer) {
		if(targetPlayer != null){
			Vector3 delta = targetPlayer.transform.position - this.transform.position;
			float timeToTarget = Mathf.Clamp01(delta.magnitude / rb.velocity.magnitude);		//eeehh, better a dot-based thing. so project the velocity etc?
			timeToTarget = 0f;
			Vector3 playerPosThen = targetPlayer.transform.position + (targetPlayer.GetVelocity() * timeToTarget);
			return (playerPosThen - transform.position).normalized;
		}else{
			return Vector3.back;
		}
	}

	void DeathCheck () {	//TODO this could be generic
		if(health <= 0){
			if(lastDamageWasWeaponDamage && dropPowerUponDeath){
				PowerUpPool.Instance.NewPowerUp(transform.position, powerUpType);
			}
			ParticleEffectPool.GetPool(ParticleEffectPool.EffectType.FIREBALL_SMALL).NewEffect(transform.position, Random.insideUnitSphere, true, gameObject.layer);
			ScoreSystem.Instance.AddScore(points);
			gameObject.SetActive(false);
		}
	}


}
