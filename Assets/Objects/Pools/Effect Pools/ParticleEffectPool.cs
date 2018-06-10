using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectPool : ObjectPool {

	static ParticleEffectPool fireballPoolMediumInstance;

	[SerializeField] EffectType type;
	[SerializeField] GameObject effectPrefab;

	bool initialized;
	int effectCount;

	List<PooledParticleEffect> activeEffects;
	List<PooledParticleEffect> inactiveEffects;
	List<PooledParticleEffect> returningEffects;

	enum EffectType{
		FIREBALL_MEDIUM
	}

	void Start () {
		effectCount = 0;
		CheckAndSetInstance();
		activeEffects = new List<PooledParticleEffect>();
		inactiveEffects = new List<PooledParticleEffect>();
		returningEffects = new List<PooledParticleEffect>();
		initialized = true;
	}
	
	void Update () {
		
	}

	public override void ResetPool () {
		if(initialized){
			foreach(PooledParticleEffect effect in activeEffects){
				effect.Deactivate();
				returningEffects.Add(effect);
			}
			foreach(PooledParticleEffect effect in returningEffects){
				ReturnToInactivePool(effect);
			}
			returningEffects.Clear();
		}
	}

	public void ReturnToInactivePool(PooledParticleEffect effect){
		if(effect.gameObject.activeSelf){
			effect.gameObject.SetActive(false);
			activeEffects.Remove(effect);
			inactiveEffects.Add(effect);
		}
	}

	public void NewEffect(Vector3 position, Vector3 direction){
		PooledParticleEffect effect;
		if(!TryTakeEffectFromInactivePool(out effect)){
			effectCount++;
			GameObject effectObject = Instantiate(effectPrefab) as GameObject;
			effectObject.transform.parent = this.transform;
			effectObject.name = "effect " + effectCount;
			effect = effectObject.GetComponent<PooledParticleEffect>();
			effect.pool = this;
		}
		effect.gameObject.SetActive(true);
		effect.transform.position = position;
		effect.transform.localRotation = Quaternion.LookRotation(direction);
		activeEffects.Add(effect);
	}

	bool TryTakeEffectFromInactivePool(out PooledParticleEffect effect){
		int count = inactiveEffects.Count;
		if(count > 0){
			int index = count - 1;
			effect = inactiveEffects[index];
			inactiveEffects.RemoveAt(index);
			return true;
		}else{
			effect = null;
			return false;
		}
	}

	public static ParticleEffectPool GetFireballPoolMedium(){
		return fireballPoolMediumInstance;
	}

	void CheckAndSetInstance(){
		switch(type){
		case EffectType.FIREBALL_MEDIUM:
			if(fireballPoolMediumInstance != null) throw new UnityException("Medium Explosion Pool Instance is not null (Singleton violation)");
			else fireballPoolMediumInstance = this;
			break;
		default:
			throw new UnityException("Unknown Effect Pool Type \"" + type.ToString() + "\"");
		}
	}

}
