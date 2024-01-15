using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
    public static List<Soldier> chosedSoldier = new();

    private void OnMouseUp()
    {
        if (!(Input.GetKey(KeyCode.LeftControl)||Input.GetKey(KeyCode.LeftShift))) ClearChoose();
        Chose();
    }
    // Update is called once per frame
    void Update()
    {
        if (points.Count == 0) return;
        if (agent.remainingDistance > 2) return;
        agent.SetDestination(points.Dequeue());
    }
    public void NavigateQueue(Vector3 target)
    {
        points.Enqueue(target);
    }
    public void Navigate(Vector3 target)
    {
        points.Clear();
        agent.ResetPath();
        NavigateQueue(target);
    }
    private void OnDrawGizmos()
    {
        try
        {
            Gizmos.DrawLineList(new(agent.path.corners));
        }
        catch { }
    }

    public void Chose()
    {
        if (chosedSoldier.Contains(this)) return;
        chosedSoldier.Add(this);
        meshRenderer.material = ChosenMaterial;
    }
    static public void ClearChoose()
    {
        if (chosedSoldier != null) chosedSoldier.ForEach(sol => sol.meshRenderer.material = sol.ReguralMaterial);
        chosedSoldier.Clear();
    }
}