using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectPool : ObjectPool {

	static SortedList<EffectType, ParticleEffectPool> map;

	[SerializeField] EffectType type;
	[SerializeField] GameObject effectPrefab;

	int effectCount;

	List<PooledParticleEffect> activeEffects;
	List<PooledParticleEffect> inactiveEffects;
	List<PooledParticleEffect> returningEffects;

	public enum EffectType{
		EXPLOSION_AA,
		FIREBALL_SMALL,
		FIREBALL_MEDIUM,
		BULLETHIT_FRIENDLY,
		BULLETHIT_ENEMY
	}

	static ParticleEffectPool () {
		map = new SortedList<EffectType, ParticleEffectPool>();
	}

	void OnDestroy () {
		map.Remove(this.type);
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

	public void ReturnToInactivePool (PooledParticleEffect effect) {
		if(effect.gameObject.activeSelf){
			effect.gameObject.SetActive(false);
			activeEffects.Remove(effect);
			inactiveEffects.Add(effect);
		}
	}

	public PooledParticleEffect NewEffect (Vector3 position, Vector3 direction, bool onPlayArea) {
		Transform parent = (onPlayArea ? this.transform : null);
		int layer = (onPlayArea ? LayerMask.NameToLayer("Default") : LayerMask.NameToLayer("Background"));
		return NewEffect(position, direction, parent, layer);
	}

	public PooledParticleEffect NewEffect (Vector3 position, Vector3 direction, bool onPlayArea, int layer) {
		return NewEffect(position, direction, (onPlayArea ? this.transform : null), layer);
	}

	public PooledParticleEffect NewEffect (Vector3 position, Vector3 direction, Transform parent, int layer) {
		PooledParticleEffect effect;
		if(!TryTakeEffectFromInactivePool(out effect)){
			effectCount++;
			GameObject effectObject = Instantiate(effectPrefab) as GameObject;
			effectObject.name = "effect " + effectCount;
			effect = effectObject.GetComponent<PooledParticleEffect>();
			effect.pool = this;
		}
		effect.gameObject.SetActive(true);
		effect.gameObject.transform.parent = parent;
		effect.transform.position = position;
		effect.transform.localRotation = Quaternion.LookRotation(direction);
		effect.SetLayerIncludingAllChildren(effect.gameObject, layer);
		activeEffects.Add(effect);
		return effect;
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

	public static ParticleEffectPool GetPool(EffectType type){
		ParticleEffectPool output;
		if(map.TryGetValue(type, out output)){
			return output;
		}else{
			throw new UnityException("No pool in the map for type \"" + type.ToString() + "\". Maybe it wasn't instantiated?");
		}
	}

	void CheckAndSetInstance () {
		if(!map.ContainsKey(this.type)){
			map.Add(this.type, this);
		}else{
			throw new UnityException("There is already a pool in the map for type \"" + this.type.ToString() + "\" (Singleton violation)");
		}
	}

}
