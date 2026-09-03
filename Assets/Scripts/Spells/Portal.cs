using UnityEngine;

public class Portal : SpellModule
{
    public override SpellCastType CastType { get; } = SpellCastType.Press;
    [SerializeField] private int manaCost;
    [SerializeField] private float minDistance;
    public override void Cast(Player player)
    {
        Enemy enemy = FindEnemy(player);
        if (enemy == null)
        {
            return;
        }

        if (player.Stats.DecreaseMana(manaCost))
        {
            player.Motor.Teleport(enemy.enemyTransform.position);
        }
    }

    private Enemy FindEnemy(Player player)
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        if (enemies.Length == 0)
        {
            return null;
        }

        float closestDistance = float.MaxValue;
        Enemy closestEnemy = null;

        foreach (Enemy e in enemies)
        {
            float distance = Vector3.Distance(player.AimSource, e.enemyTransform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = e;
            }

            if (distance < minDistance)
            {
                return null;
            }
        }

        return closestEnemy;
    }
}
