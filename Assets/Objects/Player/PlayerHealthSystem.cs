using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour, IDamageable {

		[Header("Components")]
	[SerializeField] Player playerController;
	[SerializeField] ParticleSystem smokeParticleSystem;

		[Header("Variables")]
	[SerializeField] int maxHitPoints;
	[SerializeField] float invulnerabilityTimeAfterHit;

		[Header("Camera Shake")]
	[SerializeField] float hitShakeSpeed;
	[SerializeField] float hitShakeStrength;
	[SerializeField] float hitShakeDuration;
	[SerializeField] float deathShakeSpeed;
	[SerializeField] float deathShakeStrength;
	[SerializeField] float deathShakeDuration;

	[HideInInspector] public PlayerModel playerModel;

	int hitPoints;
	int lastHitPoints;
	float invulnerabilityEnd;
	bool invulnerable;
	bool lastDamageWasCollision;

	void Start () {
		RespawnReset();
	}
	
	void Update () {
		if(invulnerable && (Time.time > invulnerabilityEnd)){
			invulnerable = false;
			playerModel.SetBlinking(false);
		}
		if((hitPoints <= 0) && (hitPoints < lastHitPoints)){	//TODO this can be moved directly into damage, right? just move the camerashake code a bit?
			smokeParticleSystem.Stop();
			playerController.InitiateDeath(lastDamageWasCollision);
			CameraShake.Instance.NewShake(deathShakeStrength, deathShakeDuration, deathShakeSpeed);
		}
		lastHitPoints = hitPoints;
	}

	public void SetInvulnerableForSeconds(float seconds){
		invulnerable = true;
		invulnerabilityEnd = Time.time + seconds;
	}

	public bool IsDead(){
		return (hitPoints <= 0);
	}

	public bool CanBeHealed(){
		return (hitPoints < maxHitPoints);
	}

	public void Heal(){
		hitPoints = maxHitPoints;
		smokeParticleSystem.Stop();
		playerModel.Shine(Color.green);
		//TODO repair sound
	}

	public void RespawnReset(){
		hitPoints = maxHitPoints;
		lastHitPoints = hitPoints;
		invulnerable = false;
		invulnerabilityEnd = Mathf.NegativeInfinity;
		smokeParticleSystem.Stop();
	}

	public void WeaponDamage(int amount){
		if(!invulnerable){
			lastDamageWasCollision = false;
			Damage(amount);
		}
	}

	public void CollisionDamage(int amount){
		if(!invulnerable){
			lastDamageWasCollision = true;
			Damage(amount);
		}
	}

	public void ObstacleCollision () {
		lastDamageWasCollision = true;
		Damage(hitPoints);
		Debug.Log(this.gameObject.name + " : obstacle collision");
	}

	void Damage(int amount){
		hitPoints -= amount;
		CameraShake.Instance.NewShake(hitShakeStrength, hitShakeDuration, hitShakeSpeed);
		if((hitPoints == 1) && (hitPoints < lastHitPoints)){
			//TODO clank sound
			smokeParticleSystem.Play();
		}
		if(hitPoints > 0){
			SetInvulnerableForSeconds(invulnerabilityTimeAfterHit);
			playerModel.SetBlinking(true);
		}
	}

}
