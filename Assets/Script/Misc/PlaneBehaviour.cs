using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBehaviour : MonoBehaviour
{
    [SerializeField]
    private float speed = 20f;

    Vector3 startPos;
    Vector3 endPos;

    private void Start()
    {
    
        startPos = this.transform.position;
        endPos = new Vector3(startPos.x - 100, startPos.y + 10, startPos.z);
    }

    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, endPos, step);
        if (Vector3.Distance(transform.position, endPos) < 0.001f)
        {
            transform.position = startPos;
        }
    }
}
