using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationeventReciever : MonoBehaviour
{
    [SerializeField] private Creature parent;
    public void AttackEvent()
    {
        parent.Attack();
    }
}
