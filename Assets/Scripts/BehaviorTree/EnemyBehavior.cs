using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    public string EnemyName;
    [SerializeField]
    private GameObject bullet;

    private Transform turret;
    private Transform bulletSpawnPoint;
    private float turretRotSpeed = 10.0f;
    private Vector3 targetPos;
    private Transform optimalPos;

    [SerializeField]
    private GameObject target;
    private GameObject potentialTarget = null;

    //Bullet shooting rate
    public float shootDelay = 2.0f;
    private float elapsedTime;
    public float firingRangePercent = 50;

    // Patrol + detection
    [Header("Patrol Settings")]
    [SerializeField] private List<Transform> patrolPoints;
    private int currentPatrolIndex = 0;
    [SerializeField] private float patrolSpeed = 3.0f;
    [SerializeField] private float waypointTolerance = 1f;

    [Header("Detection Settings")]
    //[SerializeField] private float detectionRange = 15f;
    [SerializeField] public float detectionRange = 15f;
    [SerializeField] private float enemyCloseProximity = 1f;
    private bool targetDetected = false;
	
	[Header("Behavior Nodes")]
	private SequenceNode _IdleRoot;
	private SelectorNode _EnemyMove;
	private SequenceNode _EnemyPatrol;
	private ActionNode _MoveToLocation;
	private SequenceNode _EnemyLastStand;
	private ActionNode _FireMissile;
	private SelectorNode _EnemyDefend;
	private SequenceNode _EnemyCover;
	private ActionNode _MoveToCover;
	private SequenceNode _EnemyAttack;
	private ActionNode _FireWeapon;

    private bool isAlive;
    private float moveSpeed;
    private float rotateSpeed;

    //[SerializeField] private float moveSpeed = 5.0f; // Default value
    //[SerializeField] private float rotateSpeed = 50.0f; // Default value

    public bool WaypointsEnabled;

    private NavMeshAgent agent;
    private void Start()
    {
        // Get the NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError(EnemyName + ": NavMeshAgent component not found!");
        }

        moveSpeed = patrolSpeed; // uncommented this 
        rotateSpeed = 50.0f; // added this
        isAlive = true;
		InitializeBehaviorTree();
        turret = transform.GetChild(2).transform;
        bulletSpawnPoint = turret.GetChild(0).transform;
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        _IdleRoot.Evaluate();
        DetectTarget();
        if (target != null)
        {
            if (IsTargetOutOfRange())
            {
                target = null;
                targetDetected = false;
                potentialTarget = null;
            }
        }
    }

    private void InitializeBehaviorTree()
	{
		
		_MoveToLocation = new ActionNode(EPatrol);
		_FireMissile = new ActionNode(ELastStand);
		_MoveToCover = new ActionNode(ECover);
		_FireWeapon = new ActionNode(EShoot);
		
		List<Node> PatrolActions = new() { _MoveToLocation };
		_EnemyPatrol = new SequenceNode(PatrolActions);
		List<Node> LastStandActions = new() { _FireMissile };
		_EnemyLastStand = new SequenceNode(LastStandActions);
		List<Node> CoverActions = new() { _MoveToCover };
		_EnemyCover = new SequenceNode(CoverActions);
		List<Node> AttackActions = new() { _FireWeapon };
		_EnemyAttack = new SequenceNode(AttackActions);


        //Defend -> Cover or Shoot Enemy
        List<Node> DefendSelector = new() { _EnemyCover, _EnemyAttack };
        _EnemyDefend = new SelectorNode(DefendSelector);
        //Move -> Defend, Last Stand, or Patrol
        List<Node> MoveSelector = new(){ _EnemyPatrol, _EnemyDefend, _EnemyLastStand };
		_EnemyMove = new SelectorNode(MoveSelector);
		//Root -> Move
		List<Node> bTree = new(){ _EnemyMove };
		_IdleRoot = new SequenceNode(bTree);
		
	}
	//changed stuff in this method
	private NodeState EPatrol()
	{
        // The target is now set by DetectTarget() in Update()
        if (target == null)
        {
            Debug.Log(EnemyName + ": Target is " + target);
            if (WaypointsEnabled)
            {
                Patrol();
                Debug.Log(EnemyName + ": Patrolling");
                return NodeState.SUCCESS;
            }
            Debug.Log(EnemyName + ": Patrol Failed");
            return NodeState.FAILURE;
        }

        else
        {
            Debug.Log(EnemyName + ": Patrol Failed");
            return NodeState.FAILURE;
        }
    }
    /*
    if (target == null)
    {


        // This is the check to see if waypoints are set up
        if (WaypointsEnabled)
        {
            Patrol(); 
        }

        DetectTarget(); 

        Debug.Log(EnemyName + ": Patrolling");
        return NodeState.SUCCESS; 
    }
    else
    {
        return NodeState.FAILURE;
    }
}

//Activate Patrol() after implementing waypoints
if (WaypointsEnabled)
{
    Patrol();
}
DetectTarget();
Debug.Log(EnemyName+": Patrolling");
return NodeState.SUCCESS;
}
else
{
return NodeState.FAILURE;
}

}
*/
    private NodeState ELastStand()
	{
        Random.InitState((int)System.DateTime.Now.Ticks);
        int randomNumber = Random.Range(0, 100);

        if (isAlive == false && randomNumber <= 20)
        {
            //Add Function to Drop Weapon with a boolean that describes it is a bomb
            Debug.Log(EnemyName + ": BOOM");
            return NodeState.SUCCESS;
        }

        Debug.Log(EnemyName + ": Not Boom");
        return NodeState.FAILURE;
    }
	
	private NodeState ECover()
	{
        if (target == null) { Debug.LogError(EnemyName + ": Please reference target player."); return NodeState.FAILURE; }
        else
        {
            if (WaypointsEnabled)
            {
                
                if (IsNotOptimalCover())
                {
                    MoveToTarget(optimalPos);
                    Debug.Log(EnemyName + ": Take Cover");
                    return NodeState.SUCCESS;
                }
                /*
                if (Vector3.Distance(target.transform.position, transform.position) <= enemyCloseProximity)
                {
                    MoveToTarget(optimalPos);
                    Debug.Log(EnemyName + ": Take Cover");
                    return NodeState.SUCCESS;
                }
                */
                else
                {
                    Debug.Log(EnemyName + ": Take Cover Failed");
                    return NodeState.FAILURE;
                }
            }
            else 
            {
                Debug.Log(EnemyName + ": Take Cover Failed");
                return NodeState.FAILURE;
            }
        }
	}
	
	private NodeState EShoot()
	{
        if (target == null) { Debug.LogError(EnemyName+": Please reference target player."); return NodeState.FAILURE; }
        else
        {
            if (agent != null && agent.isActiveAndEnabled)
            {
                agent.SetDestination(transform.position);
            }

            UpdateWeapon();
            Debug.Log(EnemyName + ": Shoot");
            return NodeState.SUCCESS;

            /*
            if (Vector3.Distance(target.transform.position, transform.position) <= (detectionRange * (firingRangePercent / 100)))
            {
                UpdateWeapon();
                Debug.Log(EnemyName + ": Shoot");
                return NodeState.SUCCESS;
            }

            Debug.Log(EnemyName + ": Don't Shoot");
            return NodeState.FAILURE;
            */
        }
	}

    private void DetectTarget()
    {
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                
                Vector3 directionToPlayer = (hitCollider.transform.position - transform.position).normalized;
                RaycastHit hit;

                if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out hit, detectionRange))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        potentialTarget = hitCollider.gameObject;
                        break; 
                    }
                }
            }
        }

        if (potentialTarget != null)
        {
            targetDetected = true;
            target = potentialTarget;
            Debug.Log(EnemyName + ": Detected " + target);
        }

        /*
         * 
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                
                return;
            }
        }
        
        if (Physics.Raycast(transform.position, transform.forward, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                targetDetected = true;
                target = hit.collider.gameObject;
                Debug.Log(EnemyName + ": You are Detected");
                return;
            }

        }
        */
        /*
        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance <= detectionRange)
        {

            Vector3 dir = (target.transform.position - transform.position).normalized;

            if (Physics.Raycast(transform.position + Vector3.up, dir, out hit, detectionRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    targetDetected = true;
                    return;
                }
            }
        }
        */
    }


    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Count == 0) return;

        Transform patrolTarget = patrolPoints[currentPatrolIndex];

        // This is the key line that initiates movement and rotation
        MoveToTarget(patrolTarget);

        if (Vector3.Distance(transform.position, patrolTarget.position) < waypointTolerance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        }
    }

    /*
    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Count == 0) return;

        Transform patrolTarget = patrolPoints[currentPatrolIndex];
        Vector3 direction = (patrolTarget.position - transform.position).normalized;

        MoveToTarget(patrolTarget);

        if (Vector3.Distance(transform.position, patrolTarget.position) < waypointTolerance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        }
    }
    */
    private void UpdateControl()
    {
        if (target == null) return;
        targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (targetDetected == false)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turretRotSpeed * 0.2f);
        }
        /*
        else if (distance >= detectionRange * firingRangePercent/100) //Chase 
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turretRotSpeed * 0.2f);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, patrolSpeed * 1.075f * Time.deltaTime);
        }
        */

        if (targetDetected && Vector3.Distance(transform.position, target.transform.position) >= (detectionRange * firingRangePercent / 100))
        {
            if (agent != null && agent.isActiveAndEnabled)
            {
                agent.speed = patrolSpeed * 1.075f; // Chase speed
                agent.SetDestination(target.transform.position);
            }
        }
    }


    private void UpdateWeapon()
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turretRotSpeed);

        if (Time.time >= elapsedTime) //Shoot only when static
        {
            Debug.LogWarning(EnemyName + ": Shot");
            elapsedTime = Time.time + shootDelay;
            GameObject projectile = Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange * firingRangePercent/100);
    }

    private bool IsNotOptimalCover()
    {
        if (patrolPoints == null || patrolPoints.Count == 0) { return false; }
        else
        {
            int index = 0;
            optimalPos = patrolPoints[index];
            while ((Vector3.Distance(targetPos, optimalPos.position) <= enemyCloseProximity) && (Vector3.Distance(targetPos, optimalPos.position) > detectionRange))
            {
                index++;
                if (index >= patrolPoints.Count) { break; }
                else
                {
                    optimalPos = patrolPoints[index];
                }

            }

            if (optimalPos != null && Vector3.Distance(transform.position, optimalPos.position) > waypointTolerance)
            {
                return true;
            }
            return false;
        }
    }

    public void MoveToTarget(Transform currentTarget)
    {
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.speed = patrolSpeed; // Use your patrol speed
            agent.SetDestination(currentTarget.position);
        }

        // Old manual movement code
        /*
        // Enemy will move to the target waypoint
        Vector3 targetDirection = currentTarget.position - transform.position;
        // Get the rotation that faces the targetDirection
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        // Do the actual rotation by making the enemy look towards the targetRotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
        // Since it's already rotated, just make it move forward
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
        */
    }

    private bool IsTargetOutOfRange()
    {
        if (Vector3.Distance(target.transform.position, transform.position) > detectionRange)
        {
            return true;
        }
        return false;
    }
}

// IsNotOptimalCover() - Lines that are causing errors
/*
int index = 0;
//NullReferenceException is caused here.
while ((Vector3.Distance(targetPos, optimalPos.position) <= enemyCloseProximity) && (Vector3.Distance(targetPos, optimalPos.position) > detectionRange))
{

    optimalPos = patrolPoints[index];
    index++;
}
if(Vector3.Distance(transform.position, optimalPos.position) > waypointTolerance)
{
    return true;
}
return false;
}
*/