using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform enemyTransform { get; private set; }

    private void Awake()
    {
        enemyTransform = transform;
    }
}
