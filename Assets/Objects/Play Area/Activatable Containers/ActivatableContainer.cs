using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActivatableContainer : MonoBehaviour {

	[SerializeField] protected ActivationType type;
	[SerializeField] protected float staggeredActivationInterval;

	public enum ActivationType{
		ALLATONCE,
		STAGGERED
	}

	public void Activate () {
		gameObject.SetActive(true);
		if(type.Equals(ActivationType.ALLATONCE)){
			ActivateAllAtOnce();
		}else if(type.Equals(ActivationType.STAGGERED)){
			ActivateStaggered();
		}else{
			throw new UnityException("unknown activationtype \"" + type.ToString() + "\"");
		}
	}

	protected abstract void ActivateAllAtOnce ();

	protected abstract void ActivateStaggered ();

	public abstract void LevelReset ();

}
