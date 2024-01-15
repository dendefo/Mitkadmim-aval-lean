using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Soldier : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Queue<Vector3> points = new();
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material ReguralMaterial;
    [SerializeField] Material ChosenMaterial;
    public static Soldier chosedSoldier;

    private void OnMouseDown()
    {
        if (chosedSoldier != null) chosedSoldier.meshRenderer.material = ReguralMaterial;
        chosedSoldier = this;
        meshRenderer.material = ChosenMaterial;
    }
    // Update is called once per frame
    void Update()
    {
        if (points.Count == 0) return;
        agent.SetDestination(points.Dequeue());
    }
    public void Navigate(Vector3 targetTransform)
    {
        points.Enqueue(targetTransform);
    }

}