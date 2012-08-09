using UnityEngine;
using System.Collections;

public class BuildBlock : MonoBehaviour
{
    private Vector3 offset;
    public Transform activeDockingPoint;
    public float searchRange = 5.0F;
    //Mousebuttons
    private static int leftButton =0;
    private static int rightButton =1;
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
        float lambda = Mathf.Abs(ray.origin.z) / ray.direction.z;
        Debug.DrawRay(ray.origin, ray.direction * lambda, Color.yellow);
        return ray.direction * lambda;
    }

    void OnMouseDown()
    {
        if (Input.GetMouseButton(leftButton))
        {
            Vector3 position = this.transform.position;
            offset = position - GetMousePositionAtNullPlane();

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
        Collider[] colliders = Physics.OverlapSphere(activeDockingPoint.transform.position, searchRange, 1 << LayerMask.NameToLayer("Docking Point"));
        if (colliders.Length > 0)
        {
            float minDistance = Mathf.Infinity;
            foreach (Collider c in colliders)
            {
                if (c.transform.parent.gameObject != this.gameObject)
                {
                    float distance = Vector3.Distance(c.transform.position, activeDockingPoint.transform.position);
                    if (distance < minDistance)
                    {
                        closestDockingPoint = c.transform;
                        minDistance = distance;
                    }
                }
            }
            if (closestDockingPoint != null)
            {
                Debug.DrawLine(activeDockingPoint.transform.position, closestDockingPoint.position);
            }
        }
        return closestDockingPoint;
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
        Gizmos.DrawWireSphere(activeDockingPoint.transform.position, searchRange);
    }
}
