using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] GameObject target;
    void Start()
    {
        Soldier.TargetPosition += UpdatePosition;
        Soldier.ArrivedAtPositionAll.AddListener(ArrivedAtPosition);
    }

    private void UpdatePosition(Vector3 position)
    {
        transform.position = position;
        target.SetActive(true);
    }

    private void ArrivedAtPosition()
    {
        target.SetActive(false);
    }

    
}
