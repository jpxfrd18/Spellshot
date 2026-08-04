using System.Collections.Generic;
using UnityEngine;


public class ForceLanceController : MonoBehaviour
{
    [SerializeField] private int damage = 15;
    HashSet<Rigidbody> hitEnemies = new HashSet<Rigidbody>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (other.attachedRigidbody == null)
            {
                return;
            }

            if (hitEnemies.Contains(other.attachedRigidbody))
            {
                Debug.Log("Already hit: " + other.attachedRigidbody.name);
                return;
            }

            hitEnemies.Add(other.attachedRigidbody);
            Debug.Log("Hit: " + other.attachedRigidbody.name);

            EnemyHealth enemyHealth = other.attachedRigidbody.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.DecreaseHealth(damage);
            }
            else
            {
                Debug.LogWarning("No Enemy Health found on " + other.attachedRigidbody.name);
            }
        }
    }
}
