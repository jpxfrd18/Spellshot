using System.Collections;
using UnityEngine;

public class RangedAttack : MonoBehaviour
{
    [SerializeField] private Transform eye;
    [SerializeField] private Transform root;
    [SerializeField] private Transform projectileSpawn;
    [SerializeField] private float deaggroFallback;
    [SerializeField] private float deaggroDelay;
    [SerializeField] private float interestDelay;
    [SerializeField] private float turnSpeed;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float speed;
    private bool aggro = false;
    private bool interest = false;
    private GameObject player = null;
    private float interestTimer = 0f;
    private float deaggroTimer = 0f;
    private EnemyAttack attack;
    private int layer;
    private float randomDelay = 0f;
    private bool waitingForAggro = false;

    private void Awake()
    {
        attack = GetComponentInParent<EnemyAttack>();
        layer = LayerMask.GetMask("Ground", "Player", "EnemyWall");
    }


    private void Update()
    {
        if (RespawnManager.Instance.IsPlayerDead || PauseController.Instance.isPaused)
        {
            return;
        }


        if (aggro)
        {
            Vector3 playerHorizontal = new Vector3(player.transform.position.x, root.transform.position.y, player.transform.position.z);
            Rotate(playerHorizontal);

            if (!attack.isAttacking)
            {
                attack.Attack();
            }

            if (deaggroTimer <= 0f && Mathf.Abs((playerHorizontal - root.transform.position).magnitude) > deaggroFallback)
            {
                deaggroTimer = deaggroDelay;
            }

            if (deaggroTimer > 0f)
            {
                deaggroTimer -= Time.deltaTime;

                if (deaggroTimer <= 0f)
                {
                    aggro = false;
                    interest = false;
                }
            }
        }
        else if (interest)
        {
            if (interestTimer <= 0f)
            {
                interestTimer = interestDelay;
                if (CheckLineOfSight())
                {
                    randomDelay = Random.Range(0.05f, 0.5f);
                    StartCoroutine(AggroDelay());
                    interest = false;
                    waitingForAggro = true;
                }
            }

            interestTimer -= Time.deltaTime;
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
            if (deaggroTimer <= 0f)
            {
                deaggroTimer = deaggroDelay;
            }
        }
    }

    private bool CheckLineOfSight()
    {
        Vector3 direction = (player.transform.position - eye.transform.position).normalized;

        if (Physics.Raycast(eye.transform.position, direction, out RaycastHit hit, 100f, layer))
        {
            return hit.collider.gameObject.layer == LayerMask.NameToLayer("Player");
        }

        return false;
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

    private void Interest(GameObject playerObj)
    {
        if (aggro || interest)
        {
            return;
        }

        player = playerObj;
        interest = true;
    }

    private IEnumerator AggroDelay()
    {
        if (waitingForAggro)
        {
            yield break;
        }

        yield return new WaitForSeconds(randomDelay);
        aggro = true;
        waitingForAggro = false;
    }

    public void Fire()
    {
        Vector3 direction = player.transform.position - projectileSpawn.position;
        Quaternion rot = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);
        GameObject arrow = Instantiate(projectile, projectileSpawn.transform.position, rot);
        arrow.GetComponent<Rigidbody>().linearVelocity = direction.normalized * speed;
    }
}
