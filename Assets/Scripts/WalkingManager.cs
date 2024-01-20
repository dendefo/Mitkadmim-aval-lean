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
    public static WalkingManager instance;
    private void Awake()
    {
        instance = this;
    }
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
        if (Soldier.chosenSoldier == null) return;
        Soldier.chosenSoldier.ForEach(sol => 
        {   
            if (sol.currentForm == null) sol.Navigate(hit.point);
            else sol.currentForm.GetDestination(sol, hit.point); 
        });
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
            Soldier.soldiers.ForEach(sol => sol.IsInsideRect(new(corner, secondCorner - corner), _camera));


        }

    }
    public void Form(int formationID)
    {
        switch (formationID)
        {
            case 0:
            default:
                Formation form = new LineForm();
                Soldier.chosenSoldier.ForEach(sol => form.Join(sol));
                form.Form();
                break;
        }
    }
}
