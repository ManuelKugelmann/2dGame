using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class World : MonoBehaviour
{
    public float worldRadius = 5.0F;
    public GameObject prefab;
    public int maxWorldBlocks = 200;
    public float radiusTolerance = 0.6F;
    public int sizeDecreaseMod = 100;
    private int counter;
    public int countFailedInstantiate;
    private int countLoopsSinceLastSizeChange;
    private float capsuleRadius;
    private Vector3 currentScale = new Vector3(1, 1, 1);
    // Use this for initialization
    private void Start()
    {
        capsuleRadius = prefab.GetComponent<CapsuleCollider>().radius;
        DateTime time = DateTime.Now;
        while (counter <= maxWorldBlocks)
        {
            if (countFailedInstantiate%sizeDecreaseMod == 0 && countLoopsSinceLastSizeChange > sizeDecreaseMod && currentScale.x > 0.3F)
            {
                currentScale.x *= 0.9F;
                currentScale.z *= 0.9F;
                capsuleRadius *= 0.9F;
                radiusTolerance *= 0.9F;
                Debug.Log("Scaled Radius to " + currentScale.x + " , " + currentScale.z);
            }
            Vector3 pos = Random.insideUnitCircle*worldRadius;
            pos += this.transform.position;
            if (!Physics.CheckSphere(pos, capsuleRadius*radiusTolerance))
            {
                countLoopsSinceLastSizeChange = 0;
                var go = Instantiate(prefab, pos, prefab.transform.rotation) as GameObject;
                go.transform.parent = transform;
                go.transform.localScale = currentScale;
                counter++;
                Debug.Log("Block Number " + counter + " created!");
            }
            else
            {
                countLoopsSinceLastSizeChange++;
                countFailedInstantiate++;
            }
        }
        TimeSpan span =  DateTime.Now - time;
        Debug.Log(span.Milliseconds);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
