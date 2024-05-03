using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCoin : MonoBehaviour
{
    public int value;
    public static event Action<GoldCoin> OnGoldCoinClick;
    private void OnMouseDown()
    {
        OnGoldCoinClick?.Invoke(this);
    }
}
