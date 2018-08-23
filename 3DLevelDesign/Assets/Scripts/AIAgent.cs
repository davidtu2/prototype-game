using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//-----------------------------------
public class AIAgent : MonoBehaviour
{
	//-----------------------------------
	public enum AISTATE {IDLE=0,CHASE=1,ATTACK=2};
	public AISTATE CurrentState = AISTATE.IDLE;
	private NavMeshAgent ThisAgent = null;
	private Transform ThisTransform = null;
	private Transform PlayerObject = null;

	//AI Visiility Settings
	public bool CanSeePlayer = false;
	public float ViewAngle = 90f;
	public float AttackDistance = 1f;

    //Added for testing:
    //Reference to eyes
    //This is auto initiated from the inspector
    public Transform EyePoint = null;

    //Reference to sphere collider
    private SphereCollider ThisCollider = null;

    //Reference to the animations
    private Animator ThisAnimator = null;
    //-----------------------------------
    // Use this for initialization
    void Awake () 
	{
		ThisAgent = GetComponent<NavMeshAgent> ();
		ThisTransform = GetComponent<Transform>();
        //Get the player's transform component:
		PlayerObject = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ();

        //Added for testing:
        //This is like the NPC's proximity
        ThisCollider = GetComponent<SphereCollider>();

        //Access the animations
        ThisAnimator = GetComponent<Animator>();
    }
	//-----------------------------------
	void Start()
	{
		//Set Starting State
		ChangeState (CurrentState);
	}
	//-----------------------------------
	public IEnumerator Idle()
	{
		//Get Random Point
		Vector3 Point = RandomPointOnNavMesh();
		float WaitTime = 10f;
		float ElapsedTime = 0f;

		//Loop while idling
		while(CurrentState == AISTATE.IDLE)
		{
            //Goes to the random point
            ThisAgent.SetDestination (Point);
			ElapsedTime += Time.deltaTime;

            //Utilize the blend tree to play the running animation
            ThisAnimator.SetFloat("Forward", 1.0f, 0.1f, Time.deltaTime);

            if (ThisAgent.remainingDistance <= 0)
            {
                //Utilize the blend tree to strictly play the idle animation
                ThisAnimator.SetFloat("Forward", 0.0f);
            }

            //After waiting in one spot for a while, go to the next random point
			if(ElapsedTime >= WaitTime)
			{
				ElapsedTime = 0f;
				Point = RandomPointOnNavMesh();
			}

            //During that time, if I can see the player, give chase
			if(CanSeePlayer)
			{
				ChangeState (AISTATE.CHASE);
				yield break;
			}

			yield return null;
		}
	}
	//-----------------------------------
	public IEnumerator Chase()
	{
		while(CurrentState == AISTATE.CHASE)
		{
            //Utilize the blend tree to play the running animation
            ThisAnimator.SetFloat("Forward", 1.0f, 0.1f, Time.deltaTime);

			ThisAgent.SetDestination (PlayerObject.position);

			if(!CanSeePlayer)
			{
				yield return new WaitForSeconds (2f);

				if(!CanSeePlayer)
				{
					ChangeState (AISTATE.IDLE);
					yield break;
				}
			}

			if(Vector3.Distance (ThisTransform.position, PlayerObject.position) <= AttackDistance)
			{
				ChangeState (AISTATE.ATTACK);
				yield break;
			}

			yield return null;
		}
	}
	//-----------------------------------
	public IEnumerator Attack()
	{
		while(CurrentState == AISTATE.ATTACK)
		{
			//Deal damage here
			if(!CanSeePlayer || Vector3.Distance (ThisTransform.position, PlayerObject.position) > AttackDistance)
			{
				ChangeState (AISTATE.CHASE);
			}

			yield return null;
		}
	}
	//-----------------------------------
	public void ChangeState(AISTATE NewState)
	{
		StopAllCoroutines ();
		CurrentState = NewState;

		switch(NewState)
		{
			case AISTATE.IDLE:
				StartCoroutine (Idle());
			break;

			case AISTATE.CHASE:
				StartCoroutine (Chase());
			break;

			case AISTATE.ATTACK:
				StartCoroutine (Attack());
			break;
		}
	}
    //-----------------------------------
    //Invoked whenever the player has entered within my proximity...
    void OnTriggerStay(Collider Col)
	{
        if(!Col.CompareTag ("Player"))
            return;

		CanSeePlayer = false;

		//Player transform
		Transform PlayerTransform = Col.GetComponent<Transform>();

		//Is player in sight
		Vector3 DirToPlayer = PlayerTransform.position - ThisTransform.position;

		//Get viewing angle
		float ViewingAngle = Mathf.Abs(Vector3.Angle(ThisTransform.forward, DirToPlayer));

		if(ViewingAngle > ViewAngle)
			return;

		//Is there a direct line of sight?
		/*if(!Physics.Linecast(ThisTransform.position, PlayerTransform.position))
			CanSeePlayer = true;*/
        
        //Added for testing:
        if (ClearLineofSight())
        {
            CanSeePlayer = true;
        }
	}
	//-----------------------------------
	public Vector3 RandomPointOnNavMesh()
	{
		float Radius = 5f;
		Vector3 Point = ThisTransform.position + Random.insideUnitSphere * Radius;
		NavMeshHit NH;
		NavMesh.SamplePosition (Point, out NH, Radius, NavMesh.AllAreas);
		return NH.position;
	}
	//-----------------------------------
	void OnTriggerExit(Collider Col)
	{
		if(!Col.CompareTag ("Player"))
			return;

		CanSeePlayer = false;
	}
    //-----------------------------------
    //Added for testing:
    bool ClearLineofSight()
    {
        RaycastHit Info;

        //If there is a ray between the NPC's eyes and his target
        //Also returns into Info
        if (Physics.Raycast(EyePoint.position, (PlayerObject.position - EyePoint.position).normalized, out Info, ThisCollider.radius))
        {
            //If player, then can see player
            //Access the target's Transform to compare the tag?
            if (Info.transform.CompareTag("Player"))
                return true;
        }

        return false;
    }
}
