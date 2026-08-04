using UnityEngine;

public class ProjectileSpell : SpellModule
{
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected int manaCost;
    [SerializeField] protected float initialSpeed;
    public override SpellCastType CastType { get; } = SpellCastType.Press;

    public override void Cast(Player player)
    {
        if (player.Stats.DecreaseMana(manaCost))
        {
            GameObject projectile = Instantiate(projectilePrefab, player.AimSource, Quaternion.LookRotation(player.AimForward));

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.linearVelocity = initialSpeed * player.AimForward;
        }
    }
}