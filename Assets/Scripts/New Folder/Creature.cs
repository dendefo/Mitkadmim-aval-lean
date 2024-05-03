using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Creature : MonoBehaviour
{
    public static event Action<AudioClip> VoiceEvent;
    public static List<Creature> creatures = new();
    public static List<Creature> chosenCreatures = new();

    [SerializeField] CreatureData data;
    public CreatureStats Stats;
    [SerializeField] NavMeshAgent NavAgent;
    [SerializeField] Animator animator;

    [SerializeField] List<SkinnedMeshRenderer> meshRenderers;
    private List<Vector3> points = new();


    private void Awake()
    {
        creatures.Add(this);
        if (data != null) SetStats(data.stats);
    }
    private void Update()
    {
        UpdateAnimations();
        if (points.Count > 0 && NavAgent.remainingDistance < 0.1f)
        {
            NavAgent.SetDestination(points[0]);
            var temp = points[0];
            points.RemoveAt(0);
            points.Add(temp);
        }
    }

    private void UpdateAnimations()
    {
        animator.SetFloat("Speed", NavAgent.velocity.magnitude);
        animator.SetFloat("Rotation", NavAgent.velocity.normalized.x);
    }

    public void SetStats(CreatureStats stats)
    {
        Stats = stats;
    }
    private void OnMouseDown()
    {
        if (chosenCreatures.Count != 0) chosenCreatures.ForEach(c => c.UnChoise());
        chosenCreatures.Clear();
        Choise();

    }
    public void Choise()
    {
        if (data.OnPickClip != null) VoiceEvent?.Invoke(data.OnPickClip);
        chosenCreatures.Add(this);
        foreach (var renderer in meshRenderers)
        {
            renderer.materials.ToList().ForEach(m => m.color = Color.green);

        }
        Debug.Log("Choise", this);
    }
    public void UnChoise()
    {
        foreach (var renderer in meshRenderers)
        {
            renderer.materials.ToList().ForEach(m => m.color = Color.white);
        }
    }
    public void NavigateAndClearPatroll(Vector3 destination)
    {
        NavAgent.SetDestination(destination);
        points.Clear();
        VoiceEvent?.Invoke(data.OnMoveClip);
    }
    public void AddPatrollPoint(Vector3 destination)
    {
        points.Add(destination);
    }
    public void IsInsideRect(Rect rect, Camera camera)
    {
        var screenPos = camera.WorldToScreenPoint(transform.position);
        if (rect.Contains(screenPos)) Choise();
        else if (!(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) && chosenCreatures.Contains(this))
        {
            UnChoise();
            chosenCreatures.Remove(this);
        }
    }
}