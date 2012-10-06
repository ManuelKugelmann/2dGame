using UnityEngine;
using System.Collections;

public class MousePointer : MonoBehaviour 
{
	
	public static Vector3 position 
	{
		get {return GetPositionAtNullPlane();}
	}
	
	public static Vector3 GetPositionAtNullPlane()
	{
		return I.transform.position;
	}
	
	
    static Vector3 CalculatePositionAtNullPlane()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		float lambda = -ray.origin.z / ray.direction.z;
		Debug.DrawRay (ray.origin, ray.direction * lambda, Color.yellow);
		return ray.origin + ray.direction * lambda;
	}
	
	
	public static MousePointer I;
	
	void Awake()
	{
		if(I != this)
			Debug.LogWarning("Replacing " + this.GetType().Name + " instance!");
		
		I = this;
	}
	
	void Update()
	{
		this.transform.position = CalculatePositionAtNullPlane();
	}
	
}
