using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : GenericEnemy, IDamageable {

	[Header("Stuff")]
	[SerializeField] GameObject regularPosition;

	[Header("Settings")]
	[SerializeField] int maxHP;
	[SerializeField] int points;
	[SerializeField] float moveSpeed;
	[SerializeField] float oscillateTime;
	[SerializeField] float oscillateWidth;
	[SerializeField] float ramPrepareSpeed;
	[SerializeField] float ramSpeed;

	[Header("Components")]
	[SerializeField] GenericEnemyWeapon[] straightShooterWeapons;
	[SerializeField] GenericEnemyWeapon[] burstWeapons;

	GameController gameController;
	CombatState state;
	int hp;

	enum CombatState {
		MOVINGTOREGULARPOSITION,
		SINEMOTION,
		BURSTFIRING,
		RAMMING
	}

	void FixedUpdate () {
		switch(state){
		case CombatState.MOVINGTOREGULARPOSITION:
			MoveToRegularPosition();
			break;
		case CombatState.SINEMOTION:

			break;
		case CombatState.BURSTFIRING:

			break;
		case CombatState.RAMMING:

			break;
		default: throw new UnityException("unknown state " + state.ToString());
		}
	}

	void MoveToRegularPosition () {

	}

	public override void Initialize (Player[] players, GameplayMode mode) {
		for(int i=0; i<straightShooterWeapons.Length; i++){
			straightShooterWeapons[i].Initialize(players, mode);
		}
		for(int i=0; i<burstWeapons.Length; i++){
			burstWeapons[i].Initialize(players, mode);
		}
		SetRBTranslationConstraints(mode);
	}

	public override void LevelReset () {
		state = CombatState.MOVINGTOREGULARPOSITION;
		hp = maxHP;
	}

	public void WeaponDamage (int amount) {
		hp -= amount;
	}

	public void CollisionDamage (int amount) {
		hp -= amount;
	}

	public void Kill (bool instantly) {
		hp = 0;
	}

	void OnCollisionEnter (Collision collision) {
		IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
		if(damageable != null){
			damageable.CollisionDamage(1);
		}
	}

}
