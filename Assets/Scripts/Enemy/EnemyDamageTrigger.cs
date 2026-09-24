using System;
using UnityEngine;

public class EnemyDamageTrigger : MonoBehaviour
{
    private bool hit = false;
    public event Action onHit;
    public event Action onDisable;

    private void OnEnable()
    {
        hit = false;
    }

    private void OnDisable()
    {
        onDisable?.Invoke();
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
            onHit?.Invoke();
        }
    }
}
