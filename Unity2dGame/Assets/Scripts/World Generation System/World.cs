using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class World : MonoBehaviour
{
    
    public CapsuleCollider Prefab;
    
    // Use this for initialization
    private void Start()
    {
        CreateCircleOfPrefabs(2);
        CreateCircleOfPrefabs(4); 
        CreateCircleOfPrefabs(6);

    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void CreateCircleOfPrefabs(float radius)
    {
        var circum = 2*Mathf.PI*radius;
        var countOfPrefabs = (circum/(Prefab.radius*2));
        var startPoint = RandomOnUnitCircle()*radius;
        Debug.Log("Count Needed: "+ countOfPrefabs);
        var stepAngle = (360 / countOfPrefabs);
        Debug.Log("StepAngle: "+ stepAngle);
        var quat = Quaternion.AngleAxis(stepAngle, Vector3.forward);
        for (int i = 0; i < (int)countOfPrefabs; i++)
        {
            Instantiate(Prefab, startPoint, Prefab.transform.rotation);
            startPoint = quat * startPoint;
        }
    }

    private Vector2 RandomOnUnitCircle()
    {
        var randomAngle =Random.Range(0, 360);
        var quat = Quaternion.AngleAxis(randomAngle, Vector3.forward);
        return quat * Vector2.up;
    }
}
