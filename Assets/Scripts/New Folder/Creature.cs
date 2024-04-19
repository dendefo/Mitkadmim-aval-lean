using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Creature : MonoBehaviour
{
    public static List<Creature> creatures = new();
    public static Creature chosenCreature;

    [SerializeField] CreatureData data;
    public CreatureStats Stats;
    [SerializeField] NavMeshAgent NavAgent;
    [SerializeField] Animator animator;
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    private Queue<Vector3> points = new();


    private void Awake()
    {
        creatures.Add(this);
        if (data != null) SetStats(data.stats);
    }

    public void SetStats(CreatureStats stats)
    {
        Stats = stats;
    }
    private void OnMouseDown()
    {
        if (chosenCreature != null) chosenCreature.UnChoise();
        Choise();

    }
    public void Choise()
    {
        chosenCreature = this;
        foreach (var mat in meshRenderer.materials)
        {
            mat.color = Color.green;
        }
    }
    public void UnChoise()
    {
        chosenCreature = null;
        foreach (var mat in meshRenderer.materials)
        {
            mat.color = Color.white;
        }
    }
}