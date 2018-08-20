using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triggers : MonoBehaviour {

    //Access the animator
    public Animator TargetAnimator = null;

    public string TriggerName = string.Empty;

    //Launches when the player enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        TargetAnimator.SetTrigger(TriggerName);
    }
}
