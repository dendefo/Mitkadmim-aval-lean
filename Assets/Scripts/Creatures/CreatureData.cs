using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CreatureData", menuName = "ScriptableObjects/CreatureData", order = 1)]
public class CreatureData : ScriptableObject
{
    public CreatureStats stats;
    public int GoldValue;
    public float AngerRange;
    public float AttackRange;
    public AudioClip OnPickClip;
    public AudioClip OnMoveClip;
    public AudioClip OnDieClip;
    public AudioClip OnAttackClip;
    public AudioClip OnHitClip;
}
