using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private int damage;
    private bool hit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hit && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            hit = true;
            PlayerStats.Instance.DecreaseHealth(damage);
            Destroy(gameObject);
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }

}
