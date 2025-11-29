using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField]
    private GameObject bullet;

    private Transform turret;
    private Transform bulletSpawnPoint;
    private float turretRotSpeed = 10.0f;
    private Vector3 targetPos;

    [SerializeField]
    private GameObject target; 

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
    [SerializeField] private float detectionRange = 15f;
    private bool targetDetected = false;

    private void Start()
    {
        turret = transform.GetChild(0).transform;
        bulletSpawnPoint = turret.GetChild(0).transform;
    }

    private void Update()
    {
        DetectTarget();

        if (targetDetected)
        {
            UpdateControl();
            UpdateWeapon();
        }
        else
        {
            Patrol();
        }
    }

    private void DetectTarget()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance <= detectionRange)
        {
            RaycastHit hit;
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

        targetDetected = false;
    }

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Count == 0) return;

        Transform patrolTarget = patrolPoints[currentPatrolIndex];
        Vector3 direction = (patrolTarget.position - transform.position).normalized;

        transform.position = Vector3.MoveTowards(transform.position, patrolTarget.position, patrolSpeed * Time.deltaTime);
        Quaternion targetRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 2f);

        if (Vector3.Distance(transform.position, patrolTarget.position) < waypointTolerance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        }
    }

    private void UpdateControl()
    {
        targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (targetDetected == false)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turretRotSpeed * 0.2f);
        }
        else if (distance >= detectionRange * firingRangePercent/100) //Chase 
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turretRotSpeed * 0.2f);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, patrolSpeed* 1.075f * Time.deltaTime);
        }
    }


    private void UpdateWeapon()
    {
        if (!targetDetected || target == null) return;
        float distance = Vector3.Distance(transform.position, target.transform.position);
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, targetRotation, Time.deltaTime * turretRotSpeed);

        if (Time.time >= elapsedTime && distance < detectionRange * firingRangePercent/100) //Shoot only when static
        {
            elapsedTime = Time.time + shootDelay;
            Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange * firingRangePercent/100);
    }
}
