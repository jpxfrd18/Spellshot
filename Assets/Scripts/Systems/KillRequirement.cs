public class KillRequirement : Requirement
{
    public override bool EnemyKilled(string tag)
    {
        if (tag == tagName)
        {
            currentCount++;

            if (currentCount >= targetCount)
            {
                isComplete = true;
            }
        }
        return isComplete;
    }

    public override string ToString()
    {
        return $"Kill {tagName} {currentCount}/{targetCount}";
    }
}