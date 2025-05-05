using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CameraFellow))]
public class CameraFellowEditor : Editor {

	public override void OnInspectorGUI() {

		DrawDefaultInspector ();

		CameraFellow cf = (CameraFellow)target;
		if (GUILayout.Button ("Set Min Cam Pos")) {
			cf.SetMinCameraPos ();
		}

		if (GUILayout.Button ("Set Max Cam Pos")) {
			cf.SetMaxCameraPos ();
		}
	}
}
