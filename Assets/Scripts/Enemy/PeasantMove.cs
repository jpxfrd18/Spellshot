using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PeasantMove : MonoBehaviour
{
    private Transform player;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform root;
    [SerializeField] private float moveDistance;

    [Header("Player Repulsion")]
    [SerializeField] private float playerStrengthMin;
    [SerializeField] private float playerStrengthMax;

    [Header("Peasant Repulsion")]
    [SerializeField] private float peasantStrengthMin;
    [SerializeField] private float peasantStrengthMax;
    [SerializeField] private float peasantCombinedStrengthMax;

    [Header("Noise")]
    [SerializeField] private float noiseMin;
    [SerializeField] private float noiseMax;
    [SerializeField] private float noiseResetTime;
    [Header("Wall Repulsion")]
    [SerializeField] private float wallRepulsionStrength;

    private Vector3 noise = Vector3.zero;
    private float noiseTimer = 0;
    public Transform Root => root;
    private float turnSpeed;
    private HashSet<Transform> peasants;


    private void Start()
    {
        if (playerStrengthMin > playerStrengthMax)
        {
            playerStrengthMin = playerStrengthMax;
        }

        player = PlayerSpawner.Instance.playerTransform;
        peasants = new HashSet<Transform>();
    }


    private void Update()
    {
        if (RespawnManager.Instance.IsPlayerDead || PauseController.Instance.isPaused)
        {
            return;
        }

        Vector3 playerHorizontal = new Vector3(player.position.x, root.position.y, player.position.z);
        Vector3 distance = root.position - playerHorizontal;

        if (distance.sqrMagnitude <= 0)
        {
            return;
        }

        Vector3 directionAway = distance.normalized;

        float fleeStrength = Mathf.Clamp(1 / distance.sqrMagnitude, playerStrengthMin, playerStrengthMax);

        Vector3 playerRepulsion = directionAway * fleeStrength;


        Vector3 peasantRepulsion = Vector3.zero;

        HashSet<Transform> toRemovePeasant = new HashSet<Transform>();

        foreach (var p in peasants)
        {
            if (!p)
            {
                toRemovePeasant.Add(p);
                continue;
            }

            Vector3 peasantHorizontal = new Vector3(p.position.x, root.position.y, p.position.z);
            Vector3 peasantDistance = root.position - peasantHorizontal;

            float distanceMagnitude = Mathf.Max(peasantDistance.magnitude, 0.05f);

            Vector3 peasantAway = peasantDistance / distanceMagnitude;

            float strength = Mathf.Clamp(1 / distanceMagnitude, peasantStrengthMin, peasantStrengthMax);

            peasantRepulsion += peasantAway * strength;

        }

        foreach (var p in toRemovePeasant)
        {
            peasants.Remove(p);
        }

        peasantRepulsion = Vector3.ClampMagnitude(peasantRepulsion, peasantCombinedStrengthMax);

        if (noiseTimer >= noiseResetTime)
        {
            Vector2 circle = Random.insideUnitCircle;
            float magnitude = Random.Range(noiseMin, noiseMax);
            noise = new Vector3(circle.x, 0, circle.y).normalized * magnitude;
            noiseTimer = 0;
        }

        noiseTimer += Time.deltaTime;

        Vector3 final = playerRepulsion + peasantRepulsion + noise;
        Vector3 target = root.position + final.normalized * moveDistance;

        NavMeshHit hit;
        if (agent.Raycast(target, out hit))
        {
            float wallDistance = Vector3.Distance(root.position, hit.position);
            float strength = wallRepulsionStrength * Mathf.Clamp01(1f - (wallDistance / moveDistance));


            Vector3 wallRepulsion = hit.normal * strength;
            final += wallRepulsion;
            target = root.position + final.normalized * moveDistance;
        }

        agent.SetDestination(target);
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || RespawnManager.Instance.IsPlayerDead)
        {
            return;
        }

        if (other.CompareTag("Peasant"))
        {
            peasants.Add(other.attachedRigidbody.gameObject.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (other.CompareTag("Peasant"))
        {
            peasants.Remove(other.attachedRigidbody.gameObject.transform);
        }
    }
}
