using UnityEngine;

public class MagicMissileController : MonoBehaviour
{
    private GameObject enemy = null;
    private Rigidbody rb;
    private bool hasSlowed = false;

    [SerializeField] private float turnRate;
    [SerializeField] private float velocityReduction;
    [SerializeField] private int damage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (enemy)
        {
            Vector3 desiredDirection = (enemy.transform.position - transform.position).normalized;

            Vector3 newDirection = Vector3.RotateTowards(rb.linearVelocity.normalized,
                                                         desiredDirection,
                                                         turnRate * Time.fixedDeltaTime,
                                                         0f);
            newDirection.Normalize();



            rb.linearVelocity = newDirection * rb.linearVelocity.magnitude;
            rb.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();

            if (enemyHealth)
            {
                enemyHealth.DecreaseHealth(damage);
            }

        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            EnemyHealth enemyHealth = collision.gameObject.GetComponentInParent<EnemyHealth>();

            if (enemyHealth)
            {
                enemyHealth.DecreaseHealth(damage);
            }
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("EnemyHoming"))
        {
            return;
        }

        if (!enemy)
        {
            Vector3 direction = (other.gameObject.transform.position - transform.position).normalized;

            if (Vector3.Angle(direction, rb.linearVelocity) > 45f)
            {
                return;
            }

            enemy = other.gameObject;

            DestroyAfterTime destroyAfterTime = GetComponent<DestroyAfterTime>();
            if (destroyAfterTime)
            {
                destroyAfterTime.AddTime(3f);
            }

            if (!hasSlowed)
            {
                rb.linearVelocity *= velocityReduction;
                hasSlowed = true;
            }
        }
    }
}
