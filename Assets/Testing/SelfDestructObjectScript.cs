using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructObjectScript : MonoBehaviour {

	[SerializeField] float lifeTime;
	float killTime;

	void Start(){
		killTime = Time.time + lifeTime;
	}
	
	void Update(){
		if(Time.time >= killTime) Destroy(this.gameObject);
	}
}
