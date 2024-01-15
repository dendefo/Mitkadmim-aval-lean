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
    [SerializeField] Animator animator;
    [SerializeField] Queue<Vector3> points = new();
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    public static List<Soldier> chosedSoldier = new();
    public static List<Soldier> soldiers = new();
    public Formation currentForm;

    private void Awake()
    {
        soldiers.Add(this);
    }
    private void OnMouseUp()
    {
        if (!(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift))) ClearChoose();
        Chose();
    }
    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
        if (points.Count == 0) return;
        if (agent.remainingDistance > 2) return;
        agent.SetDestination(points.Dequeue());
    }
    public void Navigate(Vector3 target)
    {
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            points.Clear();
            agent.ResetPath();
        }
        points.Enqueue(target);
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
        meshRenderer.materials.ToList().ForEach(mat => mat.color = UnityEngine.Color.green);
    }
    static public void ClearChoose()
    {
        if (chosedSoldier != null) chosedSoldier.ForEach(sol => sol.meshRenderer.materials.ToList().ForEach(mat => mat.color = UnityEngine.Color.white));
        chosedSoldier.Clear();
    }
    public void IsInsideRect(Rect rect, Camera camera)
    {
        var screenPos = camera.WorldToScreenPoint(transform.position);
        if (rect.Contains(screenPos)) Chose();
        else if (!(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) && chosedSoldier.Remove(this)) meshRenderer.materials.ToList().ForEach(mat => mat.color = UnityEngine.Color.white);
    }
}
public abstract class Formation
{
    protected List<Soldier> Soldiers { get; set; }
    public virtual bool Join(Soldier soldier)
    {
        if (Soldiers.Contains(soldier)) return false;
        Soldiers.Add(soldier);
        soldier.currentForm = this;
        return true;
    }
    public virtual bool Leave(Soldier soldier)
    {
        soldier.currentForm = null;
        return Soldiers.Remove(soldier);
    }
    public abstract void Form();
    public abstract void GetDestination(Soldier soldier, Vector3 formationDest);
    protected abstract Vector3 CentralPoint();
}

class LineForm : Formation
{
    [SerializeField] private float DistanceBetween;
    protected override Vector3 CentralPoint()
    {
        Vector3 centralPoint = Vector3.zero;
        Soldiers.ForEach(sol => centralPoint += sol.transform.position);
        return centralPoint / Soldiers.Count;
    }

    public override void Form()
    {
        Vector3 centralPoint = CentralPoint();
        var rot = WalkingManager.instance._camera.transform.rotation * Vector3.one;
        rot = Quaternion.Euler(0, 55, 0)*rot;
        for (int i = 0; i < Soldiers.Count; i++)
        {
            Soldiers[i].Navigate(centralPoint + (rot * (DistanceBetween * i)) - ((rot * DistanceBetween * (Soldiers.Count / 2))));
        }
    }

    public override void GetDestination(Soldier soldier, Vector3 formationDest)
    {
        var rot = WalkingManager.instance._camera.transform.rotation * Vector3.one;
        rot = Quaternion.Euler(0, 55, 0) * rot;
        soldier.Navigate(formationDest + (rot * (DistanceBetween * Soldiers.IndexOf(soldier))) - ((rot * DistanceBetween * (Soldiers.Count / 2))));
    }
    public LineForm()
    {
        Soldiers = new();
        DistanceBetween = 2;
    }
}