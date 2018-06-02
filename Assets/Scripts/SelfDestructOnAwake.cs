using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructOnAwake : MonoBehaviour {

	void Awake(){
		Destroy(this.gameObject);
	}

}
