using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour {
	
		[Header("Components")]
	[SerializeField] GameObject boidModelPrefab;
	[SerializeField] BoxCollider boxColl;

		[Header("Settings")]
	[SerializeField] int numberOfBoids;
	[SerializeField] float boidMaxSpeed;
	[SerializeField] float boidMinSpeed;
	[SerializeField] float boidMaxAccel;
	[SerializeField] float targetShuffleInterval;
	[Range(0f, 1f)] [SerializeField] float percentageAfflictedPerShuffle;
	[Range(0f, 1f)] [SerializeField] float chanceForTargetingOtherBoid;

		[Header("Behavior Parameters")]
	[SerializeField] float avoidDistance;
	[SerializeField] float avoidImportance;
	[SerializeField] float stickToBoxImportance;
	[SerializeField] float targetImportance;

	GameObject[] targets;
	GameObject[] boids;
	Vector3[] velocities;

	float nextShuffle;

	void Start(){
		boids = new GameObject[numberOfBoids];
		velocities = new Vector3[numberOfBoids];
		SpawnBoids();
		targets = new GameObject[numberOfBoids];
		InitiateTargets();
		nextShuffle = Time.time + targetShuffleInterval;
	}
	
	void Update(){
		for(int i=0; i<boids.Length; i++){
			GameObject boid = boids[i];
			Vector3 velocity = velocities[i];
			float[] distances = CalculateDistances(i);

			Vector3 accel = Vector3.zero;
			accel += StayInBoxVector(i) * stickToBoxImportance;
			accel += ToTargetVector(i) * targetImportance;
			accel += AvoidNearbyVector(i, distances) * avoidImportance;
			accel = ClampVector(accel, 0f, boidMaxAccel);

			velocity += accel * Time.deltaTime;
			velocity = ClampVector(velocity, boidMinSpeed, boidMaxSpeed);

			Vector3 upVector = boid.transform.up + (CalculateUpVector(velocities[i], velocity) * Time.deltaTime);
			boid.transform.rotation = Quaternion.LookRotation(velocity, upVector);
			boid.transform.position += velocity * Time.deltaTime;
			velocities[i] = velocity;
		}
		if(Time.time > nextShuffle){
			ReshuffleTargets();
			nextShuffle = Time.time + targetShuffleInterval;
		}
	}

	void SpawnBoids(){
		for(int i=0; i<boids.Length; i++){
			GameObject boid = Instantiate(boidModelPrefab, this.transform) as GameObject;
			Vector3 velocity = Random.insideUnitSphere.normalized * boidMaxSpeed;
			boid.transform.position = this.transform.position + MultiplyElementWise(Random.insideUnitSphere, boxColl.size / 2f);
			boid.transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);
			boids[i] = boid;
			velocities[i] = velocity;
		}
	}

	void InitiateTargets(){
		for(int i=0; i<targets.Length; i++){
			if(Random.value < chanceForTargetingOtherBoid){
				targets[i] = boids[Random.Range(0, numberOfBoids)];
			}
		}
	}

	void ReshuffleTargets(){
		for(int i=0; i<targets.Length; i++){
			if(Random.value < percentageAfflictedPerShuffle){
				if(Random.value < chanceForTargetingOtherBoid){
					targets[i] = boids[Random.Range(0, numberOfBoids)];
				}else{
					targets[i] = null;
				}
			}
		}
	}

	float[] CalculateDistances(int thisBoidIndex){
		GameObject thisBoid = boids[thisBoidIndex];
		float[] distances = new float[boids.Length];
		for(int i=0; i<boids.Length; i++){
			distances[i] = (thisBoid.transform.position - boids[i].transform.position).magnitude;
		}
		return distances;
	}

	Vector3 MultiplyElementWise(Vector3 a, Vector3 b){
		return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}

	Vector3 ClampVector(Vector3 vec, float min, float max){
		if(vec.magnitude < min) return vec.normalized * min;
		if(vec.magnitude > max) return vec.normalized * max;
		return vec;
	}

	Vector3 CalculateUpVector(Vector3 lastVelocity, Vector3 newVelocity){
		Vector3 deltaV = (newVelocity - lastVelocity) / Time.deltaTime;
		return Vector3.up + (20f * deltaV);
	}

	Vector3 ToTargetVector(int thisBoidIndex){
		GameObject thisBoid = boids[thisBoidIndex];
		GameObject target = targets[thisBoidIndex];
		if(target != null){
			Debug.DrawLine(thisBoid.transform.position, target.transform.position, Color.green);
			return target.transform.position - thisBoid.transform.position;
		}else{
			return Vector3.zero;
		}
	}

	Vector3 AvoidNearbyVector(int thisBoidIndex, float[] distances){
		GameObject thisBoid = boids[thisBoidIndex];
		Vector3 avoid = Vector3.zero;
		for(int i=0; i<boids.Length; i++){
			if(i != thisBoidIndex){
				if(distances[i] < avoidDistance){
					avoid += ((thisBoid.transform.position - boids[i].transform.position) * ((avoidDistance - distances[i]) / avoidDistance));
					Debug.DrawLine(thisBoid.transform.position, boids[i].transform.position, Color.red);
				}
			}
		}
		return avoid;
	}

	Vector3 StayInBoxVector(int thisBoidIndex){
		GameObject thisBoid = boids[thisBoidIndex];
		Vector3 lPos = thisBoid.transform.localPosition;
		Vector3 absLPos = new Vector3(Mathf.Abs(lPos.x), Mathf.Abs(lPos.y), Mathf.Abs(lPos.z));
		Vector3 backIn = Vector3.zero;
		if(Mathf.Clamp01(absLPos.x - boxColl.size.x/2f) > 0){
			backIn.x = -Mathf.Sign(lPos.x);
		}
		if(Mathf.Clamp01(absLPos.y - boxColl.size.y/2f) > 0){
			backIn.y = -Mathf.Sign(lPos.y);
		}
		if(Mathf.Clamp01(absLPos.z - boxColl.size.z/2f) > 0){
			backIn.z = -Mathf.Sign(lPos.z);
		}
		return backIn;
	}

}
