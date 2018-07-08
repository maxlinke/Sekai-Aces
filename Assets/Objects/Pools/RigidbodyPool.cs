using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyPool : ObjectPool {

	protected List<Rigidbody> activeRBs;
	protected List<Rigidbody> inactiveRBs;
	protected List<Rigidbody> returningRBs;

	[SerializeField] float rbMaxDistance;

	float sqrRBMaxDistance;

	public override void Initialize () {
		activeRBs = new List<Rigidbody>();
		inactiveRBs = new List<Rigidbody>();
		returningRBs = new List<Rigidbody>();
		sqrRBMaxDistance = rbMaxDistance * rbMaxDistance;
	}

	public override void ResetPool () {
		returningRBs.AddRange(activeRBs);
		for(int i=0; i<returningRBs.Count; i++){
			ReturnToInactivePool(returningRBs[i]);
		}
		returningRBs.Clear();
	}

	public void ReturnToInactivePool (Rigidbody rb) {
		if(rb.gameObject.activeSelf){
			rb.gameObject.SetActive(false);
			activeRBs.Remove(rb);
			inactiveRBs.Add(rb);
		}
	}

	protected void SqrDistReturnCheck () {
		for(int i=0; i<activeRBs.Count; i++){
			if(activeRBs[i].transform.localPosition.sqrMagnitude > sqrRBMaxDistance){
				returningRBs.Add(activeRBs[i]);
			}
		}
		for(int i=0; i<returningRBs.Count; i++){
			ReturnToInactivePool(returningRBs[i]);
		}
		returningRBs.Clear();
	}

	protected bool TryTakeRBFromInactivePool (out Rigidbody rb) {
		int count = inactiveRBs.Count;
		if(count > 0){
			int index = count - 1;
			rb = inactiveRBs[index];
			inactiveRBs.RemoveAt(index);
			return true;
		}else{
			rb = null;
			return false;
		}
	}

}
