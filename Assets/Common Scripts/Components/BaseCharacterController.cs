using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------------------
public class BaseCharacterController : MonoBehaviour {
	public float speed = 10.0f;
	public float maxVelocityChange = 10.0f;
	public float gravity = 10.0f;
	public bool canJump = true;
	public float jumpHeight = 2.0f;
	public bool moveLocal=false;
	//----------------------------------------
	public Vector3 moveDirection;
	public Vector2 lookAngle;
	//----------------------------------------
	public void SetMoveLocal(Vector3 direction){
		moveDirection=direction;
		moveLocal=true;
	}
	public void SetMove(Vector3 move){
		moveDirection=move;
		moveLocal=false;
	}
	public Vector3 GetMove(){
		return moveDirection;
	}
	//----------------------------------------
	public void AddLookAngle(float pitch, float yaw){
		lookAngle.x+=pitch;
		lookAngle.y+=yaw;
	}
	
	public Quaternion GetLookRotation(){
		Quaternion result=Quaternion.Euler(0,lookAngle.y,0);
		result=result*Quaternion.Euler(lookAngle.x,0,0);
		return result;
	}
	
	public Vector3 GetLookForward(){
		return GetLookRotation()*Vector3.forward;
	}
	
	public Vector3 GetLookEuler(){
		return new Vector3(lookAngle.x,lookAngle.y);
	}
	
	public void SetLookEuler(Vector3 look){
		lookAngle=new Vector2(look.x,look.y);
	}
}
//--------------------------------------------------------------------------------

//Part of Awate's Standard Assets Pack