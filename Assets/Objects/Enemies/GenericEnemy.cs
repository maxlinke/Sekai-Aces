using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericEnemy : MonoBehaviour, IDamageable, IEnemyComponent {

	protected const string defaultHitboxTag = "Untagged";
	protected const string vulnerableHitboxTag = "ActiveEnemyHitbox";

		[Header("Generic Settings")]
	[SerializeField] protected int maxHealth;
	[SerializeField] protected int pointValue;
	[SerializeField] protected int dealtCollisionDamage;
	[SerializeField] protected bool vulnerableOutsideEnemyArea;
	[SerializeField] protected bool deactivateUponLeavingEnemyArea;

		[Header("Default Death Effect (ignore this if the enemy has a custom death action)")]
	[SerializeField] protected ParticleEffectPool.EffectType deathEffect;

		[Header("Hitboxes (Keep on a different gameObject than other colliders)")]
	[SerializeField] protected Collider[] hitboxes;

	protected Player[] players;
	protected GameplayMode gameplayMode;
	protected PlayArea playArea;

	protected int health;
	protected bool inActiveEnemyArea;

	//monobehavior

	protected virtual void OnTriggerEnter (Collider otherCollider) {
		if(otherCollider.CompareTag("ActiveEnemyArea")){
			inActiveEnemyArea = true;
			UpdateHitboxTags();
		}
	}

	protected virtual void OnTriggerExit (Collider otherCollider) {
		if(otherCollider.CompareTag("ActiveEnemyArea")){
			inActiveEnemyArea = false;
			UpdateHitboxTags();
			if(deactivateUponLeavingEnemyArea){
				gameObject.SetActive(false);
			}
		}
	}

	protected virtual void OnCollisionEnter (Collision collision) {
		IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
		if(damageable != null){
			damageable.CollisionDamage(dealtCollisionDamage);
			Debug.Log(gameObject.name + " dealing collision damage");
		}
	}

	//generic enemy

	public virtual void Initialize (Player[] players, GameplayMode gameplayMode, PlayArea playArea) {
		this.players = players;
		this.gameplayMode = gameplayMode;
		this.playArea = playArea;
		//for example initialize movement and weapons
	}

	public virtual void LevelReset () {
		health = maxHealth;
		inActiveEnemyArea = false;
		UpdateHitboxTags();
		//for example reset movement and weapons
	}

	public abstract void Disappear ();

	//IDamagable

	public void WeaponDamage (int amount) {
		if(vulnerableOutsideEnemyArea || inActiveEnemyArea){
			health -= amount;
		}
		DeathCheck();
	}

	public void CollisionDamage (int amount) {
		if(vulnerableOutsideEnemyArea || inActiveEnemyArea){
			health -= amount;
		}
		DeathCheck();
	}

	//utility

	//TODO since i'm not deleting anything i could still call this directly after damage, right?
	protected void DeathCheck () {
		Debug.Log(gameObject.name + " checking for death");
		if(health <= 0){
			ScoreSystem.Instance.AddScore(pointValue);
			Debug.Log(gameObject.name + " is dead now (deactivated)");
			DeathAction();
		}
	}

	protected virtual void DeathAction () {
		ParticleEffectPool.GetPool(deathEffect).NewEffect(transform.position, transform.forward, true);
		gameObject.SetActive(false);
	}

	protected void UpdateHitboxTags () {
		if(vulnerableOutsideEnemyArea || inActiveEnemyArea){
			TagHitboxes(vulnerableHitboxTag);
		}else{
			TagHitboxes(defaultHitboxTag);
		}
	}

	protected void TagHitboxes (string tag) {
		for(int i=0; i<hitboxes.Length; i++){
			hitboxes[i].gameObject.tag = tag;
		}
	}

}
