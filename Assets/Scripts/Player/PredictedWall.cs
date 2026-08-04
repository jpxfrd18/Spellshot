using System.Collections.Generic;
using UnityEngine;


public class PredictedWall : MonoBehaviour
{
    //Exposing the mega normal for player motor but is cleared in different cases.
    public Vector3 wallNormal { get; private set; } = Vector3.zero;
    public bool triangleSoup { get; private set; } = false;

    private SphereCollider col;
    private PlayerRef playerRef;
    private Vector3 normalLastFrame = Vector3.zero;
    private bool nudgedThisFrame = false;
    private float nudgeAmount = 0.1f;
    private bool failedLastFrame = false;
    private HoverState hoverState = HoverState.None;
    private int mask;

    enum HoverState
    {
        None,
        Nudging,
        AntiGravity,
        FallbackAntiGravity
    }

    private void Awake()
    {
        col = GetComponent<SphereCollider>();
        playerRef = GetComponentInParent<PlayerRef>();
        mask = LayerMask.GetMask("Ground", "Enemy");
    }

    public void PredictPosition(Vector3 currentVelocity, Vector3 jumpImpulse, Vector3 appliedAcceleration, float damping)
    {
        //Gravity
        Vector3 predictedVelocity = currentVelocity + Physics.gravity * Time.fixedDeltaTime;

        //Jump Impulse (mass is 1)
        predictedVelocity += jumpImpulse;

        //Move Direction Acceleration
        predictedVelocity += appliedAcceleration * Time.fixedDeltaTime;

        //Damping
        predictedVelocity = predictedVelocity * (1 - damping * Time.fixedDeltaTime);

        //Predict Position
        Vector3 predictedPosition = col.transform.TransformPoint(col.center) + predictedVelocity * Time.fixedDeltaTime;

        FindBadVelocity(predictedPosition, currentVelocity);
    }


    private void FindBadVelocity(Vector3 predictedPosition, Vector3 currentVelocity)
    {
        Vector3[] directions = new Vector3[]
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right,
            Vector3.forward,
            Vector3.back,
            new Vector3(1, 1, 0).normalized,
            new Vector3(-1, 1, 0).normalized,
            new Vector3(1, -1, 0).normalized,
            new Vector3(-1, -1, 0).normalized,
            new Vector3(0, 1, 1).normalized,
            new Vector3(0, 1, -1).normalized,
            new Vector3(0, -1, 1).normalized,
            new Vector3(0, -1, -1).normalized,
            new Vector3(1, 0, 1).normalized,
            new Vector3(-1, 0, 1).normalized,
            new Vector3(1, 0, -1).normalized,
            new Vector3(-1, 0, -1).normalized
        };

        Collider[] futureOverlappingColliders = Physics.OverlapSphere(
            predictedPosition,
            col.radius,
            mask,
            QueryTriggerInteraction.Ignore
        );

        for (int i = futureOverlappingColliders.Length - 1; i >= 0; i--)
        {
            var col = futureOverlappingColliders[i];

            if (!col || col.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                continue;
            }

            if (!col.gameObject.TryGetComponent<Ground>(out _))
            {
                futureOverlappingColliders[i] = null;
            }
        }

        List<Vector3> normals = new List<Vector3>();

        foreach (Collider collider in futureOverlappingColliders)
        {
            if (collider == null)
            {
                continue;
            }

            foreach (Vector3 dir in directions)
            {
                Vector3 samplePoint = predictedPosition + dir * col.radius;
                Vector3 closestPoint = collider.ClosestPoint(samplePoint);
                Vector3 normal = samplePoint - closestPoint;

                if (normal.y < 0f)
                {
                    continue;
                }

                float angle = Vector3.Angle(normal, Vector3.up);
                if (angle < playerRef.playerStats.persistant.maxSlopeAngle)
                {
                    continue;
                }

                if (Vector3.Dot(normal, currentVelocity) < 0)
                {
                    normals.Add(normal);
                    //Debug.Log("[Frame " + Time.frameCount + "] Normal: " + normal + ", Angle: " + angle);
                }
            }
        }

        nudgedThisFrame = false;

        if (normals.Count > 1)
        {
            bool isValid = ClusterVarianceCheck(normals);

            if (isValid)
            {
                Vector3 megaNormal = Vector3.zero;
                foreach (Vector3 n in normals)
                {
                    megaNormal += n;
                }

                megaNormal.Normalize();
                float dot = Vector3.Dot(megaNormal, currentVelocity.normalized);

                if (dot > 0f)
                {
                    Vector3 badVelocity = dot * megaNormal;
                    Vector3 goodVelocity = currentVelocity - badVelocity;

                    //Debug.Log("[Frame " + Time.frameCount + "] Current Velocity: " + currentVelocity + ", Bad Velocity: " + badVelocity + ", Good Velocity: " + goodVelocity);
                    playerRef.playerMotor.SetVelocity(goodVelocity);
                }

                triangleSoup = false;
                normalLastFrame = megaNormal;
                wallNormal = megaNormal;
            }
            else if (failedLastFrame)
            {
                if (hoverState == HoverState.None)
                {
                    hoverState = HoverState.Nudging;
                }

                if (hoverState != HoverState.FallbackAntiGravity)
                {
                    NudgeUpwards(currentVelocity);
                    nudgedThisFrame = true;
                }

                normalLastFrame = Vector3.zero;
                wallNormal = Vector3.zero;
                triangleSoup = true;
                failedLastFrame = false;
            }
            else
            {
                failedLastFrame = true;
            }
        }
        else
        {
            failedLastFrame = false;
            wallNormal = Vector3.zero;
        }

        if (hoverState == HoverState.None)
        {
            nudgeAmount = 0.1f;
        }

        if (hoverState == HoverState.Nudging && nudgedThisFrame == false)
        {
            hoverState = HoverState.AntiGravity;
        }

        if (hoverState == HoverState.AntiGravity)
        {
            AntiGravity(currentVelocity);
            hoverState = HoverState.None;
        }

        if (hoverState == HoverState.FallbackAntiGravity)
        {
            FallbackAntiGravity();
        }
    }

    private bool ClusterVarianceCheck(List<Vector3> normals)
    {
        if (normals == null || normals.Count < 2)
        {
            //Debug.Log("[Frame " + Time.frameCount + "] IsValid = false. Invalid input for clustering. Normals count: "
            //+ (normals != null ? normals.Count : 0));
            return false;
        }

        List<List<Vector3>> clusters = CreateClusters(normals);

        // for (int i = 0; i < clusters.Count; i++)
        // {
        //     Debug.Log("[Frame " + Time.frameCount + "] Cluster[" + i + "] Cluster Count: " + clusters[i].Count + ", First Normal: " + clusters[i][0]);
        // }

        //Emergency Fallback if all clusters have 1 normal
        if (clusters[0].Count == 1 && normalLastFrame.sqrMagnitude > 0.01)
        {
            //Debug.Log("[Frame " + Time.frameCount + "] Emergency Fallback attempted");
            foreach (List<Vector3> cluster in clusters)
            {
                if (SameSlope(normalLastFrame, cluster[0]))
                {
                    //Debug.Log("[Frame " + Time.frameCount + "] Emergency Fallback, mega normal = " + cluster[0]);
                    return true;
                }
            }
        }

        //Allow the cluster to pass if its the only one and there is no temporal normal data
        if (clusters.Count == 1 && normalLastFrame.sqrMagnitude < 0.01 && clusters[0].Count > 2)
        {
            //Debug.Log("[Frame " + Time.frameCount + "] Single cluster found, and no temporal data exists. Is Valid = true");
            return true;
        }

        //Dominance is how much bigger the largest cluster is compared to smallest
        int dominanceAmount = 0;
        bool dominanceFallback = false;
        if (clusters.Count > 1)
        {
            dominanceAmount = clusters[0].Count - clusters[1].Count;
        }

        //If small dominance, then use the dominance fallback which is temporal consistency check
        if (dominanceAmount < 2 && normalLastFrame.sqrMagnitude > 0
        && SameSlope(normalLastFrame, clusters[0][0]))
        {
            dominanceFallback = true;
            //Debug.Log("[Frame " + Time.frameCount + "] Dominance Fallback");
        }

        //Remove all clusters with only 1 normal
        for (int i = clusters.Count - 1; i > 0; i--)
        {
            if (clusters[i].Count == 1)
            {
                //Debug.Log("[Frame " + Time.frameCount + "] Removing cluster[" + i + "]");
                normals.Remove(clusters[i][0]);
                clusters.RemoveAt(i);
            }
            else
            {
                break;
            }
        }

        //Strict voting is > comparison if the loose voting wouldn't remove anything
        bool strictVoting = clusters[0].Count < 3 * clusters[clusters.Count - 1].Count;

        //Remove garbage clusters via voting filtering system
        for (int i = clusters.Count - 1; i > 0; i--)
        {
            if (!strictVoting && (clusters[i].Count == 2 || clusters[i].Count * 3 < clusters[0].Count))
            {
                //Debug.Log("[Frame " + Time.frameCount + "] Removing cluster[" + i + "]");
                foreach (Vector3 n in clusters[i])
                {
                    //Debug.Log("[Frame " + Time.frameCount + "] Removing outlier normal: " + n);
                    normals.Remove(n);
                }
                clusters.RemoveAt(i);
            }
            else if (strictVoting && clusters[i].Count < 5 && clusters[i].Count < clusters[0].Count)
            {
                //Debug.Log("[Frame " + Time.frameCount + "] Removing cluster[" + i + "]");

                foreach (Vector3 n in clusters[i])
                {
                    //Debug.Log("[Frame " + Time.frameCount + "] Removing outlier normal: " + n);
                    normals.Remove(n);
                }
                clusters.RemoveAt(i);
            }
            else
            {
                break;
            }
        }

        //Dominance check if only 1 cluster survives the voting filtering
        if (clusters.Count == 1)
        {
            //Debug.Log("[Frame " + Time.frameCount + "] Only one cluster found. IsValid: " + (dominanceAmount > 2 || dominanceFallback));
            return dominanceAmount > 2 || dominanceFallback;
        }

        //False cluster check if multiple clusters survive with large clusters automatically being valid (different angles same y value)
        RemoveFalseClusters(clusters, normals);

        //If all clusters are removed doing to being false clusters then not valid
        if (clusters.Count == 0)
        {
            //Debug.Log("[Frame " + Time.frameCount + "] No valid clusters found, isValid = false");
            return false;
        }

        //Debug.Log("[Frame " + Time.frameCount + "] Is Valid = true");
        return true;
    }

    private List<List<Vector3>> CreateClusters(List<Vector3> normals)
    {
        List<List<Vector3>> clusters = new List<List<Vector3>>();

        clusters.Add(new List<Vector3>());
        clusters[0].Add(normals[0]);

        //Cluster angles by how similar their angles to up are
        for (int i = 1; i < normals.Count; i++)
        {
            bool clustered = false;

            foreach (List<Vector3> cluster in clusters)
            {
                if (SameSlope(cluster[0], normals[i]))
                {
                    cluster.Add(normals[i]);
                    clustered = true;
                    break;
                }
            }

            if (!clustered)
            {
                clusters.Add(new List<Vector3>());
                clusters[clusters.Count - 1].Add(normals[i]);
            }

        }

        clusters.Sort((a, b) => b.Count.CompareTo(a.Count));
        return clusters;
    }

    private void RemoveFalseClusters(List<List<Vector3>> clusters, List<Vector3> normals)
    {
        for (int i = clusters.Count - 1; i >= 0; i--)
        {
            for (int j = i - 1; j >= 0; j--)
            {
                if (Mathf.Abs(clusters[i][0].y - clusters[j][0].y) < 0.05f)
                {
                    if (clusters[i].Count >= 10)
                    {
                        //Debug.Log("[Frame " + Time.frameCount + "] Removing cluster[" + j + "] due to seam");
                        foreach (Vector3 n in clusters[i])
                        {
                            //Debug.Log("[Frame " + Time.frameCount + "] Removing outlier normal: " + n);
                            normals.Remove(n);
                        }

                        clusters.RemoveAt(j);

                        continue;
                    }

                    if (clusters[j].Count >= 10)
                    {
                        //Debug.Log("[Frame " + Time.frameCount + "] Removing cluster[" + i + "] due to seam");
                        foreach (Vector3 n in clusters[i])
                        {
                            //Debug.Log("[Frame " + Time.frameCount + "] Removing outlier normal: " + n);
                            normals.Remove(n);
                        }
                        clusters.RemoveAt(i);

                        break;
                    }

                    //Debug.Log("[Frame " + Time.frameCount + "] Removing both cluster[" + i + "] and cluster [" + j + "]");
                    foreach (Vector3 n in clusters[j])
                    {
                        //Debug.Log("[Frame " + Time.frameCount + "] Removing outlier normal: " + n);
                        normals.Remove(n);
                    }

                    foreach (Vector3 n in clusters[i])
                    {
                        //Debug.Log("[Frame " + Time.frameCount + "] Removing outlier normal: " + n);
                        normals.Remove(n);
                    }

                    i--;
                    break;
                }
            }
        }
    }

    private bool SameSlope(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(Vector3.Angle(a, Vector3.up) - Vector3.Angle(b, Vector3.up)) < 2f;
    }

    private void NudgeUpwards(Vector3 currentVelocity)
    {
        currentVelocity.y = Mathf.Max(currentVelocity.y, nudgeAmount);
        playerRef.playerMotor.SetVelocity(currentVelocity);
        playerRef.checkGround.StartForcedGrounding();
    }

    private void AntiGravity(Vector3 currentVelocity)
    {
        currentVelocity.y = Mathf.Max(currentVelocity.y, -nudgeAmount);
        playerRef.playerMotor.SetVelocity(currentVelocity);

        if (nudgeAmount > 0)
        {
            nudgeAmount -= 0.01f;
        }

        playerRef.checkGround.EndForcedGrounding();
    }


    private void FallbackAntiGravity()
    {
        //Debug.Log("[Frame " + Time.frameCount + "] AntiGravity fallback");
        Vector3 newVelocity = playerRef.playerMotor.linearVelocity;
        newVelocity.y = Mathf.Max(newVelocity.y, -Physics.gravity.y * Time.fixedDeltaTime);
        playerRef.playerMotor.SetVelocity(newVelocity);
    }

    public void EnableFallbackAntiGravity()
    {
        //Debug.Log("[Frame " + Time.frameCount + "] AntiGravity fallback enabled");
        hoverState = HoverState.FallbackAntiGravity;
        playerRef.checkGround.StartForcedGrounding();

    }

    public void DisableFallbackAntiGravity()
    {
        //Debug.Log("[Frame " + Time.frameCount + "] AntiGravity fallback disabled");
        hoverState = HoverState.None;
        playerRef.checkGround.EndForcedGrounding();
    }
}

