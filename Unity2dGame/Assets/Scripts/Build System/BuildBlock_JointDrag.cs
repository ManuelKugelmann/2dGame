using UnityEngine;
using System.Collections;

public class BuildBlock_JointDrag : BuildBlockBase
{

	ConfigurableJoint joint;
	
	override protected void OnDragStart()
	{
		
		if (rigidbody != null) 
		{
			//rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY;
		}
		
		joint = MousePointer.I.GetComponent<ConfigurableJoint>();//   .gameObject.AddComponent<ConfigurableJoint>();
		
		/*
		//joint.anchor = activeDockingPoint.position;
		joint.anchor = new Vector3(0, 0, 0);
		//joint.anchor = this.transform.position;
		
		joint.xMotion = ConfigurableJointMotion.Locked;
		joint.yMotion = ConfigurableJointMotion.Locked;
		joint.zMotion = ConfigurableJointMotion.Locked;
		
	//	var limit = new SoftJointLimit();
	//	limit.limit = 0.5f;
	//	joint.linearLimit = limit;
		
		joint.angularXMotion = ConfigurableJointMotion.Locked;
		joint.angularYMotion = ConfigurableJointMotion.Locked;
		joint.angularZMotion = ConfigurableJointMotion.Locked;
		*/
		
		
		//joint.targetPosition = -mousePosition+this.transform.position;
		joint.targetPosition = new Vector3(0, 0, 0);
		
		joint.connectedBody = this.rigidbody;
	}
	
	
	override protected void OnDragging()
	{ }
	
	
	override protected void OnDragEnd()
	{
		//Destroy(joint);
		//joint = null;
		joint.connectedBody = null;
		
		//Destroy(this.rigidbody);
		
		if (rigidbody != null) 
		{
			rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		}
	}
	
	
}
