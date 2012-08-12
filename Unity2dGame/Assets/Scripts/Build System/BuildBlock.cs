using UnityEngine;
using System.Collections;

public class BuildBlock : MonoBehaviour
{
    private Vector3 offset;
    private Transform activeDockingPoint;
    private ArrayList dockingPoints;
    private bool leftMouseDown = false;
    private int currentActive = 0;
    public float searchRange = 2.0F;
    //Mousebuttons
    private static int leftButton = 0;
    private static int rightButton = 1;
    // Use this for initialization
    void Start()
    {
        dockingPoints = new ArrayList();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(rightButton) && dockingPoints.Count >0 && leftMouseDown)
        {
            Debug.Log("Righclick");
            currentActive += 1;
            if (currentActive >= dockingPoints.Count)
            {
                currentActive = 0;
            }
            activeDockingPoint = (Transform)dockingPoints[currentActive];
            var mousePosition = GetMousePositionAtNullPlane();
            this.transform.position = this.transform.position + (mousePosition - activeDockingPoint.position);
            offset = this.transform.position - mousePosition;
        }
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
        Transform closest = null;
        
        if (Input.GetMouseButton(leftButton))
        {
            leftMouseDown = true;
            var mousePosition = GetMousePositionAtNullPlane();

            float minDistance = Mathf.Infinity;
            foreach (Transform child in this.transform)
            {
                if (child.gameObject.layer == LayerMask.NameToLayer("Docking Point"))
                {
                    dockingPoints.Add(child);
                    if (CheckDistance(mousePosition, ref minDistance, child.position))
                    {
                        closest = child;
                    }
                }
            }
            activeDockingPoint = closest;
            currentActive = dockingPoints.IndexOf(activeDockingPoint);
            this.transform.position = this.transform.position + (mousePosition - activeDockingPoint.position);
            offset = this.transform.position - mousePosition;
        }
        if (Input.GetMouseButton(rightButton) && dockingPoints != null)
        {
            

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
                    if (CheckDistance(activeDockingPoint.position, ref minDistance, c.transform.position))
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
        if (distance < minDistance)
        {
            minDistance = distance;
            return true;
        }
        return false;
    }

    void OnMouseUp()
    {
        leftMouseDown = false;
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
