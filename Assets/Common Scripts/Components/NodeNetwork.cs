using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeNetwork : MonoBehaviour {
	public List<NodeNetwork> connections;
	public Color color=Color.cyan;
	
	void Start () {
		foreach(NodeNetwork node in connections){
			if(!node.connections.Contains(this))
				node.connections.Add (this);
		}
	}
	
	void OnDrawGizmos(){
		foreach(NodeNetwork node in connections){
			Gizmos.color=color;
			Gizmos.DrawLine(transform.position,node.transform.position);
		}
	}
}
