using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Player player;
    private EnemyHealth enemyHealth;
    [SerializeField] MeshRenderer healthBar;
    [SerializeField] MeshRenderer healthBarBackground;


    private void Start()
    {
        enemyHealth = GetComponentInParent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.OnDamageTaken += UpdateHealthDisplay;
        }

        player = Player.Instance;
    }

    private void Update()
    {
        if (player != null)
        {
            Vector3 direction = transform.position - player.AimSource;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void UpdateHealthDisplay()
    {
        healthBar.enabled = enemyHealth.currentHealth > 0;
        healthBarBackground.enabled = enemyHealth.currentHealth > 0;
    }

    private void OnDestroy()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnDamageTaken -= UpdateHealthDisplay;
        }
    }
}
