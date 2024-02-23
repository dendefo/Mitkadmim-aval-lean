using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Soldier : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] List<AudioClip> VoiceLines;
    
    [SerializeField] Queue<Vector3> points = new();
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    private static List<Soldier> _chosenSoldier = new();

    public UnityEvent<string,float> SoldierMove;
    public static Action<AudioClip> VoiceLineTrigger;
    public static Action<Vector3> TargetPosition;
    public static Action ArrivedAtPosition;

    public static List<Soldier> chosenSoldier
    {
        get { return _chosenSoldier; }
        set
        {
            _chosenSoldier = value;
            //here Activate Sounds
        }
    }
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
        SoldierMove.Invoke("Speed", agent.velocity.magnitude);
        if (agent.remainingDistance < 1 && !agent.isStopped)
        {
            agent.isStopped = true;
            ArrivedAtPosition.Invoke();
            Debug.Log("I have arraived");
        }
        if (!agent.isStopped) return;
        if (points.Count == 0) return;
        agent.SetDestination(points.Dequeue());

    }
    public void Navigate(Vector3 target)
    {
        if (VoiceLines.Count > 0)
        {
            VoiceLineTrigger.Invoke(VoiceLines[VoiceLines.Count - 1]);
        }
        TargetPosition.Invoke(target);
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            points.Clear();
            agent.ResetPath();
            points.Enqueue(target);
            agent.SetDestination(points.Dequeue());
            return;
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
        if (chosenSoldier.Contains(this)) return;
        chosenSoldier.Add(this);
        meshRenderer.materials.ToList().ForEach(mat => mat.color = UnityEngine.Color.green);
        if (VoiceLines.Count > 0)
        {
            VoiceLineTrigger.Invoke(VoiceLines[0]);
        }
    }
    static public void ClearChoose()
    {
        if (chosenSoldier != null) chosenSoldier.ForEach(sol => sol.meshRenderer.materials.ToList().ForEach(mat => mat.color = UnityEngine.Color.white));
        chosenSoldier.Clear();
    }
    public void IsInsideRect(Rect rect, Camera camera)
    {
        var screenPos = camera.WorldToScreenPoint(transform.position);
        if (rect.Contains(screenPos)) Chose();
        else if (!(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) && chosenSoldier.Remove(this)) meshRenderer.materials.ToList().ForEach(mat => mat.color = UnityEngine.Color.white);
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
        rot = Quaternion.Euler(0, 55, 0) * rot;
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