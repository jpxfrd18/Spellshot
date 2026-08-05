using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float lifetime;
    private float timer = 0f;
    private bool cancelled = false;

    private void Update()
    {
        if (cancelled)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    public void Cancel()
    {
        cancelled = true;
    }

    public void AddTime(float additionalTime)
    {
        lifetime += additionalTime;
    }
}
