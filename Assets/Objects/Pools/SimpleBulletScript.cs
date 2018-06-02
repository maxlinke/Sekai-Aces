using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBulletScript : MonoBehaviour {

	[SerializeField] Rigidbody rb;

	[HideInInspector] public SimpleBulletPool pool;
	[HideInInspector] public int damage;

	void OnCollisionEnter(Collision collision){
		IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
		if(damageable != null){
			damageable.WeaponDamage(damage);
		}
		pool.ReturnToInactivePool(this.rb);
	}

}
