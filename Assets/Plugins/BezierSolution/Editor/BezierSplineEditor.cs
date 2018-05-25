using UnityEngine;
using UnityEditor;

namespace BezierSolution
{
	[CustomEditor( typeof( BezierSpline ) )]
	public class BezierSplineEditor : Editor
	{
		private BezierSpline spline;

		void OnEnable()
		{
			spline = target as BezierSpline;
			spline.Refresh();

			Undo.undoRedoPerformed -= OnUndoRedo;
			Undo.undoRedoPerformed += OnUndoRedo;
		}

		void OnDisable()
		{
			Undo.undoRedoPerformed -= OnUndoRedo;
		}

		void OnSceneGUI()
		{
			BezierUtils.DrawSplineDetailed( spline );
			DrawVectors();

			for( int i = 0; i < spline.Count; i++ )
				BezierUtils.DrawBezierPoint( spline[i], i + 1, false );
		}

		public override void OnInspectorGUI()
		{
			BezierUtils.DrawSplineInspectorGUI( spline );
		}

		//added
		void DrawVectors(){
			Color beforeColor = Handles.color;
			int lineCount = spline.Count * 10;
			float f_lineCount = (float)lineCount;
			for(int i=0; i<lineCount; i++){
				float t = ((float)i / f_lineCount);
				Vector3 point = spline.GetPoint(t);
				Quaternion rotation = spline.GetRotation(t);
				Vector3 up = rotation * Vector3.up;
				Handles.color = Color.green;
				Handles.DrawLine(point, point + (up * 0.2f));
				Vector3 forward = rotation * Vector3.forward;
				Handles.color = Color.blue;
				Handles.DrawLine(point, point + (forward * 0.2f));
				Vector3 right = rotation * Vector3.right;
				Handles.color = Color.red;
				Handles.DrawLine(point, point + (right * 0.2f));
			}
			Handles.color = beforeColor;
		}

		private void OnUndoRedo()
		{
			if( spline != null && !spline.Equals( null ) )
				spline.Refresh();

			Repaint();
		}

		private bool HasFrameBounds()
		{
			return true;
		}

		private Bounds OnGetFrameBounds()
		{
			return new Bounds( spline.transform.position, Vector3.one );
		}
	}
}