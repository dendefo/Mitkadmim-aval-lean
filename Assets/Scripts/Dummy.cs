using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Dummy : MonoBehaviour, IInteractable, IPointerClickHandler
{
    [SerializeField] UnityEvent _OnInteract;
    [SerializeField] Animator animator;

    UnityEvent IInteractable.OnInteract { get { return _OnInteract; } set { _OnInteract = value; } }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) (this as IInteractable).Interact();
    }

    private void OnValidate()
    {
        (this as IInteractable).OnInteract = _OnInteract;
    }

    public void BecameAttacked()
    {
        Soldier.chosenSoldier.ForEach(sol => sol.WalkAndAttack(this));
    }
    public void Defend()
    {
        animator.SetTrigger("Damage");
    }

}
