using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    [SerializeField]
    public UnityEngine.Events.UnityEvent OnInteract { get; set; }


    public void Interact()
    {
        OnInteract.Invoke();
    }
}
