using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float lifetime;
    private bool disabled = false;
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();
    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= lifetime)
        {
            disabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (disabled || other.isTrigger || other.attachedRigidbody == null)
        {
            return;
        }

        // If target was already hit, don't hit them
        if (!hitTargets.Add(other.attachedRigidbody.gameObject))
        {
            return;
        }

        if (other.attachedRigidbody.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyHealth enemyHealth = other.attachedRigidbody.gameObject.GetComponent<EnemyHealth>();

            if (enemyHealth)
            {
                enemyHealth.DecreaseHealth(damage);
            }
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerStats playerStats = PlayerStats.Instance;

            if (playerStats)
            {
                playerStats.DecreaseHealth(damage);
            }
        }

    }
}
