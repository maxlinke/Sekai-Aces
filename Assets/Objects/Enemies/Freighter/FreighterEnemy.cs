using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreighterEnemy : GenericEnemy, IDamageable {

	[SerializeField] GenericEnemyWeapon[] weapons;
	[SerializeField] float speed;
	[SerializeField] int points;
	[SerializeField] int maxHealth;
	[SerializeField] float fireInterval;

	int health;
	float nextShot;

	void Start () {
		
	}
	
	void Update () {
		DeathCheck();
		if(Time.time > nextShot){
			for(int i=0; i<weapons.Length; i++){
				weapons[i].Shoot();
			}
			nextShot = Time.time + fireInterval;
		}
	}

	void FixedUpdate () {
		rb.velocity = Vector3.back * speed;
	}

	public override void Initialize (Player[] players, GameplayMode mode){
		SetRBTranslationConstraints(mode);
		for(int i=0; i<weapons.Length; i++){
			weapons[i].Initialize(players, mode);
		}
	}

	void OnCollisionEnter (Collision collision) {	//TODO also this could be generic
		IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
		if(damageable != null){
			damageable.CollisionDamage(1);
		}
	}

	void DeathCheck () {	//TODO this could be generic
		if(health <= 0){
			ParticleEffectPool.GetPool(ParticleEffectPool.EffectType.FIREBALL_MEDIUM).NewEffect(transform.position, Random.insideUnitSphere, true, gameObject.layer);
			ScoreSystem.Instance.AddScore(points);
			gameObject.SetActive(false);
		}
	}

	public override void LevelReset (){
		health = maxHealth;
		nextShot = 0f;
	}

	public void WeaponDamage (int amount) {	//TODO all this could be generic
		health -= amount;
	}

	public void CollisionDamage (int amount) {
		health -= amount;
	}

	public void Kill (bool instantly) {		//i dont like this. 
		health = 0;
	}
}
