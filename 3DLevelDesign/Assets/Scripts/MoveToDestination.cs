using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//We need to work with the navigation API...
using UnityEngine.AI;

public class MoveToDestination : MonoBehaviour {
    private NavMeshAgent thisAgent = null;
    //This needs to be public in order to access it in the inspector
    public Transform destination = null;

	// Use this for initialization
	void Start () {
        //Get a reference to the agent
        thisAgent = GetComponent<NavMeshAgent>();
		
	}
	
	// Update is called once per frame
	void Update () {
        thisAgent.SetDestination(destination.position);
	}
}
