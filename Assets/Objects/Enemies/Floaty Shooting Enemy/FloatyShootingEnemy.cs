using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatyShootingEnemy : GenericEnemy, IDamageable {

	[SerializeField] float speed;
	[SerializeField] int points;
	[SerializeField] int maxHealth;
	[SerializeField] GenericEnemyWeapon weapon;
	[SerializeField] float shotInterval;

	int health;
	float nextShot;
	bool armed;

	void Start () {
		armed = false;
	}

	void Update () {
		DeathCheck();
	}

	void FixedUpdate () {
		rb.velocity = Vector3.back * speed;
		if(armed){
			if(Time.time > nextShot){
				weapon.Shoot();
				nextShot = Time.time + shotInterval;
			}
		}
		armed = false;
	}

	void OnEnable () {
		health = maxHealth;
	}

	public override void Initialize (Player[] players, GameplayMode mode) {
		weapon.Initialize(players, mode);
		SetRBTranslationConstraints(mode);
	}

	public override void LevelReset () {
		nextShot = 0f;
	}

	public void WeaponDamage (int amount) {	//TODO all this could be generic
		health -= amount;
	}

	public void CollisionDamage (int amount) {
		health -= amount;
	}

	public void Kill (bool instantly) {
		health = 0;
	}

	void OnCollisionEnter (Collision collision) {	//TODO also this could be generic
		IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
		if(damageable != null){
			damageable.CollisionDamage(1);
		}
	}

	void OnTriggerStay (Collider otherCollider) {	//TODO this could be generic
		if(otherCollider.CompareTag("EnemyArmingTrigger")) armed = true;
	}

	void DeathCheck () {	//TODO this could be generic
		if(health <= 0){
			ParticleEffectPool.GetPool(ParticleEffectPool.EffectType.FIREBALL_SMALL).NewEffect(transform.position, Random.insideUnitSphere, true, gameObject.layer);
			ScoreSystem.Instance.AddScore(points);
			gameObject.SetActive(false);
		}
	}
}
