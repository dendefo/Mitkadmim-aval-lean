using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WalkingManager : MonoBehaviour
{
    [SerializeField] public Camera _camera;
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
        if (!Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) return;
        if (!hit.collider.CompareTag("Surface")) return;
        if (Creature.chosenCreatures == null) return;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Creature.chosenCreatures.ForEach(c => c.AddPatrollPoint(hit.point));
        }
        else Creature.chosenCreatures.ForEach(c => c.NavigateAndClearPatroll(hit.point));
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
            if (Vector2.Distance(curentMousePos, startMousePos) < 10) return;
            var corner = new Vector2(Mathf.Min(startMousePos.x, curentMousePos.x), Mathf.Min(startMousePos.y, curentMousePos.y));
            var secondCorner = new Vector2(Mathf.Max(startMousePos.x, curentMousePos.x), Mathf.Max(startMousePos.y, curentMousePos.y));
            Creature.creatures.ForEach(sol => sol.IsInsideRect(new(corner, secondCorner - corner), _camera));


        }

    }
}
