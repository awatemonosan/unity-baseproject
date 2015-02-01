using UnityEngine;
using System.Collections;

static public class GameObject_Extensions {
	public static void SetExclusiveChild(this Transform t, string name){
		for(int i = 0; i<t.childCount; i++)
			t.GetChild(i).gameObject.SetActive(false);
		t.FindChild(name).gameObject.SetActive(true);
	}
}
