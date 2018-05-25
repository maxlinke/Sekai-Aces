using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class PlayAreaScript : MonoBehaviour {

	[SerializeField] BoxCollider coll;

	static PlayAreaScript instance;

	void Start () {
		if(instance != null) throw new UnityException("singleton violation");
		instance = this;
		GetComponent<MeshRenderer>().enabled = false;
		this.enabled = false;
	}
	
	void Update () {
		
	}

	public static Vector3 ProjectTowardsCamera(Vector3 point){
		return instance.CamProject(point);
	}

	public static Vector3 GetDimensions(){
		return instance.Dimensions();
	}

	Vector3 Dimensions(){
		Vector3 cs = coll.size;
		Vector3 ls = transform.localScale;
		return new Vector3(cs.x * ls.x, cs.y * ls.y, cs.z * ls.z);
	}

	Vector3 CamProject(Vector3 point){
		Vector3 projectedAlongNormal = transform.position + Vector3.ProjectOnPlane(point - transform.position, transform.up);
		Vector3 difference = projectedAlongNormal - point;
		if(difference == Vector3.zero){
			return point;
		}else{
			Vector3 toCamera = Camera.main.transform.position - point;
			//float vecLength = ((Vector3.Dot(difference, difference) * toCamera.magnitude) / Vector3.Dot(toCamera, difference));
			//return (point + (toCamera.normalized * vecLength));
			float x = (Vector3.Dot(difference, difference) / Vector3.Dot(toCamera, difference));
			return (point + (toCamera * x));
		}
	}
}
