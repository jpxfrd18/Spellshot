using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageController : MonoBehaviour
{
    [SerializeField] List<EnemyDamageTrigger> triggers;
    [SerializeField] private int damage;
    private bool canHit = true;

    private void Start()
    {
        foreach (EnemyDamageTrigger trigger in triggers)
        {
            trigger.onHit += HandleHit;
            trigger.onDisable += HandleDisable;
        }
    }

    private void HandleHit()
    {
        if (canHit)
        {
            PlayerStats.Instance.DecreaseHealth(damage);
            canHit = false;
        }
    }

    // Assumes that all triggers are disabled on the same frame
    private void HandleDisable()
    {
        canHit = true;
    }
}