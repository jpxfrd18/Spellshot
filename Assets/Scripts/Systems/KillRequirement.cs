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
        if (tagName.Equals("Munitions"))
        {
            return $"Destroy {tagName} {currentCount}/{targetCount}";
        }

        return $"Kill {tagName} {currentCount}/{targetCount}";
    }
}