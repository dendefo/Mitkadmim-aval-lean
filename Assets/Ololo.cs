using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;

public class Ololo : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] List<Transform> points;
    int currentpoint = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        if (Vector3.Distance(agent.pathEndPosition, transform.position) < 2f) { currentpoint++; }
        if (currentpoint > points.Count - 1) currentpoint = 0;

        agent.SetDestination(points[currentpoint].position);
    }
}