using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericEnemy : MonoBehaviour, IDamageable {

		[Header("Rigidbody Settings")]
	[SerializeField] protected Rigidbody rb;
	[SerializeField] protected bool useTranslationConstraints;

		[Header("Generic Settings")]
	[SerializeField] protected int maxHealth;
	[SerializeField] protected int pointValue;
	[SerializeField] protected int dealtCollisionDamage;
	[SerializeField] protected GenericEnemyWeapon[] weapons;

	protected Player[] players;
	protected GameplayMode mode;

	protected int health;
	protected bool inActiveEnemyArea;

	//monobehavior

	protected virtual void OnTriggerEnter (Collider otherCollider) {
		if(otherCollider.CompareTag("ActiveEnemyArea")){
			inActiveEnemyArea = true;
		}
	}

	protected virtual void OnTriggerExit (Collider otherCollider) {
		if(otherCollider.CompareTag("ActiveEnemyArea")){
			inActiveEnemyArea = false;
			gameObject.SetActive(false);
			Debug.Log("Deactivated " + gameObject.name + " because it left the area");
		}
	}

	protected virtual void OnCollisionEnter (Collision collision) {
		IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
		if(damageable != null){
			damageable.CollisionDamage(dealtCollisionDamage);
		}
	}

	//generic enemy

	public virtual void Initialize (Player[] players, GameplayMode mode) {
		this.players = players;
		this.mode = mode;
		if(useTranslationConstraints) SetRBTranslationConstraints(mode);
		for(int i=0; i<weapons.Length; i++){
			weapons[i].Initialize(players, mode);
		}
	}

	public virtual void LevelReset () {
		health = maxHealth;
		inActiveEnemyArea = false;
	}

	//IDamagable

	public void WeaponDamage (int amount) {
		if(inActiveEnemyArea){
			health -= amount;
		}
	}

	public void CollisionDamage (int amount) {
		if(inActiveEnemyArea){
			health -= amount;
		}
	}

	//utility

	protected void SetRBTranslationConstraints (GameplayMode mode) {
		RigidbodyConstraints rotationConstraints = rb.constraints & RigidbodyConstraints.FreezeRotation;
		RigidbodyConstraints positionConstraints;
		switch(mode){
		case GameplayMode.TOPDOWN:
			positionConstraints = RigidbodyConstraints.FreezePositionY;
			break;
		case GameplayMode.SIDE:
			positionConstraints = RigidbodyConstraints.FreezePositionX;
			break;
		default:
			throw new UnityException("unknown mode " + mode.ToString());
		}
		rb.constraints = positionConstraints | rotationConstraints;
	}

}
