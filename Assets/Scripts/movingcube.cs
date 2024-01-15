using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingcube : MonoBehaviour
{
    [SerializeField] Transform left;
    [SerializeField] Transform right;
    bool isMovingRight;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isMovingRight)
        {

            transform.Translate(right.position - transform.position);
            if (Vector3.Distance(transform.position, right.position) < 0.5f) isMovingRight = false;
        }

        else
        {
            transform.Translate(left.position - transform.position);
            if (Vector3.Distance(transform.position, left.position) < 0.5f) isMovingRight = true;
        }
    }
}
