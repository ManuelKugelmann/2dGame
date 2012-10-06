using UnityEngine;
using System.Collections;

public class BuildBlock_SnapDrag : BuildBlockBase
{
	
	
	
	
	override protected void OnDragStart()
	{ }
	
	
	override protected void OnDragging()
	{
		if(closestDockingPoint == null || (MousePointer.position+offset - activeDockingPoint.position).magnitude > 6 ) 
		{
			
			this.transform.position = this.transform.position + (MousePointer.position+offset - activeDockingPoint.position);
			
		}
		else
		{
			
			var targetPoint = closestDockingPoint.position;
			
			var diffRotation = closestDockingPoint.rotation * Quaternion.Inverse(activeDockingPoint.localRotation);
			var targetRotation =  diffRotation * Quaternion.AngleAxis(180,Vector3.forward);
			
			this.transform.rotation = targetRotation;
			this.transform.position = targetPoint - (targetRotation*(activeDockingPoint.position*this.transform.localScale));
			
			/*
			Vector3 activeDockingPointRelativePosition = -this.transform.position + activeDockingPoint.position; // NOTE: could use local position if docking points are always direct children
			float angle = 0;
			
			if((targetPoint-activeDockingPoint.position).magnitude > 0.01f)
			{
				Vector3	activeDockingPointDir = Vector3.Normalize(activeDockingPointRelativePosition);
				Vector3 targetPointDir = Vector3.Normalize(-this.transform.position + targetPoint);
				angle = signedAngle(activeDockingPointDir,targetPointDir);
			}
			
			var finalRotationMove = Quaternion.AngleAxis(angle,Vector3.forward);
			
			var finalActiveDockingPointRelativePosition = finalRotationMove*activeDockingPointRelativePosition; //rotate relativeVector
			var finalActiveDockingPointPosition = this.transform.position +finalActiveDockingPointRelativePosition; // final Position
			
			var finalPositionMove = targetPoint - finalActiveDockingPointPosition;
	        
			this.transform.rotation *= Quaternion.AngleAxis(angle,Vector3.forward);
	        this.transform.position += finalPositionMove;
			
			offset = finalRotationMove * offset;
          
            this.transform.position = this.transform.position + (MousePointer.position+offset - activeDockingPoint.position);
			
            //RotateToTargetPoint(closestDockingPoint.position, ref closestDockingPointVelocity);
            */
		}
		
	}



	

	override protected void OnDragEnd()
	{
		
	}
	
	
	
	
	
	
	
}
