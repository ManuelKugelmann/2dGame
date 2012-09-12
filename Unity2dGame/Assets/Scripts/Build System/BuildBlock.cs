using UnityEngine;
using System.Collections;

public class BuildBlock : MonoBehaviour
{
	//Public
	public float searchRange = 2.0F;
    public float mousePullStrength = 10F;
    public float dockingPointPullStrength = 20F;
    public float alpha = 0.7F;
	
	//private
	private Shader ghostShader;
	private Shader normalShader;
    private Color normalColor;
	private Vector3 offset;
    private Vector3 closestDockingPointVelocity = Vector3.zero;
    private Vector3 mousePointVelocity = Vector3.zero;
    private bool isBuildingBlockDragMode;
    private Transform backingfield_closestDockingPoint;
    private Transform closestDockingPoint
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
    
	private Transform backingfield_activeDockingPoint;
	private Transform activeDockingPoint
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
	
	
	private ArrayList dockingPoints;
	
	private int activeDockingPointIdx = 0;
	
	//Mousebuttons
	private static int leftButton = 0;
	private static int rightButton = 1;
	
	// Use this for initialization
	void Start ()
	{
		dockingPoints = new ArrayList ();
		ghostShader = Shader.Find("Transparent/Diffuse");
		normalShader = this.gameObject.renderer.material.shader;
        normalColor = this.gameObject.renderer.material.color;
	}

	// Update is called once per frame
	void Update ()
	{
		// Lock y rotation
		// TODO: look for something more efficient
		//var r = this.transform.rotation.eulerAngles;
		//r.y = 0;
		//this.transform.rotation = Quaternion.Euler(r);
		
		if (isBuildingBlockDragMode) {

			var mousePosition = GetMousePositionAtNullPlane ();
			
			/*
			if(Input.GetMouseButtonDown (rightButton) && dockingPoints.Count > 0) { // when righclicked
				
				activeDockingPointIdx += 1;
				if (activeDockingPointIdx >= dockingPoints.Count) {
					activeDockingPointIdx = 0;
				}
				
				activeDockingPoint = (Transform)dockingPoints [activeDockingPointIdx];
				RotateToTargetPoint(mousePosition, ref mousePointVelocity);
				this.transform.position = this.transform.position + (mousePosition - activeDockingPoint.position);
				offset = this.transform.position - mousePosition;
			}
			*/
					
			closestDockingPoint = GetClosestDockingPoint ();
			
			if(closestDockingPoint == null) {
				
				this.transform.position = this.transform.position + (mousePosition + offset - activeDockingPoint.position);
				
			}
			else {
				
				var targetPoint = closestDockingPoint.position;
				
						
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

              
                this.transform.position = this.transform.position + (mousePosition - activeDockingPoint.position);
				
                //RotateToTargetPoint(closestDockingPoint.position, ref closestDockingPointVelocity);
			}
		}
	}
	
	
	
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
	
	
	
	public static Vector3 GetMousePositionAtNullPlane ()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		float lambda = -ray.origin.z / ray.direction.z;
		Debug.DrawRay (ray.origin, ray.direction * lambda, Color.yellow);
		return ray.origin + ray.direction * lambda;
	}

	void OnMouseDown ()
	{
		Ghost();
		
		if (rigidbody != null) {
			rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		}
		
		var mousePosition = GetMousePositionAtNullPlane ();
		
		Transform closest = null;
		float minDistance = Mathf.Infinity;
        // get all Dockingpoints on this object
		foreach (Transform child in this.transform) {
			if (child.gameObject.layer == LayerMask.NameToLayer ("Docking Point")) {
				dockingPoints.Add (child);
				if (CheckDistance (mousePosition, ref minDistance, child.position)) {
					closest = child;
				}
			}
		}
		activeDockingPoint = closest;
		activeDockingPointIdx = dockingPoints.IndexOf (activeDockingPoint);
		
		// snap active docking point to mouseposition
		// TODO: soft snap
		this.transform.position = this.transform.position + (mousePosition - activeDockingPoint.position);
		
		offset = activeDockingPoint.position - mousePosition;
		
    	isBuildingBlockDragMode = true;
		
		Debug.Log("Mouse Down");
	}
	
	/*
	void OnMouseDrag ()
	{      
		
		Vector3 mousePosition = GetMousePositionAtNullPlane ();
		
		//offset -= offset*Time.deltaTime; //slide to center
		// pull towards mousePointer
		var move = mousePosition+offset - this.transform.position;

		this.transform.position += move * Time.deltaTime*mousePullStrength;
		
		
		closestDockingPoint = GetClosestDockingPoint (); 
	}
	*/

	void OnMouseUp ()
	{
		UnGhost();
        Destroy(this.rigidbody);
        activeDockingPoint = null;
		isBuildingBlockDragMode = false;
		
		Debug.Log("Mouse Up");
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
		
	

	
	
	private Transform GetClosestDockingPoint ()
	{
		Transform closestDockingPoint = null;
		Collider[] colliders = Physics.OverlapSphere (activeDockingPoint.position, searchRange, 1 << LayerMask.NameToLayer ("Docking Point"));
		if (colliders.Length > 0) {
			float minDistance = Mathf.Infinity;
			foreach (Collider c in colliders) {
				if (c.transform.parent.gameObject != this.gameObject) {
					if (CheckDistance (activeDockingPoint.position, ref minDistance, c.transform.position)) {
						closestDockingPoint = c.transform;
					}
				}
			}
			if (closestDockingPoint != null) {
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
