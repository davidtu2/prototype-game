using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {
    //Get a reference to the animator component
    private Animator ThisAnimator = null;

    private void Awake()
    {
        //Get the animator component
        ThisAnimator = GetComponent<Animator>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //Get input on the vertical axis per frame
        //When either "W" or the "UP KEY" is pressed, +1 will return
        //Otherwise Unity detects no input and will return +0
        float Vertical = Input.GetAxis("Vertical");

        //Update the Forward field from the blend tree with the Vertical value
        //When Forward = +1 in a frame, the running animation will play
        //Otherwise it gets +0, which will play the idle animation
        ThisAnimator.SetFloat("Forward", Vertical);
	}
}
