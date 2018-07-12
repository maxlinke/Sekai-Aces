using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TrackTrigger))]
public class TrackTriggerEditor : Editor {

	public override void OnInspectorGUI () {
		DrawDefaultInspector();
		TrackTrigger tt = target as TrackTrigger;
		DrawCustomElements(tt);
	}

	public static void DrawCustomElements (TrackTrigger tt) {
		if(tt.curve != null){
			if(GUILayout.Button("Apply Position")){
				Undo.RecordObject(tt.transform, "Apply Position (TrackTrigger)");
				tt.ApplyPosition();
			}
			if(GUILayout.Button("Snap To Curve")){
				Undo.RecordObject(tt.transform, "Snap To Curve (TrackTrigger)");
				tt.SnapToCurve();
			}
		}
	}

}
