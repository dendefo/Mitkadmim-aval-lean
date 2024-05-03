using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] Image FillBar;
    Action<float> SubscribedEvent;
    Transform _lookAtReference;
    
    public void SetUp(Transform lookAtReference)
    {
        _lookAtReference = lookAtReference;
    }
    public void Update()
    {
        if (_lookAtReference == null) return;
        transform.LookAt(_lookAtReference);
    }
    public void Subscribe(ref Action<float> action)
    {
        SubscribedEvent = action;
        action += UpdateHpBar;
    }
    public void UpdateHpBar(float value)
    {
        Debug.Log("Updating hp bar");
        FillBar.fillAmount = value;
    }
    public void Unsubscribe()
    {
        SubscribedEvent -= UpdateHpBar;
    }

}
