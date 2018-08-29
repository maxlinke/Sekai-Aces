using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncomingAA : MonoBehaviour {

	[SerializeField] MeshRenderer mr;
	[SerializeField] Collider coll;

	[SerializeField] float waitTime;
	[SerializeField] int fixedUpdateHurtTicks;

	float explodeTime;
	int fixedUpdateTicks;

	void FixedUpdate () {
		if((Time.time > explodeTime) && mr.enabled){
			mr.enabled = false;
			coll.enabled = true;
//			ParticleEffectPool.GetPool(ParticleEffectPool.EffectType.EXPLOSION_AA).NewEffect(transform.position, transform.forward, true, gameObject.layer);
			ParticleEffectPool.GetPool(ParticleEffectPool.EffectType.EXPLOSION_AA).NewEffect(transform.position, transform.forward, true);
		}
		if(coll.enabled){
			fixedUpdateTicks++;
			if(fixedUpdateTicks > fixedUpdateHurtTicks){
				gameObject.SetActive(false);
			}
		}
	}

	void OnEnable () {
		mr.enabled = true;
		coll.enabled = false;
		explodeTime = Time.time + waitTime;
		fixedUpdateTicks = 0;
	}

	void OnTriggerEnter (Collider otherCollider) {
		IDamageable damageable = otherCollider.gameObject.GetComponent<IDamageable>();
		if(damageable != null){
			damageable.WeaponDamage(1);
		}
	}
}
