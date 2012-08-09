using UnityEngine;
using System.Collections;

public class BuildBlock : MonoBehaviour
{
    private Vector3 offset;
    private Transform activeDockingPoint;
    public float searchRange = 2.0F;
    //Mousebuttons
    private static int leftButton = 0;
    private static int rightButton = 1;
    // Use this for initialization
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static Vector3 GetMousePositionAtNullPlane()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float lambda = -ray.origin.z / ray.direction.z;
        Debug.DrawRay(ray.origin, ray.direction * lambda, Color.yellow);
        return ray.origin + ray.direction * lambda;
    }

    void OnMouseDown()
    {
        if (Input.GetMouseButton(leftButton))
        {
            Vector3 position = this.transform.position;
            var mousePosition = GetMousePositionAtNullPlane();
            offset = position - mousePosition ;
            Transform closest = null;
            float minDistance = Mathf.Infinity;
            foreach (Transform child in this.transform) 
            {
                if (child.gameObject.layer == LayerMask.NameToLayer("Docking Point"))
                {
                    Debug.Log(mousePosition + " " + child.position);
                    if (CheckDistance(mousePosition, ref minDistance, child.position))
                    {
                        closest = child.transform;
                    }
                }
            }
            activeDockingPoint = closest;
        }
        if (Input.GetMouseButton(rightButton))
        {
            //TODO switch activeDockingPoint
        }

    }

    void OnMouseDrag()
    {
        if (Input.GetMouseButton(leftButton))
        {
            if (rigidbody != null)
            {
                rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                this.transform.position = GetMousePositionAtNullPlane() + offset;
            }
            Transform closestDockingPoint = GetClosestDockingPoint();
        }
    }

    private Transform GetClosestDockingPoint()
    {
        Transform closestDockingPoint = null;
        Collider[] colliders = Physics.OverlapSphere(activeDockingPoint.position, searchRange, 1 << LayerMask.NameToLayer("Docking Point"));
        if (colliders.Length > 0)
        {
            float minDistance = Mathf.Infinity;
            foreach (Collider c in colliders)
            {
                if (c.transform.parent.gameObject != this.gameObject)
                {
                    if(CheckDistance(activeDockingPoint.position,ref minDistance, c.transform.position))
                    {
                        closestDockingPoint = c.transform;
                    }
                }
            }
            if (closestDockingPoint != null)
            {
                Debug.DrawLine(activeDockingPoint.position, closestDockingPoint.position);
            }
        }
        return closestDockingPoint;
    }

    private bool CheckDistance(Vector3 start, ref float minDistance, Vector3 end)
    {

        float distance = Vector3.Distance(start, end);
        Debug.Log("previous Distance " + minDistance + " new distance " + distance + " bool " + (distance < minDistance));
        if (distance < minDistance)
        {
            minDistance = distance;
            return true;
        }
        return false;
    }

    void OnMouseUp()
    {
        if (rigidbody != null)
        {
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
            
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (activeDockingPoint != null)
        {
            Gizmos.DrawWireSphere(activeDockingPoint.position, searchRange);
        }
    }
}
