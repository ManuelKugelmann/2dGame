using UnityEngine;
using System.Collections;

public class BuildBlock : MonoBehaviour
{
	//Public
	public float searchRange = 2.0F;
	
	//private
	private Shader ghostShader;
	private Shader normalShader;
	private Vector3 offset;
	private Transform activeDockingPoint;
	private ArrayList dockingPoints;
	private bool leftMouseDown = false;
	private int currentActive = 0;
	
	//Mousebuttons
	private static int leftButton = 0;
	private static int rightButton = 1;
	
	// Use this for initialization
	void Start ()
	{
		dockingPoints = new ArrayList ();
		ghostShader = Shader.Find("Transparent/Diffuse");
		normalShader = this.gameObject.renderer.material.shader;
	}

	// Update is called once per frame
	void Update ()
	{
		if (leftMouseDown && Input.GetMouseButtonDown (rightButton) && dockingPoints.Count > 0) {
			currentActive += 1;
			if (currentActive >= dockingPoints.Count) {
				currentActive = 0;
			}
			activeDockingPoint = (Transform)dockingPoints [currentActive];
			var mousePosition = GetMousePositionAtNullPlane ();
			RotateToTargetPoint(mousePosition);
			this.transform.position = this.transform.position + (mousePosition - activeDockingPoint.position);
			offset = this.transform.position - mousePosition;
		}
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
		Transform closest = null;
		leftMouseDown = true;
		var mousePosition = GetMousePositionAtNullPlane ();

		float minDistance = Mathf.Infinity;
		foreach (Transform child in this.transform) {
			if (child.gameObject.layer == LayerMask.NameToLayer ("Docking Point")) {
				dockingPoints.Add (child);
				if (CheckDistance (mousePosition, ref minDistance, child.position)) {
					closest = child;
				}
			}
		}
		activeDockingPoint = closest;
		currentActive = dockingPoints.IndexOf (activeDockingPoint);
		this.transform.position = this.transform.position + (mousePosition - activeDockingPoint.position);
		offset = this.transform.position - mousePosition;
        
	}

	void OnMouseDrag ()
	{      
		if (rigidbody != null) {
			rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		}
		Vector3 mousePosition = GetMousePositionAtNullPlane ();
		this.transform.position = mousePosition+ offset;
		
		Transform closestDockingPoint = GetClosestDockingPoint (); 
		if(closestDockingPoint != null)
		{
			RotateToTargetPoint(closestDockingPoint.position);
		}
	}

	void OnMouseUp ()
	{
		UnGhost();
		leftMouseDown = false;
		if (rigidbody != null) {
			rigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
		}
	}

	void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.yellow;
		if (activeDockingPoint != null) {
			Gizmos.DrawWireSphere (activeDockingPoint.position, searchRange);
		}
	}

	private void Ghost ()
	{
		this.gameObject.collider.isTrigger = true;
		this.gameObject.renderer.material.shader = ghostShader;
		Color newColor = this.gameObject.renderer.material.color;
		newColor.a = 0.5F;
		this.gameObject.renderer.material.color = newColor;
	}
	
	private void UnGhost ()
	{
		this.gameObject.collider.isTrigger = false;
		Color newColor = this.gameObject.renderer.material.color;
		newColor.a = 1;
		this.gameObject.renderer.material.color = newColor;
		this.gameObject.renderer.material.shader = normalShader;
	}
	
	
	protected float signedAngle(Vector2 v1, Vector2 v2)
	{
	      float perpDot = v1.x * v2.y - v1.y * v2.x;
	      return Mathf.Rad2Deg * Mathf.Atan2(perpDot, Vector2.Dot(v1, v2));
		
		  //return - Mathf.Rad2Deg * (Mathf.Atan2(v1.y,v1.x) - Mathf.Atan2(v2.y,v2.x)); // alternative calculation
	}
		
	
	private void RotateToTargetPoint(Vector3 targetPoint)
	{
		Vector3	activeDockingPointDir = Vector3.Normalize(-this.transform.position + activeDockingPoint.position);
		Vector3 targetPointDir = Vector3.Normalize(-this.transform.position + targetPoint);
		float angle = signedAngle(activeDockingPointDir,targetPointDir);
		this.transform.rotation *= Quaternion.AngleAxis(angle,Vector3.forward);
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
