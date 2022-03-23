using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMove : MonoBehaviour
{

    public float cloudSpeed = 1f;
    private Vector3 cloudVector;

    // Start is called before the first frame update
    void Start()
    {
        cloudSpeed *= (transform.localScale.x * 10);
        cloudSpeed *= .0004f;
        cloudVector = new Vector3 (cloudSpeed, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position -= cloudVector;
    }
}
