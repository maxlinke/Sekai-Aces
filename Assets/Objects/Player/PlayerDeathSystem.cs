using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathSystem : MonoBehaviour {

	[SerializeField] ParticleSystem fireParticleSystem;
	[SerializeField] Player pc;
	[SerializeField] Rigidbody rb;
	[SerializeField] Collider hitbox;

	[HideInInspector] public PlayArea playArea;
	[HideInInspector] public LevelTrackFollower levelTrackFollower;
	[HideInInspector] public PlayerModel playerModel;

	ParticleEffectPool fireballPool;
	bool running;
	bool exploded;

	void Start () {
		fireballPool = ParticleEffectPool.GetFireballPoolMedium();
	}
	
	void Update () {
		
	}

	void OnCollisionEnter(Collision collision){
		if(running){
			Explode();
		}
	}

	public void RespawnReset(){
		fireParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		running = false;
		exploded = false;
	}

	public void InitiateCrash(){
		if(this.gameObject.layer != LayerMask.NameToLayer("Background")) TransferToBackground();
		fireParticleSystem.Play(true);
		running = true;
	}

	public void Explode(){
		if(this.gameObject.layer != LayerMask.NameToLayer("Background")) TransferToBackground();
		fireballPool.NewEffect(transform.position, transform.forward, false, gameObject.layer);
		fireParticleSystem.Stop(true);
		playerModel.Hide();
		hitbox.enabled = false;
		running = false;
		exploded = true;
	}

	void TransferToBackground(){
		pc.SetLayerIncludingAllChildren(pc.gameObject, LayerMask.NameToLayer("Background"));
		transform.parent = null;
		transform.position = playArea.TransformPointFromPlayAreaToLevel(transform.position);
		Vector3 newForward = playArea.TransformDirectionFromPlayAreaToLevel(transform.forward);
		Vector3 newUp = playArea.TransformDirectionFromPlayAreaToLevel(transform.up);
		transform.rotation = Quaternion.LookRotation(newForward, newUp);
		rb.velocity = levelTrackFollower.GetVelocity();
		rb.useGravity = true;
		rb.constraints = RigidbodyConstraints.None;
	}

	public bool IsExploded(){
		return exploded;
	}

}
