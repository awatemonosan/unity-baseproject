using UnityEngine;
using System.Collections;

public static class GUIExtra {
	public static void SetNativeResolution(float nativeWidth, float nativeHeight){
		//1920x1080
		float rx = Screen.width / nativeWidth;
		float ry = Screen.height / nativeHeight;
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (rx, ry, 1)); 
	}
}
