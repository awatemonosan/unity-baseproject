using UnityEngine;
using System.Collections;
//TODO: Make BaseCharacterController hold the speed, gravity, jump height etc attributes
//--------------------------------------------------------------------------------
public class RigidBodyCharacterController : BaseCharacterController {
	public bool freezeRotation=false;
	private bool grounded = false;
	public Vector3 groundNormal=Vector3.up;
	
	//----------------------------------------
	
	float CalculateJumpVerticalSpeed () {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}
	
	void FixedUpdate () {
		rigidbody.freezeRotation = true;
		rigidbody.useGravity = false;
		if(!freezeRotation)
			transform.rotation=Quaternion.Euler(0,GetLookEuler().y,0);
		
		float finalSpeed=grounded ? speed : speed*0.25f;
		float finalMaxVelocityChange=grounded ? maxVelocityChange : maxVelocityChange*0.01f;
		
		// Calculate how fast we should be moving
		Vector3 targetVelocity = GetMove().normalized;
		if(moveLocal){
			targetVelocity=GetLookRotation()*targetVelocity;
		}
		targetVelocity *= speed;
		targetVelocity = Math3d.ProjectPointOnPlane(groundNormal,Vector3.zero,targetVelocity);
		
		// Apply a force that attempts to reach our target velocity
		Vector3 velocity = rigidbody.velocity;
		Vector3 velocityChange = (targetVelocity - velocity);
		velocityChange.x = Mathf.Clamp(velocityChange.x, -finalMaxVelocityChange, finalMaxVelocityChange);
		velocityChange.z = Mathf.Clamp(velocityChange.z, -finalMaxVelocityChange, finalMaxVelocityChange);
		velocityChange.y = 0;
		rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
		
		if (grounded) {
			if (canJump && moveDirection.y>0) {
				rigidbody.velocity = Math3d.ProjectPointOnPlane(groundNormal,Vector3.zero,rigidbody.velocity)+groundNormal*CalculateJumpVerticalSpeed();
			}
		}
		
		//transform.rotation.SetLookRotation(Quaternion.Euler(0,lookDirection.y,0)*Vector3.forward,Vector3.up);
		// We apply gravity manually for more tuning control
		rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
		
		grounded = false;
		groundNormal=Vector3.up;
	}
	
	void OnCollisionStay (Collision collisionInfo) {
		groundNormal=Vector3.zero;
		foreach(ContactPoint contact in collisionInfo.contacts){
			if(contact.normal.y>0.5f)
				groundNormal+=contact.normal;
		}
		groundNormal.Normalize();
		if(groundNormal==Vector3.zero)
			groundNormal=Vector3.up;
		grounded = groundNormal.y>0.5f;
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if(stream.isReading){
			transform.position=(Vector3)stream.ReceiveNext();
			rigidbody.velocity=(Vector3)stream.ReceiveNext();
			SetMove((Vector3)stream.ReceiveNext());
			SetLookEuler((Vector3)stream.ReceiveNext());
		}
		else if(stream.isWriting){
			stream.SendNext(transform.position);
			stream.SendNext(rigidbody.velocity);
			stream.SendNext(GetMove());
			stream.SendNext(GetLookEuler());
		}
	}
}
//--------------------------------------------------------------------------------

//Part of Awate's Standard Assets Pack
