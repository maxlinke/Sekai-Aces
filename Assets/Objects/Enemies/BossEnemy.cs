using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : GenericEnemy, IDamageable {

	[Header("Stuff")]
	[SerializeField] GameObject regularPosition;
	[SerializeField] GameObject sineUpPosition;
	[SerializeField] GameObject sineDownPosition;
	[SerializeField] GameObject ramPreparePosition;
	[SerializeField] GameObject ramPosition;

	[Header("Settings")]
	[SerializeField] int maxHP;
	[SerializeField] int points;
	[SerializeField] float moveSpeed;
	[SerializeField] float oscillateSpeed;
	[SerializeField] float ramPrepareSpeed;
	[SerializeField] float ramSpeed;
	[SerializeField] int straightShootTickInterval;
	[SerializeField] int burstShootWaitTicks;

	[Header("Components")]
	[SerializeField] GenericEnemyWeapon[] straightShooterWeapons;
	[SerializeField] GenericEnemyWeapon[] burstWeapons;

	GameController gameController;
	CombatState state;
	int hp;
	float sineMotionStartTime;
	bool gameEndMessageSent;

	int fixedUpdateTicks;
	int fixedUpdateTicksSinceLastMove;

	enum CombatState {
		MOVINGTOREGULARPOSITION,
		SINEUP1, SINEDOWN1, SINEUP2, SINEDOWN2,
		MOVINGBACKTOREGULARPOSITION,
		BURSTFIRING,
		RAMPREPARE,
		RAMEXECUTE
	}

	void Update () {
		if(hp <= 0){
			rb.useGravity = true;
			ParticleEffectPool.GetPool(ParticleEffectPool.EffectType.FIREBALL_MEDIUM).NewEffect(transform.position, Random.insideUnitSphere, true, gameObject.layer);
			if(!gameEndMessageSent){
				GameController.Instance.EndGame();
				gameEndMessageSent = true;
			}
		}
	}

	void FixedUpdate () {
		if(hp > 0){
			rb.velocity = Vector3.zero;
			fixedUpdateTicks++;
			fixedUpdateTicksSinceLastMove++;
			switch(state){
			case CombatState.MOVINGTOREGULARPOSITION:
				MoveToPosition(regularPosition.transform.position, moveSpeed, CombatState.SINEUP1);
				break;
			case CombatState.SINEUP1:
				MoveToPosition(sineUpPosition.transform.position, oscillateSpeed, CombatState.SINEDOWN1);
				FireStraightWeapon();
				break;
			case CombatState.SINEDOWN1:
				MoveToPosition(sineDownPosition.transform.position, oscillateSpeed, CombatState.SINEUP2);
				FireStraightWeapon();
				break;
			case CombatState.SINEUP2:
				MoveToPosition(sineUpPosition.transform.position, oscillateSpeed, CombatState.SINEDOWN2);
				FireStraightWeapon();
				break;
			case CombatState.SINEDOWN2:
				MoveToPosition(sineDownPosition.transform.position, oscillateSpeed, CombatState.MOVINGBACKTOREGULARPOSITION);
				FireStraightWeapon();
				break;
			case CombatState.MOVINGBACKTOREGULARPOSITION:
				MoveToPosition(regularPosition.transform.position, moveSpeed, CombatState.BURSTFIRING);
				break;
			case CombatState.BURSTFIRING:
				BurstFire(CombatState.RAMPREPARE);
				break;
			case CombatState.RAMPREPARE:
				MoveToPosition(ramPreparePosition.transform.position, ramPrepareSpeed, CombatState.RAMEXECUTE);
				break;
			case CombatState.RAMEXECUTE:
				MoveToPosition(ramPosition.transform.position, ramSpeed, CombatState.MOVINGTOREGULARPOSITION);
				break;
			default: throw new UnityException("unknown state " + state.ToString());
			}
		}
	}

	void MoveToPosition (Vector3 point, float speed, CombatState afterState) {
		Vector3 delta = point - this.transform.position;
		Vector3 v;
		if(delta.magnitude < (speed * Time.fixedDeltaTime)){
			v = delta / Time.fixedDeltaTime;
			state = afterState;
			fixedUpdateTicksSinceLastMove = 0;
		}else{
			v = delta.normalized * speed;
		}
		rb.velocity = v;
	}

	void BurstFire (CombatState afterState) {
		if(fixedUpdateTicksSinceLastMove == 2){
			FireBurstWeapon();
		}
		if(fixedUpdateTicksSinceLastMove > burstShootWaitTicks){
			state = afterState;
		}
	}

	void FireStraightWeapon () {
		if((fixedUpdateTicks % straightShootTickInterval) == 0){
			for(int i=0; i<straightShooterWeapons.Length; i++){
				straightShooterWeapons[i].Shoot();
			}
		}
	}

	void FireBurstWeapon () {
		for(int i=0; i<burstWeapons.Length; i++){
			burstWeapons[i].Shoot();
		}
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
		rb.useGravity = false;
		state = CombatState.MOVINGTOREGULARPOSITION;
		hp = maxHP;
		gameEndMessageSent = false;
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
