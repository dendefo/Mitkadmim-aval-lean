using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WalkingManager : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [SerializeField] Vector2 startMousePos;
    [SerializeField] Vector2 curentMousePos;
    [SerializeField] Image _image;
    void Update()
    {
        ChooseUnits();
        Navigation();
    }
    void Navigation()
    {

        if (!Input.GetMouseButtonDown(1)) return;
        RaycastHit hit;
        if (!Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hit)) return;
        if (!hit.collider.CompareTag("Surface")) return;
        if (Soldier.chosedSoldier == null) return;
        if (Input.GetKey(KeyCode.LeftShift)) Soldier.chosedSoldier.ForEach(sol => sol.NavigateQueue(hit.point));
        else Soldier.chosedSoldier.ForEach(sol => sol.Navigate(hit.point));
    }
    void ChooseUnits()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startMousePos = Input.mousePosition;
            _image.rectTransform.position = startMousePos;
            _image.rectTransform.sizeDelta = startMousePos;
            _image.enabled = true;
        }
        if (Input.GetMouseButton(0))
        {
            curentMousePos = Input.mousePosition;
            _image.rectTransform.pivot = new(curentMousePos.x < startMousePos.x ? 1 : 0, curentMousePos.y < startMousePos.y ? 1 : 0);

            _image.rectTransform.sizeDelta = new(Mathf.Abs(curentMousePos.x - startMousePos.x), Mathf.Abs(curentMousePos.y - startMousePos.y));
        }
        if (Input.GetMouseButtonUp(0))
        {
            curentMousePos = Input.mousePosition;
            _image.enabled = false;
            RaycastHit[] hits = Physics.BoxCastAll(_camera.transform.position, _image.rectTransform.sizeDelta / 2, _camera.transform.rotation * Vector3.forward, Quaternion.identity, 200f, LayerMask.GetMask("Units"));
            if (hits.Length != 0) Soldier.ClearChoose();
            foreach (var hit in hits)
            {
                Soldier sol;
                if (!hit.transform.TryGetComponent(out sol)) continue;
                sol.Chose();
                Debug.Log(Soldier.chosedSoldier.Count);
            }
        }

    }
}
