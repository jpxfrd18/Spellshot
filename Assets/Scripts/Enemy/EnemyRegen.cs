using UnityEngine;

public class EnemyRegen : MonoBehaviour
{
    [SerializeField] private float regenFrequency;
    private float regenTimer = 0f;
    private EnemyHealth health;

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
    }

    private void Update()
    {
        regenTimer += Time.deltaTime;

        if (regenTimer >= regenFrequency)
        {
            health.IncreaseHealth(1);
            regenTimer = 0f;
        }
    }
}
