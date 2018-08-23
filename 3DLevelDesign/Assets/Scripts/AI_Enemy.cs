using UnityEngine;
using System.Collections;
//------------------------------------------
public class AI_Enemy : MonoBehaviour
{
	//------------------------------------------
	public enum ENEMY_STATE {PATROL, CHASE, ATTACK};
	//------------------------------------------
	public ENEMY_STATE CurrentState
	{
		get{return currentstate;}

		set
		{
			//Update current state
			currentstate = value;

			//Stop all running coroutines
			StopAllCoroutines();

			switch(currentstate)
			{
				case ENEMY_STATE.PATROL:
					StartCoroutine(AIPatrol());
				break;

				case ENEMY_STATE.CHASE:
					StartCoroutine(AIChase());
				break;

				case ENEMY_STATE.ATTACK:
					StartCoroutine(AIAttack());
				break;
			}
		}
	}
	//------------------------------------------
	[SerializeField]
	private ENEMY_STATE currentstate = ENEMY_STATE.PATROL;

    //Reference to line of sight component.
    //LineSight is also initialized, so it's not part of Unity
	private LineSight ThisLineSight = null;

	//Reference to nav mesh agent
	private UnityEngine.AI.NavMeshAgent ThisAgent = null;

	//Reference to player health
	private Health PlayerHealth = null;

	//Reference to player transform
	private Transform PlayerTransform = null;

	//Reference to patrol destination
	private Transform PatrolDestination = null;

	//Damage amount per second
	public float MaxDamage = 10f;

    //Reference to the animations
    private Animator ThisAnimator = null;
	//------------------------------------------
	void Awake()
	{
		ThisLineSight = GetComponent<LineSight>();
		ThisAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        //Access the player object's attributes, such as it's health and transform (position):
		PlayerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
		PlayerTransform = PlayerHealth.GetComponent<Transform>();

        //Access the animations
        ThisAnimator = GetComponent<Animator>();
	}
	//------------------------------------------
	void Start()
	{
		//Get random destination
		GameObject[] Destinations = GameObject.FindGameObjectsWithTag("Dest");
		PatrolDestination = Destinations[Random.Range(0, Destinations.Length)].GetComponent<Transform>();

		//Configure starting state
		CurrentState = ENEMY_STATE.PATROL;//Immediately jump to "public ENEMY_STATE CurrentState"
        //Note that there is a "currentState" variable and a "CurrentState" as well

        //Utilize the blend tree to strictly play the running animation
        ThisAnimator.SetFloat("Forward", 1.0f);
    }
	//------------------------------------------
	public IEnumerator AIPatrol()
	{
        //Loop while patrolling
        while (currentstate == ENEMY_STATE.PATROL)
        {
            //Set strict search
            ThisLineSight.Sensitity = LineSight.SightSensitivity.STRICT;

            //Chase to patrol position
            ThisAgent.isStopped = false;
            ThisAgent.SetDestination(PatrolDestination.position);

            //Wait until path is computed
            //Is a path in the process of being computed and not yet ready? (Read Only)
            //Will this skip over once the path is computed within another iteration of the while loop?
            while (ThisAgent.pathPending)
                yield return null;

            //While the NPC is going towards the destination...
            //If we can see the target then start chasing
            if (ThisLineSight.CanSeeTarget)
            {
                ThisAgent.isStopped = true;
                CurrentState = ENEMY_STATE.CHASE;
                yield break;
            }

            //Have we arrived at dest, get new dest. The stoppingDistance was originally 0 and it's now changed to 1
            //Otherwise, the agent will just "stay" at the destination as it will never satisfy the following condition (Distance can never be less than 0):
            if (Vector3.Distance(transform.position, PatrolDestination.position) <= ThisAgent.stoppingDistance*1.2f)
                {
                //Debug.Log("The Enemy is deciding on a new destination");
                GameObject[] Destinations = GameObject.FindGameObjectsWithTag("Dest");
                PatrolDestination = Destinations[Random.Range(0, Destinations.Length)].GetComponent<Transform>();
            }
       
			//Wait until next frame
			yield return null;
		}
	}
	//------------------------------------------
	public IEnumerator AIChase()
	{
		//Loop while chasing
		while(currentstate == ENEMY_STATE.CHASE)
		{
			//Set loose search
			ThisLineSight.Sensitity = LineSight.SightSensitivity.LOOSE;

            //Chase to last known position
            ThisAgent.isStopped = false;
			ThisAgent.SetDestination(ThisLineSight.LastKnowSighting);

			//Wait until path is computed
			while(ThisAgent.pathPending)
				yield return null;

			//Have we reached destination?
			if(ThisAgent.remainingDistance <= ThisAgent.stoppingDistance)
			{
				//Stop agent
                ThisAgent.isStopped = true;

				//Reached destination but cannot see player
				if(!ThisLineSight.CanSeeTarget)
					CurrentState = ENEMY_STATE.PATROL;
				else //Reached destination and can see player. Reached attacking distance
					CurrentState = ENEMY_STATE.ATTACK;

				yield break;
			}

			//Wait until next frame
			yield return null;
		}
	}
	//------------------------------------------
	public IEnumerator AIAttack()
	{
		//Loop while chasing and attacking
		while(currentstate == ENEMY_STATE.ATTACK)
		{
            //Chase to player position
            ThisAgent.isStopped = false;
			ThisAgent.SetDestination(PlayerTransform.position);

			//Wait until path is computed
			while(ThisAgent.pathPending)
				yield return null;

			//Has player run away?
			if(ThisAgent.remainingDistance > ThisAgent.stoppingDistance)
			{
				//Change back to chase
				CurrentState = ENEMY_STATE.CHASE;
				yield break;
			}
			else
			{
				//Attack
				PlayerHealth.HealthPoints -= MaxDamage * Time.deltaTime;
			}

			//Wait until next frame
			yield return null;
		}

		yield break;
	}
	//------------------------------------------
}
//------------------------------------------