using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatyPassiveEnemy : GenericEnemy, IDamageable {

	[SerializeField] int points;
	[SerializeField] int maxHealth;
	[SerializeField] Rigidbody rb;

	int health;

	void Start () {
		
	}
	
	void Update () {
		DeathCheck();
		OOBCheck();
	}

	void FixedUpdate () {
		rb.velocity = Vector3.back;
	}

	void OnEnable () {
		health = maxHealth;
	}

	public override void Initialize (Player[] players, GameplayMode mode) {
		//nothing, this guy is dumb as a rock
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
			ScoreSystem.Instance.AddScore(points);
			gameObject.SetActive(false);
		}
	}

	void OOBCheck () {
		//TODO
	}
}
