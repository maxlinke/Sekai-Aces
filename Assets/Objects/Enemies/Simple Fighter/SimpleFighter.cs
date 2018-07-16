using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFighter : GenericEnemy, IDamageable {

	[SerializeField] GameObject moveTarget;
	[SerializeField] float waitTimeOnTarget;
	[SerializeField] GenericEnemyWeapon weapon;
	[SerializeField] int maxHealth; 
	[SerializeField] int points;
	[SerializeField] float speed;

	[SerializeField] bool dropPowerupUponDeath;
	[SerializeField] PowerUp.PowerUpType powerUpType;

	bool beenToTarget;
	bool shot;
	int health;
	bool lastDamageWasWeaponDamage;
	float escapeTime;
	
	void Update () {
		DeathCheck();
	}

	void FixedUpdate () {
		rb.velocity = Vector3.zero;
		if(!beenToTarget){
			Vector3 delta = moveTarget.transform.position - this.transform.position;
			if(delta.magnitude > speed * Time.fixedDeltaTime){
				rb.velocity = delta.normalized * speed;
			}else{
				rb.velocity = delta / Time.fixedDeltaTime;
			}
		}
		if((moveTarget.transform.position - this.transform.position).magnitude < 0.5f){
			beenToTarget = true;
		}
		if(beenToTarget && !shot){
			weapon.Shoot();
			shot = true;
			escapeTime = Time.time + waitTimeOnTarget;
		}
		if(Time.time > escapeTime){
			rb.velocity = Vector3.back * speed;
		}
	}

	public override void Initialize (Player[] players, GameplayMode mode) {
		weapon.Initialize(players, mode);
		SetRBTranslationConstraints(mode);
	}

	public override void LevelReset () {
		health = maxHealth;
		beenToTarget = false;
		shot = false;
		escapeTime = Mathf.Infinity;
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

	void DeathCheck () {	//TODO this could be generic
		if(health <= 0){
			if(lastDamageWasWeaponDamage && dropPowerupUponDeath){
				PowerUpPool.Instance.NewPowerUp(transform.position, powerUpType);
			}
			ParticleEffectPool.GetPool(ParticleEffectPool.EffectType.FIREBALL_SMALL).NewEffect(transform.position, Random.insideUnitSphere, true, gameObject.layer);
			ScoreSystem.Instance.AddScore(points);
			gameObject.SetActive(false);
		}
	}
}
