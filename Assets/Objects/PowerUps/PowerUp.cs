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

	GameObject activeModel;
	Vector3 rotationOffset;

	PowerUpPool pool;
	PowerUpType type;

	public PowerUpType Type {
		get {
			return this.type;
		}
	}

	void Update () {
		activeModel.transform.rotation = Quaternion.Euler(rotationOffset + (new Vector3(Time.time * 3f, Time.time * 5f, Time.time * 7f) * 20f));
	}

	public void Initialize (PowerUpType type, PowerUpPool pool) {
		this.type = type;
		this.pool = pool;
		ActivateProperModel(type);
		rotationOffset = new Vector3(Random.value, Random.value, Random.value) * 360f;
		rb.velocity = Vector3.back * speed;
	}

	public void ReturnToPool () {
		pool.ReturnToInactivePool(this.rb);
	}

	void ActivateProperModel (PowerUpType type) {
		switch(type){
		case PowerUpType.WEAPON:
			activeModel = weaponModel;
			repairModel.SetActive(false);
			reloadModel.SetActive(false);
			break;
		case PowerUpType.REPAIR:
			weaponModel.SetActive(false);
			activeModel = repairModel;
			reloadModel.SetActive(false);
			break;
		case PowerUpType.RELOAD:
			weaponModel.SetActive(false);
			repairModel.SetActive(false);
			activeModel = reloadModel;
			break;
		default:
			throw new UnityException("Unknown PowerUp-Type \"" + type.ToString() + "\"");
		}
		activeModel.SetActive(true);
	}

}
