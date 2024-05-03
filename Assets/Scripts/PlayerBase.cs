using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public HpBar hpBar;
    public CreatureStats stats;

    void Awake()
    {
        hpBar.Subscribe(ref stats.HpChanged);
    }

    private void OnDestroy()
    {
        hpBar.Unsubscribe();
    }

}
