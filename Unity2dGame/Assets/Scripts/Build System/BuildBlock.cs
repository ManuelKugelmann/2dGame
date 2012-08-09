using UnityEngine;
using System.Collections;

public class BuildBlock : MonoBehaviour 
{
    private Vector3 offset;
	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

	public static Vector3 GetMousePositionAtNullPlane ()
	{
		Ray ray =Camera.main.ScreenPointToRay(Input.mousePosition);
		float lambda = Mathf.Abs(ray.origin.z)/ray.direction.z;
		Debug.DrawRay(ray.origin, ray.direction * lambda, Color.yellow);
		return ray.direction * lambda;
	}
	
	void OnMouseDown () 
    {
		Vector3 position = this.transform.position;
        offset = position - GetMousePositionAtNullPlane ();
        
	}

    void OnMouseDrag()
    {
        this.transform.position = GetMousePositionAtNullPlane()+ offset;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
       
    }

    void OnMouseUp()
    {
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
    }
}
