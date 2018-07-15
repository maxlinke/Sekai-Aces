using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOOBTrigger : MonoBehaviour {

	void OnTriggerEnter (Collider otherCollider) {
		otherCollider.gameObject.SetActive(false);
	}

}
