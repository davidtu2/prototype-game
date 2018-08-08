using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//We need to ensure that we can change a scene using script
using UnityEngine.SceneManagement;

public class CoinCollection : MonoBehaviour {
    //A static object counter = Unity will always know the number of objects in the level 
    public static int NumberCoinsRemaining = 0;

	// Use this for initialization
	void Start () {
        NumberCoinsRemaining += 1;
	}

    // Update is called once per frame
    /*void Update () {
		
	}*/

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {   //Just return if whatever is touching the coin isn't the player
            return;
        }

        //If the code reaches here, the player has touched the coin
        NumberCoinsRemaining -= 1;
        //Deactivate the object
        gameObject.SetActive(false);

        //Check for the win condition
        if (NumberCoinsRemaining <= 0)
        {   //Win condition
            SceneManager.LoadScene("completedScene");
        }
    }
}
