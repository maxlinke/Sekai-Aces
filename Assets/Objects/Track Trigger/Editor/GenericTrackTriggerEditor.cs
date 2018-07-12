using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenericTrackTrigger))]
public class GenericTrackTriggerEditor : Editor {

	public override void OnInspectorGUI (){
		DrawDefaultInspector();
		TrackTrigger tt = target as TrackTrigger;
		TrackTriggerEditor.DrawCustomElements(tt);
	}
}
