using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatyPassiveEnemy : GenericEnemy, IDamageable {

	[SerializeField] float speed;
	[SerializeField] int points;
	[SerializeField] int maxHealth;

	int health;

	void Start () {
		
	}
	
	void Update () {
		DeathCheck();
	}

	void FixedUpdate () {
		rb.velocity = Vector3.back * speed;
	}

	void OnEnable () {
		health = maxHealth;
	}

	public override void Initialize (Player[] players, GameplayMode mode) {
		SetRBTranslationConstraints(mode);
	}

	public override void LevelReset () {
		//nothing, this guy is dumb as a rock
	}

	public void WeaponDamage (int amount) {
		health -= amount;
	}

	public void CollisionDamage (int amount) {
		health -= amount;
	}

	public void Kill (bool instantly) {
		health = 0;
	}

	void OnCollisionEnter (Collision collision) {
		IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
		if(damageable != null){
			damageable.CollisionDamage(1);
		}
	}

	void DeathCheck () {
		if(health <= 0){
			ParticleEffectPool.GetPool(ParticleEffectPool.EffectType.FIREBALL_SMALL).NewEffect(transform.position, Random.insideUnitSphere, true, gameObject.layer);
			ScoreSystem.Instance.AddScore(points);
			gameObject.SetActive(false);
		}
	}
}
