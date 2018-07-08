using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

	public enum PowerUpType {
		WEAPON,
		REPAIR,
		RELOAD
	}

		[Header("Components")]
	[SerializeField] Rigidbody rb;
	[SerializeField] GameObject weaponModel;
	[SerializeField] GameObject repairModel;
	[SerializeField] GameObject reloadModel;

		[Header("Settings")]
	[SerializeField] float speed;

	PowerUpPool pool;
	PowerUpType type;

	public PowerUpType Type {
		get {
			return this.type;
		}
	}

	public void Initialize (PowerUpType type, PowerUpPool pool) {
		this.type = type;
		this.pool = pool;
		ActivateProperModel(type);
		rb.velocity = Vector3.back * speed;
	}

	public void ReturnToPool () {
		pool.ReturnToInactivePool(this.rb);
	}

	void ActivateProperModel (PowerUpType type) {
		switch(type){
		case PowerUpType.WEAPON:
			weaponModel.SetActive(true);
			repairModel.SetActive(false);
			reloadModel.SetActive(false);
			break;
		case PowerUpType.REPAIR:
			weaponModel.SetActive(false);
			repairModel.SetActive(true);
			reloadModel.SetActive(false);
			break;
		case PowerUpType.RELOAD:
			weaponModel.SetActive(false);
			repairModel.SetActive(false);
			reloadModel.SetActive(true);
			break;
		default:
			throw new UnityException("Unknown PowerUp-Type \"" + type.ToString() + "\"");
		}
	}

}
