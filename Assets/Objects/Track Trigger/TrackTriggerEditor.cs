using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TrackTrigger))]
public class TrackTriggerEditor : Editor {

	public override void OnInspectorGUI () {
		DrawDefaultInspector();
		TrackTrigger tt = target as TrackTrigger;
		if(GUILayout.Button("Apply Position")){
			tt.ApplyPosition();
		}
		if(GUILayout.Button("Snap To Curve")){
			tt.SnapToCurve();
		}
		if(GUILayout.Button("Debug Thing")){
			tt.DebugThing();
		}
	}

}
