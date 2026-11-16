using UnityEngine;
using UnityEngine.SceneManagement;

public class ForceLance : ProjectileSpell
{
    public override void Cast(Player player)
    {
        if (!player.Stats.DecreaseMana(manaCost))
        {
            if (player.Stats.DecreaseHealth(2 * manaCost))
            {
                return;
            }
        }

        GameObject projectile = Instantiate(projectilePrefab, player.AimSource, Quaternion.LookRotation(player.AimForward) * Quaternion.Euler(90, 0, 0));
        SceneManager.MoveGameObjectToScene(projectile, gameObject.scene);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.linearVelocity = initialSpeed * player.AimForward;
    }
}
