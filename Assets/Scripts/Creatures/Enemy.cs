using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Creature
{
    public static List<Enemy> enemies = new List<Enemy>();
    public static event Action<Enemy> EnemyReachedBase;
    override protected void Awake()
    {
        base.Awake();
        enemies.Add(this);
    }
    override protected void Update()
    {
        if (IPausable.isPaused) return;
        if (AttackingTarget == null)
        {
            return;
        }
        NavAgent.SetDestination(AttackingTarget.position);

        if (Vector3.Distance(transform.position, AttackingTarget.position) < 7f && AttackingTarget.TryGetComponent(out PlayerBase playerBase))
        {
            EnemyReachedBase?.Invoke(this);
            playerBase.stats.GetDamage(Stats.Damage);
            Die();
        }
        base.Update();

    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
        if (collision.gameObject.TryGetComponent(out PlayerBase playerBase))
        {
            playerBase.stats.GetDamage(Stats.Damage);
            Die();
        }
    }
    protected override void Die()
    {
        enemies.Remove(this);
        base.Die();
    }
    override protected void OnDestroy()
    {
        base.OnDestroy();
        enemies.Remove(this);

    }
}
