using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectPool : ObjectPool {

	static ParticleEffectPool fireballPoolMediumInstance;
	static ParticleEffectPool fireballPoolSmallInstance;

	[SerializeField] EffectType type;
	[SerializeField] GameObject effectPrefab;

	int effectCount;

	List<PooledParticleEffect> activeEffects;
	List<PooledParticleEffect> inactiveEffects;
	List<PooledParticleEffect> returningEffects;

	enum EffectType{
		FIREBALL_SMALL,
		FIREBALL_MEDIUM
	}

	public override void Initialize () {
		effectCount = 0;
		CheckAndSetInstance();
		activeEffects = new List<PooledParticleEffect>();
		inactiveEffects = new List<PooledParticleEffect>();
		returningEffects = new List<PooledParticleEffect>();
	}

	public override void ResetPool () {
		foreach(PooledParticleEffect effect in activeEffects){
			effect.Deactivate();
			returningEffects.Add(effect);
		}
		foreach(PooledParticleEffect effect in returningEffects){
			ReturnToInactivePool(effect);
		}
		returningEffects.Clear();
	}

	public void ReturnToInactivePool(PooledParticleEffect effect){
		if(effect.gameObject.activeSelf){
			effect.gameObject.SetActive(false);
			activeEffects.Remove(effect);
			inactiveEffects.Add(effect);
		}
	}

	public void NewEffect(Vector3 position, Vector3 direction, bool onPlayArea, int layer){
		PooledParticleEffect effect;
		if(!TryTakeEffectFromInactivePool(out effect)){
			effectCount++;
			GameObject effectObject = Instantiate(effectPrefab) as GameObject;
			effectObject.name = "effect " + effectCount;
			effect = effectObject.GetComponent<PooledParticleEffect>();
			effect.pool = this;
		}
		effect.gameObject.SetActive(true);
		effect.gameObject.transform.parent = (onPlayArea ? this.transform : null);
		effect.transform.position = position;
		effect.transform.localRotation = Quaternion.LookRotation(direction);
		effect.SetLayerIncludingAllChildren(effect.gameObject, layer);
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

	public static ParticleEffectPool GetFireballPoolSmall(){
		return fireballPoolSmallInstance;
	}

	public static ParticleEffectPool GetFireballPoolMedium(){
		return fireballPoolMediumInstance;
	}

	void CheckAndSetInstance(){
		switch(type){
		case EffectType.FIREBALL_SMALL:
			if(fireballPoolSmallInstance != null) throw new UnityException("Small Fireball Pool instance is not null (Singleton violation)");
			else fireballPoolSmallInstance = this;
			break;
		case EffectType.FIREBALL_MEDIUM:
			if(fireballPoolMediumInstance != null) throw new UnityException("Medium Fireball Pool Instance is not null (Singleton violation)");
			else fireballPoolMediumInstance = this;
			break;
		default:
			throw new UnityException("Unknown Effect Pool Type \"" + type.ToString() + "\"");
		}
	}

}
