using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//We need to ensure that we can change a scene using script
using UnityEngine.SceneManagement;

//[RequireComponent(typeof(AudioSource))]

public class CoinCollection : MonoBehaviour {
    //A static object counter = Unity will always know the number of objects in the level 
    public static int NumberCoinsRemaining = 0;

    //Collectable pickup audio:
    public AudioClip pickUpSound;
    private AudioSource audioSource;

    //The sound won't play after deactivation setActive. We need to deactivate these instead:
    private Renderer render;
    private Collider collision;

	// Use this for initialization
	void Start () {
        NumberCoinsRemaining += 1;
        audioSource = GetComponent<AudioSource>();
        render = GetComponent<Renderer>();
        collision = GetComponent<Collider>();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {   //Just return if whatever is touching the coin isn't the player
            return;
        }

        //If the code reaches here, the player has touched the coin:
        PlaySound();
        NumberCoinsRemaining -= 1;

        //Deactivate the object
        //gameObject.SetActive(false);

        //Deactivate the renderer, collider, and then destroy it to free up memory after 10 secs
        render.enabled = false;
        collision.enabled = false;
        Destroy(gameObject, 10f);

        //Check for the win condition
        if (NumberCoinsRemaining <= 0)
        {   //Win condition
            SceneManager.LoadScene("GameOver");
        }
    }

    private void PlaySound()
    {
        //Use this is you want to use setActive:
        //AudioSource.PlayClipAtPoint(pickUpSound, transform.position);

        audioSource.clip = pickUpSound;
        audioSource.Play();
    }
}