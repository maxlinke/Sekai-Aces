using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingSatDish : MonoBehaviour, IDamageable, ILevelResetee {

	[SerializeField] GameObject theObject;
	[SerializeField] Collider coll;
	[SerializeField] GameObject[] explosionPoints;
	[SerializeField] int maxHP;

	int hitpoints;

	void Update () {
		if(hitpoints <= 0 && coll.enabled){
			for(int i=0; i<explosionPoints.Length; i++){
				ParticleEffectPool pool;
				if(Random.value > 0.5f){
					pool = ParticleEffectPool.GetPool(ParticleEffectPool.EffectType.FIREBALL_MEDIUM);
					Debug.Log("medium");
				}else{
					pool = ParticleEffectPool.GetPool(ParticleEffectPool.EffectType.FIREBALL_SMALL);
					Debug.Log("small");
				}
				pool.NewEffect(explosionPoints[i].transform.position, Random.insideUnitSphere, gameObject.transform.parent, gameObject.layer);	//LayerMask.NameToLayer("Background")
			}
			theObject.SetActive(false);
			coll.enabled = false;
		}
	}

	public void LevelReset () {
		theObject.SetActive(true);
		coll.enabled = true;
		hitpoints = maxHP;
	}

	public void WeaponDamage (int amount) {
		hitpoints -= amount;
	}

	public void CollisionDamage (int amount) {
		hitpoints -= amount;
	}
}
