//Make sure to not share this script with any other game object that may modify the eventSystem and/or selectedObject vars

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectOnInput : MonoBehaviour {

    //Used to help detect keyboard input
    public EventSystem eventSystem;
    public GameObject selectedObject;
    private bool buttonSelected;

    //Updates once per frame
    void Update()
    {
        Debug.Log(selectedObject);

        if (Input.GetAxisRaw("Vertical") != 0 && buttonSelected == false)
        {
            //Select the selected game object once input on the vertical axis is detected
            eventSystem.SetSelectedGameObject(selectedObject);
            buttonSelected = true;
        }
    }

    //When the game object is deactivated
    private void OnDisable()
    {
        buttonSelected = false;
    }
}