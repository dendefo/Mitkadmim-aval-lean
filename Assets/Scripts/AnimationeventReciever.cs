using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationeventReciever : MonoBehaviour
{
    public void AttackEvent()
    {
        transform.parent.gameObject.SendMessage("Attack");
    }
}
