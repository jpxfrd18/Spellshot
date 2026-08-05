using UnityEngine;

public class Objective : MonoBehaviour
{
    [SerializeField] private Requirement[] requirements;
    private bool isComplete;

    /// <summary>
    /// Calls EnemyKilled on all requirements in the objective.
    /// </summary>
    /// <param name="tagName">The semantic tag of the enemy that died.</param>
    /// <returns>
    /// True if this objective is completed.
    /// </returns>
    public bool EnemyKilled(string tag)
    {
        isComplete = true;
        foreach (Requirement req in requirements)
        {
            if (!req.EnemyKilled(tag))
            {
                isComplete = false;
            }
        }

        return isComplete;
    }

    /// <summary>
    /// Calls PlayerReached on all requirements in the objective.
    /// </summary>
    /// <param name="tagName">The semantic tag of trigger that the player reached.</param>
    /// <returns>
    /// True if this objective is completed.
    /// </returns>
    public bool PlayerReached(string tag)
    {
        isComplete = true;
        foreach (Requirement req in requirements)
        {
            if (!req.PlayerReached(tag))
            {
                isComplete = false;
            }
        }

        return isComplete;
    }

    public void Reset()
    {
        foreach (Requirement req in requirements)
        {
            req.Reset();
        }
    }

    public override string ToString()
    {
        if (requirements.Length == 1)
        {
            return requirements[0].ToString();
        }

        string result = "";
        foreach (Requirement req in requirements)
        {
            result += req.ToString() + " AND ";
        }

        // Remove the trailing " AND "
        return result.Substring(0, result.Length - 5);
    }
}