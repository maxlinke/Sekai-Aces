using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathSystem : MonoBehaviour {

		[Header("Components")]
	[SerializeField] Rigidbody rb;

		[Header("Prefabs")]
	[SerializeField] GameObject explosionPrefab;

	[HideInInspector] public PlayerModel playerModel;

	bool isExploded;
	bool justActivated;
	bool activelyDead;
	Vector3 lastPos;

	void Start () {
		Reset();
	}
	
	void Update () {
		
	}

	void FixedUpdate(){
		if(justActivated){
			Vector3 currentPos = transform.position;
			Vector3 deltaPos = currentPos - lastPos;
			Vector3 velocity = deltaPos / Time.fixedDeltaTime;
			transform.parent = null;
			rb.useGravity = true;
			rb.constraints = RigidbodyConstraints.None;
			rb.velocity = velocity;
			rb.angularVelocity = Random.insideUnitSphere * 10f * Random.value;
			rb.interpolation = RigidbodyInterpolation.Interpolate;
			justActivated = false;
			activelyDead = true;
		}
		lastPos = transform.position;
	}

	public void Reset(){
		isExploded = false;
		justActivated = false;
		activelyDead = false;		 
	}

	public bool IsExploded(){
		return isExploded;
	}

	public void Explode(bool disableGameobject){
		justActivated = true;

		GameObject expl = Instantiate(explosionPrefab);		//TODO explosion pool
		expl.transform.position = transform.position;		//TODO fragments pool
		expl.transform.localScale = Vector3.one * 3f;

		isExploded = true;
		if(disableGameobject) gameObject.SetActive(false);
	}

	public void Crash(){
		justActivated = true;
		isExploded = false;
	}

	void OnCollisionEnter(Collision collision){
		if(activelyDead){
			Explode(true);
		}
	}

}
