using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingManager : MonoBehaviour
{
    [SerializeField] Camera _camera;
    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        RaycastHit hit;
        if (!Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hit)) return;
        if (!hit.collider.CompareTag("Surface")) return;
        if (Soldier.chosedSoldier == null) return;
        Soldier.chosedSoldier.Navigate(hit.point);
    }
}
