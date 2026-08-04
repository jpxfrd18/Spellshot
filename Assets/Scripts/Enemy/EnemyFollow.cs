using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    private bool aggro = false;
    private bool interest = false;
    private GameObject player = null;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private bool defensive;
    [SerializeField] private float idealDistance;
    [SerializeField] private Transform root;
    [SerializeField] private Transform eye;
    [SerializeField] private float deaggroDelay;
    [SerializeField] private float interestDelay;
    [SerializeField] private float deaggroFallback;
    private float deaggroTimer = 0f;
    private float interestTimer = 0f;
    private float turnSpeed;
    private EnemyAttack attack;
    private EnemyHealth health;
    private int layer;


    private void Awake()
    {
        turnSpeed = agent.angularSpeed * Mathf.Deg2Rad;
        attack = GetComponentInParent<EnemyAttack>();
        health = GetComponentInParent<EnemyHealth>();
        layer = LayerMask.GetMask("Ground", "Player", "EnemyWall");

        if (defensive)
        {
            agent.updateRotation = false;
            agent.updatePosition = false;
        }
    }

    private void OnEnable()
    {
        health.OnDamageTaken += HandleDamageTaken;
    }

    private void OnDisable()
    {
        health.OnDamageTaken -= HandleDamageTaken;
    }

    private void Interest(GameObject p)
    {
        if (aggro)
        {
            return;
        }

        player = p;
        interest = true;
    }

    private void EnterAggro()
    {
        aggro = true;
        deaggroTimer = 0f;
    }

    private void LeaveAggro()
    {
        aggro = false;
        agent.isStopped = true;
        agent.ResetPath();
        player = null;
        interest = false;
    }

    private void Update()
    {
        if (RespawnManager.Instance.IsPlayerDead || PauseController.Instance.isPaused)
        {
            return;
        }

        if (defensive)
        {
            Defensive();
            return;
        }

        Agressive();
    }

    private void Defensive()
    {
        if (player)
        {
            Vector3 playerHorizontal = new Vector3(player.transform.position.x, root.position.y, player.transform.position.z);
            Rotate(playerHorizontal);

            CapsuleCollider collider = player.GetComponent<CapsuleCollider>();
            Vector3 closest = collider.ClosestPoint(root.position);
            float distance = Vector3.Distance(root.position, closest);

            if (!attack.isAttacking && distance <= idealDistance)
            {
                attack.Attack();
            }
        }
    }

    private void Agressive()
    {
        if (aggro)
        {
            Aggro();
        }
        else if (interest)
        {
            if (interestTimer <= 0)
            {
                interestTimer = interestDelay;
                if (CheckLineOfSight())
                {
                    EnterAggro();
                }
            }

            interestTimer -= Time.deltaTime;
        }
    }

    private void Aggro()
    {
        Vector3 playerHorizontal = new Vector3(player.transform.position.x, root.position.y, player.transform.position.z);
        Vector3 distance = playerHorizontal - root.position;

        if (distance.magnitude > idealDistance && !attack.isAttacking)
        {
            //Find nearest point on navmesh to player and pathfind to it.
            NavMeshHit hit;
            NavMesh.SamplePosition(player.transform.position, out hit, 5f, NavMesh.AllAreas);

            if (float.IsInfinity(hit.position.x) || float.IsNaN(hit.position.x))
            {
                hit.position = player.transform.position;
            }

            Vector3 target = hit.position;

            //Check if path is valid
            NavMeshPath path = new NavMeshPath();
            bool hasPath = agent.CalculatePath(target, path);
            bool reachable = hasPath && path.status == NavMeshPathStatus.PathComplete;

            if (reachable)
            {
                if ((agent.nextPosition - root.position).sqrMagnitude > 0.001f)
                {
                    agent.Warp(root.position);
                    agent.velocity = Vector3.zero;
                }

                agent.avoidancePriority = 50;
                agent.updateRotation = true;
                agent.updatePosition = true;
                agent.SetDestination(target);
            }
            else
            {
                NavMeshHit edge;
                if (agent.Raycast(target, out edge))
                {
                    if (path.status == NavMeshPathStatus.PathPartial &&
                        agent.remainingDistance <= agent.stoppingDistance + 0.05f)
                    {
                        agent.updateRotation = false;
                        agent.updatePosition = false;
                    }
                    else
                    {
                        agent.updateRotation = true;
                        agent.updatePosition = true;
                        agent.SetDestination(edge.position);
                    }
                }
                else if (path.status == NavMeshPathStatus.PathPartial)
                {
                    if (agent.remainingDistance <= agent.stoppingDistance + 0.05f)
                    {
                        agent.updateRotation = false;
                        agent.updatePosition = false;
                    }
                    else
                    {
                        agent.updateRotation = true;
                        agent.updatePosition = true;
                        agent.SetDestination(path.corners[path.corners.Length - 1]);
                    }
                }
                else
                {
                    agent.updateRotation = false;
                    agent.updatePosition = false;
                }
            }
        }
        else
        {
            agent.updateRotation = false;
            agent.updatePosition = false;
            agent.avoidancePriority = 1;
            agent.nextPosition = root.position;

            Vector3 animationVelocity = attack.animationDelta / Time.deltaTime;
            agent.velocity = animationVelocity + transform.right * 0.001f;

            if (!attack.isAttacking)
            {
                attack.Attack();
            }
        }

        //Failsafe
        if ((player.transform.position - root.position).magnitude > deaggroFallback)
        {
            LeaveAggro();
        }

        if (!agent.updateRotation)
        {
            Rotate(playerHorizontal);
        }

        if (deaggroTimer > 0)
        {
            deaggroTimer -= Time.deltaTime;
            if (deaggroTimer <= 0)
            {
                LeaveAggro();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (gameObject.layer == LayerMask.NameToLayer("EnemyAggro") &&
           other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Interest(other.attachedRigidbody.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (gameObject.layer == LayerMask.NameToLayer("EnemyAggro") &&
           other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            deaggroTimer = deaggroDelay;
        }
    }

    private void Rotate(Vector3 playerHorizontal)
    {
        Vector3 desiredDirection = (playerHorizontal - root.position).normalized;

        Vector3 newDirection = Vector3.RotateTowards(root.forward,
                                                     desiredDirection,
                                                     turnSpeed * Time.deltaTime,
                                                     0f);
        newDirection.Normalize();

        root.rotation = Quaternion.LookRotation(newDirection);
    }

    private bool CheckLineOfSight()
    {
        Vector3 direction = (player.transform.position - eye.transform.position).normalized;

        if (Physics.Raycast(eye.transform.position, direction, out RaycastHit hit, 40f, layer))
        {
            return hit.collider.gameObject.layer == LayerMask.NameToLayer("Player");
        }

        return false;
    }

    private void HandleDamageTaken()
    {
        if (defensive)
        {
            defensive = false;
        }

        if (!aggro)
        {
            EnterAggro();
        }
    }
}
