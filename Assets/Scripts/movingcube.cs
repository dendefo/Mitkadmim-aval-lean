using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingcube : MonoBehaviour
{
    [SerializeField] Transform left;
    [SerializeField] Transform right;
    bool isMovingRight;
    [SerializeField] float TimeToMove;
    private float lastTimeOfDirectionChange;
    // Start is called before the first frame update
    void Start()
    {
        lastTimeOfDirectionChange = 0;
    }

    // Update is called once per frame
    void Update()
    {


        if (isMovingRight)
        {

            transform.position = Vector3.Lerp(transform.position, right.transform.position, (Time.time - lastTimeOfDirectionChange) / TimeToMove);
            if (Vector3.Distance(transform.position, right.position) < 0.5f) { isMovingRight = false; lastTimeOfDirectionChange = Time.time; }
        }

        else
        {
            transform.position = Vector3.Lerp(transform.position, left.transform.position, (Time.time - lastTimeOfDirectionChange) / TimeToMove);
            if (Vector3.Distance(transform.position, left.position) < 0.5f) { isMovingRight = true; lastTimeOfDirectionChange = Time.time; }
        }
    }
}
