using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RPNBehavior))]
public class RPN_Inspector : Editor {
	private string cmd;
	
	private bool showCMD=true;
	private bool showCode=true;
	
	private bool showDebug=false;
	
	public override void OnInspectorGUI()
	{
		EditorUtility.SetDirty( target );
		RPNBehavior rpn = (RPNBehavior)target;
		
		EditorGUILayout.BeginHorizontal();
		if(rpn.IsRunning ()){ if(GUILayout.Button("Stop")) rpn.Stop ();}
		else {if(GUILayout.Button("Start")) rpn.Run ();}
		if(GUILayout.Button("Step")) rpn.Step();
		GUILayout.Box (rpn.GetPoint ().ToString());
		EditorGUILayout.LabelField("CycleSpeed: ");
		rpn.CyclesPerFrame=EditorGUILayout.IntField(rpn.CyclesPerFrame);
		EditorGUILayout.EndHorizontal();
		
		if(showCMD=EditorGUILayout.Foldout(showCMD,"Command Line")){
			EditorGUILayout.BeginHorizontal();
			cmd=EditorGUILayout.TextArea(cmd);
			if(GUILayout.Button("Execute"))
				rpn.Execute(cmd);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.HelpBox(rpn.GetConsole(),MessageType.None);
		}
		if(showCode=EditorGUILayout.Foldout(showCode,"Source Code")){
			string newCode = EditorGUILayout.TextField(rpn.GetCode());
			if(newCode!=rpn.GetCode())
				rpn.LoadCode(newCode);
		}
		if(showDebug=EditorGUILayout.Foldout(showDebug,"DEBUG")){
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField("RAM");
			foreach(string key in rpn.GetRAMKeys()){
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(key);
				rpn.Set (key,GUILayout.TextField(rpn.Get (key)));
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField("STACK");
			foreach(string value in rpn.GetStack()){
				GUILayout.Label(value);
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}
	}
}
