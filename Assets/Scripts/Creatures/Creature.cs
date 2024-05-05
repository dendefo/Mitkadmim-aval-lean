using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Creature : MonoBehaviour, IPausable, ISavable
{
    public static event Action<AudioClip> VoiceEvent;
    public static event Action<Creature> CreatureSpawned;
    public static List<Creature> creatures = new();
    public static List<Creature> chosenCreatures = new();

    public CreatureData data;
    public CreatureStats Stats;
    [SerializeField] protected NavMeshAgent NavAgent;
    [SerializeField] Animator animator;
    public HpBar hpBar;

    [SerializeField] List<SkinnedMeshRenderer> meshRenderers;
    public List<Vector3> points = new();
    public Transform AttackingTarget;

    public CreatureSaveData SaveData;

    protected virtual void Awake()
    {
        ISavable.SaveEvent += Save;
        IPausable.PauseEvent += Pause;
        creatures.Add(this);
        if (ISavable.WantsToLoad) return;
        if (data != null) SetStats(data.stats);

        if (hpBar != null) hpBar.Subscribe(ref Stats.HpChanged);
        Stats.Die += Die;
    }

    private void Save(Dictionary<string, object> dictionary)
    {
        if (Stats.CurrentHP == 0) return;
        CreatureSaveData saveData = new(this);
        dictionary[GetInstanceID().ToString() + "Creature"] = saveData;
    }

    public void Load(CreatureSaveData data)
    {
        SetStats(data.Stats);
        points = data.Points;
        if (hpBar != null) hpBar.Subscribe(ref Stats.HpChanged);
        Stats.Die += Die;
    }
    private void Pause(bool isPaused)
    {
        NavAgent.enabled = !isPaused;
        animator.enabled = !isPaused;
    }

    private void Start()
    {
        CreatureSpawned?.Invoke(this);
    }
    protected virtual void Die()
    {
        if (data.OnDieClip != null) VoiceEvent?.Invoke(data.OnDieClip);
        animator.SetBool("IsDead", true);
        creatures.Remove(this);
        chosenCreatures.Remove(this);
        NavAgent.enabled = false;
        Destroy(gameObject, 2f);
        enabled = false;

    }

    protected virtual void Update()
    {
        if (IPausable.isPaused) return;
        UpdateAnimations();
        if (points.Count > 0 && NavAgent.remainingDistance < 0.1f)
        {
            NavAgent.SetDestination(points[0]);
            var temp = points[0];
            points.RemoveAt(0);
            points.Add(temp);
        }
        CheckEnemiesToAttack();
        if (AttackingTarget != null && this is not Enemy)
        {
            NavAgent.SetDestination(AttackingTarget.position);
            Debug.Log(NavAgent.remainingDistance, this);
            if (NavAgent.remainingDistance < data.AttackRange)
            {
                animator.SetTrigger("Attack");
            }
        }
    }

    //Called from AnimationEventReciever by SendMessage
    public void Attack()
    {
        if (AttackingTarget.TryGetComponent(out Creature creature))
        {
            creature.Stats.GetDamage(Stats.Damage);
            if (data.OnAttackClip != null) VoiceEvent?.Invoke(data.OnAttackClip);
        }
    }
    private void CheckEnemiesToAttack()
    {
        if (this is Enemy) return;
        if (AttackingTarget != null) return;
        var enemies = Enemy.enemies;
        if (enemies.Count == 0) return;
        if (enemies.Any(x => Vector3.Distance(x.transform.position, transform.position) < data.AngerRange))
        {
            AttackingTarget = enemies.First(x => Vector3.Distance(x.transform.position, transform.position) < data.AngerRange).transform;
        }
    }
    private void UpdateAnimations()
    {
        animator.SetFloat("Speed", (transform.rotation * NavAgent.velocity).normalized.x);
        animator.SetFloat("Rotation", (transform.rotation * NavAgent.velocity).normalized.z);
    }

    public void SetStats(CreatureStats stats)
    {
        Stats = stats;
    }

    private void OnMouseDown()
    {
        if (this is Enemy) return;
        if (chosenCreatures.Count != 0) chosenCreatures.ForEach(c => c.UnChoise());
        chosenCreatures.Clear();
        Choise();

    }
    private void OnMouseEnter()
    {
        hpBar.gameObject.SetActive(true);
    }
    private void OnMouseExit()
    {
        hpBar.gameObject.SetActive(false);
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
        if (data.OnMoveClip != null) VoiceEvent?.Invoke(data.OnMoveClip);
    }
    public void AddPatrollPoint(Vector3 destination)
    {
        points.Add(destination);
    }
    public void IsInsideRect(Rect rect, Camera camera)
    {
        var screenPos = camera.WorldToScreenPoint(transform.position);
        if (this is Enemy) return;
        if (rect.Contains(screenPos)) Choise();
        else if (!(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) && chosenCreatures.Contains(this))
        {
            UnChoise();
            chosenCreatures.Remove(this);
        }
    }
    protected virtual void OnDestroy()
    {
        ISavable.SaveEvent -= Save;
        IPausable.PauseEvent -= Pause;
        Stats.Die -= Die;
        if (hpBar != null) hpBar.Unsubscribe();
        creatures.Remove(this);
        chosenCreatures.Remove(this);
    }
}
[Serializable]
public struct CreatureSaveData
{
    public CreatureStats Stats;
    public Vector3 Position;
    public Quaternion Rotation;
    public List<Vector3> Points;
    public string Name;

    public CreatureSaveData(Creature creature)
    {
        Stats = creature.Stats;
        Position = creature.transform.position;
        Rotation = creature.transform.rotation;
        Points = creature.points;
        Name = creature.name.Replace("(Clone)", "");
    }
}