using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CreatureData", menuName = "ScriptableObjects/CreatureData", order = 1)]
public class CreatureData : ScriptableObject
{
    public CreatureStats stats;
}
