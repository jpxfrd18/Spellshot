using UnityEngine;

public abstract class Requirement : MonoBehaviour
{
    [SerializeField] protected string tagName;
    [SerializeField] protected int targetCount;
    protected int currentCount;
    protected bool isComplete;

    /// <summary>
    /// Increments the kill count when an enemy with the matching tag dies.
    /// </summary>
    /// <param name="tag">The semantic tag of the enemy that died.</param>
    /// <returns>
    /// Whether or not the requirement is completed.
    /// </returns>
    public virtual bool EnemyKilled(string tag) { return isComplete; }

    /// <summary>
    /// Completes the requirement when a trigger of the matching tag is triggered
    /// </summary>
    /// <param name="tag">The semantic tag of the trigger collider.</param>
    /// <returns>
    /// Whether or not the requirement is completed.
    /// </returns>
    public virtual bool PlayerReached(string tag) { return isComplete; }

    /// <summary>
    /// Resets the requirement
    /// </summary>
    public virtual void Reset()
    {
        currentCount = 0;
        isComplete = false;
    }
}
