using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpeedChangeTrackTrigger))]
public class SpeedChangeTrackTriggerEditor : Editor {

	public override void OnInspectorGUI (){
		DrawDefaultInspector();
		TrackTrigger tt = target as TrackTrigger;
		TrackTriggerEditor.DrawCustomElements(tt);
	}
}
