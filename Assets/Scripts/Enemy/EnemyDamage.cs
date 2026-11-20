using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private int damage;
    private bool hit = false;

    private void OnEnable()
    {
        hit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (!hit && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            hit = true;
            PlayerStats.Instance.DecreaseHealth(damage);
        }
    }
}
