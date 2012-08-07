using UnityEngine;
using System.Collections;

public class BuildBlock : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static Vector3 GetMousePositionAtNullPlane ()
	{
		Ray ray =Camera.main.ScreenPointToRay(Input.mousePosition);
		float lambda = Mathf.Abs(ray.origin.z)/ray.direction.z;
		Debug.DrawRay(ray.origin, ray.direction * lambda, Color.yellow);
		return ray.direction * lambda;
	}
	
	void OnMouseDown () {
		Vector3 position = this.transform.position;
		Vector3 atNullPlane = GetMousePositionAtNullPlane ();
	}
}
