using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BuildBlockBase : MonoBehaviour
{
	//Public
	public float searchRange = 2.0F;
    public float alpha = 0.7F;
	
	//private
	protected Shader ghostShader;
	protected Shader normalShader;
    protected Color normalColor;
	protected Vector3 offset;
    protected Vector3 closestDockingPointVelocity = Vector3.zero;
    protected Vector3 mousePointVelocity = Vector3.zero;
    protected bool isBuildingBlockDragMode;
	
    protected Transform backingfield_closestDockingPoint;
    protected Transform closestDockingPoint
    {
        get
        {
            return backingfield_closestDockingPoint;
        }

        set
        {
            if (backingfield_closestDockingPoint != null)
            {
                backingfield_closestDockingPoint.gameObject.renderer.material.color = Color.red; // inactive
            }
			
            if (backingfield_closestDockingPoint != value)
            {
                backingfield_closestDockingPoint = value;
                closestDockingPointVelocity = Vector3.zero;
            }
			
            if (backingfield_closestDockingPoint != null)
            {
                backingfield_closestDockingPoint.gameObject.renderer.material.color = Color.yellow; // active
            }
        }
    }
    
	protected Transform backingfield_activeDockingPoint;
	protected Transform activeDockingPoint
	{
		get
		{
			return backingfield_activeDockingPoint;
		}
		
		set
		{
			if(backingfield_activeDockingPoint != null)
			{
				backingfield_activeDockingPoint.gameObject.renderer.material.color = Color.red; // inactive
			}
			
			backingfield_activeDockingPoint = value;
			
			if(backingfield_activeDockingPoint != null)
			{
				backingfield_activeDockingPoint.gameObject.renderer.material.color = Color.yellow; // active
			}
		}
	}
	
	
	public List<Transform> dockingPoints {get; protected set;}
	protected int activeDockingPointIdx = 0;
    protected bool isFirstMouseDown = true;
    protected bool isGui = true;
	
	//Mousebuttons
	const int leftButton = 0;
	const int rightButton = 1;
	
	// Use this for initialization
    
	
	void Start()
	{
		Init();
	}
	
	void Init()
	{
		
		Debug.Log("Try Init");
		
		if(dockingPoints != null)
			return;
		
		Debug.Log("Init");
		
        dockingPoints = new List<Transform>();
        foreach (Transform child in this.transform)
        {
            if (child.gameObject.layer == LayerMask.NameToLayer("Docking Point"))
            {
                dockingPoints.Add(child);
            }
        }
		
		ghostShader = Shader.Find("Transparent/Diffuse");
		normalShader = this.gameObject.renderer.material.shader;
        normalColor = this.gameObject.renderer.material.color;
	}
    
	


	void OnMouseDown ()
	{
		if(enabled)
		{
			Debug.Log("Mouse Down");
			StartDragging();
		}
	}
	
	public void StartDragging()
	{
		StartCoroutine(Dragging());
	}
	
	
	IEnumerator Dragging()
	{
		while(dockingPoints == null)// wait for newly instantiated blocks
		{
			Debug.Log("Wait...");
			yield return null;
		}
		
		
		isBuildingBlockDragMode = true;
		Ghost();
        Camera.main.cullingMask |= (1 << LayerMask.NameToLayer("Docking Point"));
   		

		activeDockingPoint = null;
		activeDockingPointIdx = -1;
		float minDistance = Mathf.Infinity;
		for(int i = 0; i < dockingPoints.Count; i++)
		{
			Transform dockingPoint = dockingPoints[i];
			if(CheckDistance(MousePointer.position, ref minDistance, dockingPoint.position))
			{
				activeDockingPoint = dockingPoint;
				activeDockingPointIdx = i;
			}			
		}

		// snap active docking point to mouseposition
		//this.transform.position = this.transform.position + (mousePosition - activeDockingPoint.position);
		// TODO: soft snap
		
		offset = activeDockingPoint.position - MousePointer.position;
	
		OnDragStart();
		
		while(Input.GetMouseButton(leftButton)) // while down
		{
			closestDockingPoint = GetClosestDockingPoint();
			
			/*
			if(Input.GetMouseButtonDown(rightButton) && dockingPoints.Count > 0) // on right click
			{ 
				activeDockingPointIdx += 1;
				if (activeDockingPointIdx >= dockingPoints.Count) 
				{
					activeDockingPointIdx = 0;
				}
				
				activeDockingPoint = (Transform)dockingPoints [activeDockingPointIdx];
				RotateToTargetPoint(mousePosition, ref mousePointVelocity);
				this.transform.position = this.transform.position + (mousePosition - activeDockingPoint.position);
				offset = this.transform.position - mousePosition;
			}
			*/

			OnDragging();
			
			yield return null;
		}
		
		UnGhost();
        Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("Docking Point")); 
        
		activeDockingPoint = null;
		isBuildingBlockDragMode = false;
		
		OnDragEnd();
		
	}
	
	
	abstract protected void OnDragStart();
	
	abstract protected void OnDragging();
	
	abstract protected void OnDragEnd();
	
	
	
	
	
	
	private void RotateToTargetPoint(Vector3 targetPoint, ref Vector3 velocity)
	{
		
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

        /*
        var stepRotation = Quaternion.AngleAxis(angle * Time.deltaTime * dockingPointPullStrength, Vector3.forward);
        var stepPosition = finalPositionMove * Time.deltaTime * dockingPointPullStrength;


        this.transform.rotation *= stepRotation;
        this.transform.position += stepPosition;

        //this.transform.position = Vector3.SmoothDamp(this.transform.position, targetPoint,ref velocity,0.5F);

        //offset = stepRotation * offset;
        //offset += stepPosition;


        //this.transform.RotateAround(targetPoint,Vector3.forward,angle);// target point
        //this.transform.RotateAround(activeDockingPoint.position,Vector3.forward,angle);// own docking point

        this.transform.rotation *= finalRotationMove;
        this.transform.position = finalPositionMove;
        */
		
	}
	
	
	

	
	void OnDrawGizmosSelected ()
	{
        if (isBuildingBlockDragMode)
        {
            Gizmos.color = Color.yellow;
            if (activeDockingPoint != null)
            {
                Gizmos.DrawWireSphere(activeDockingPoint.position, searchRange);
            }
        }
	}
	
	 void OnTriggerEnter ()
	{
        var redColor = Color.red;
        redColor.a = alpha;
		this.gameObject.renderer.material.color=redColor;	
	}
	
	void OnTriggerExit ()
	{
        var whiteColor = Color.white;
        whiteColor.a = alpha;
		this.gameObject.renderer.material.color = whiteColor;	
	}

	private void Ghost ()
	{
		this.gameObject.collider.isTrigger = true;
		this.gameObject.renderer.material.shader = ghostShader;
		Color newColor = this.gameObject.renderer.material.color;
		newColor.a = alpha;
		this.gameObject.renderer.material.color = newColor;
	}
	
	private void UnGhost ()
	{
		this.gameObject.collider.isTrigger = false;
		this.gameObject.renderer.material.shader = normalShader ;
        this.gameObject.renderer.material.color = normalColor ;
	}
	
	protected float signedAngle(Vector2 v1, Vector2 v2)
	{
	      float perpDot = v1.x * v2.y - v1.y * v2.x;
	      return Mathf.Rad2Deg * Mathf.Atan2(perpDot, Vector2.Dot(v1, v2));
		
		  //return - Mathf.Rad2Deg * (Mathf.Atan2(v1.y,v1.x) - Mathf.Atan2(v2.y,v2.x)); // alternative calculation
	}
		
	

	
	
	private Transform GetClosestDockingPoint()
	{
		if(activeDockingPoint == null)
			return null;
		
		Transform closestDockingPoint = null;
		
		Collider[] colliders = Physics.OverlapSphere(activeDockingPoint.position, searchRange, 1 << LayerMask.NameToLayer ("Docking Point"));
		
		if (colliders.Length > 0)
		{
			float minDistance = Mathf.Infinity;
			
			foreach (Collider c in colliders)
			{
				if (c.transform.parent != null && c.transform.parent.gameObject != this.gameObject) 
				{
					if (CheckDistance (activeDockingPoint.position, ref minDistance, c.transform.position)) 
					{
						closestDockingPoint = c.transform;
					}
				}
			}
			
			if (closestDockingPoint != null) 
			{
				Debug.DrawLine (activeDockingPoint.position, closestDockingPoint.position);
			}
		}
		
		return closestDockingPoint;
	}
	
	
	private bool CheckDistance (Vector3 start, ref float minDistance, Vector3 end)
	{
		float distance = Vector3.Distance (start, end);
		if (distance < minDistance) {
			minDistance = distance;
			return true;
		}
		return false;
	}
	
}
