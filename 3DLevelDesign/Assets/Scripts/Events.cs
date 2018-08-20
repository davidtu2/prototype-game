using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//We need to ensure that we can change a scene using script
using UnityEngine.SceneManagement;

public class Events : MonoBehaviour {

    private void Start()
    {
        //Unlock the cursor after the game is done and make it visible for use
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void quit()
    {
        //Platform specific code. Meaning that if we are running from the Unity Editor, we will exit play mode
        //Else we are running on a build, so we just tell the application to quit:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Debug.Log("Exiting...");
        Application.Quit();
#endif
    }

    public void restart()
    {
        Debug.Log("Loading...");
        SceneManager.LoadScene("SampleScene");
    }

    //Alternatively, you can just use this for loading all scenes... Just make sure your Build Settings are correct
    public void loadByIndex(int sceneIndex)
    {
        Debug.Log("Loading Scene Index #" + sceneIndex);
        SceneManager.LoadScene(sceneIndex);
    }
}
