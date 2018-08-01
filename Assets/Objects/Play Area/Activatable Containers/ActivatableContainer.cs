using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActivatableContainer : MonoBehaviour {

		[Header("Type")]
	[SerializeField] ActivationType type;
	[SerializeField] protected float staggeredActivationInterval;

		[Header("Retriggering")]
	[SerializeField] bool retriggerable;
	[SerializeField] bool resetBeforeRetrigger;

	bool activated;

	public enum ActivationType{
		ALLATONCE,
		STAGGERED
	}

	public void Activate () {
		if(!activated || retriggerable){
			if(!activated){
				activated = true;
				gameObject.SetActive(true);	//possibly redundant. i dont know. i have this set in the enemysystem to activate everything in the overall reset
				gameObject.name += " (activated)";
			}
			if(retriggerable && resetBeforeRetrigger){
				LevelReset();
			}
			if(type.Equals(ActivationType.ALLATONCE)){
				ActivateAllAtOnce();
			}else if(type.Equals(ActivationType.STAGGERED)){
				ActivateStaggered();
			}else{
				throw new UnityException("unknown activationtype \"" + type.ToString() + "\"");
			}
		}
	}

	public void Deactivate () {
		activated = false;
		DeactivateContainer();
	}

	public void LevelReset () {
		activated = false;
		ResetContainer();
	}

	protected abstract void ActivateAllAtOnce ();

	protected abstract void ActivateStaggered ();

	protected abstract void DeactivateContainer ();

	protected abstract void ResetContainer ();

}
