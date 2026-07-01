using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ScienceAgent : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private Animator animator;

    [Header("Station Targets")]
    [SerializeField] private Transform biologyTarget;
    [SerializeField] private Transform chemistryTarget;
    [SerializeField] private Transform physicsTarget;
    [SerializeField] private Transform analyticsTarget;

    [Header("NPC Settings")]
    [SerializeField] private float waitTimeAtStation = 4f;
    [SerializeField] private float observeTime = 3f;
    [SerializeField] private float introWaitTime = 2f;
    [SerializeField] private float walkSpeed = 1.2f;

    [Header("Audio")]
    [SerializeField] private AudioClip[] stationClips;
    [SerializeField] private AudioSource audioSource;

    private const float NavMeshSearchRadius = 5f;
    private Transform[] stations;
    private int currentStation = 0;
    private bool isCommandedToStation = false;

    private void Start()
    {
        stations = new Transform[]
        {
            biologyTarget, chemistryTarget,
            physicsTarget, analyticsTarget
        };

        if (navAgent != null)
        {
            navAgent.speed = walkSpeed;
            navAgent.angularSpeed = 120f;
            navAgent.acceleration = 8f;
            navAgent.autoBraking = true;
            navAgent.stoppingDistance = 0.5f;
        }

        SetAnimator("IsWalking", false);
        SetAnimator("IsTalking", false);
        StartCoroutine(InitialiseNavMesh());
    }

    private void Update()
    {
        if (navAgent != null && navAgent.isOnNavMesh)
        {
            float speed = navAgent.velocity.magnitude;
            bool isMoving = speed > 0.1f;
            SetAnimator("IsWalking", isMoving);
        }
    }

    private IEnumerator InitialiseNavMesh()
    {
        yield return null;
        if (navAgent != null && !navAgent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, NavMeshSearchRadius, NavMesh.AllAreas))
            {
                navAgent.Warp(hit.position);
                Debug.Log("[ScienceAgent] Warped to NavMesh at " + hit.position);
            }
            else
            {
                Debug.LogError("[ScienceAgent] No NavMesh found.");
                yield break;
            }
        }

        float timeout = 5f;
        while (navAgent != null && !navAgent.isOnNavMesh && timeout > 0)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        if (navAgent == null || !navAgent.isOnNavMesh)
        {
            Debug.LogError("[ScienceAgent] Agent still not on NavMesh.");
            yield break;
        }

        Debug.Log("[ScienceAgent] On NavMesh. Starting patrol.");
        StartCoroutine(NPCPatrol());
    }

    private IEnumerator NPCPatrol()
    {
        yield return new WaitForSeconds(introWaitTime);
        while (true)
        {
            if (isCommandedToStation)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            if (stations[currentStation] != null)
            {
                yield return StartCoroutine(WalkToTarget(stations[currentStation]));
                SetAnimator("IsWalking", false);
                SetAnimator("IsTalking", false);
                yield return new WaitForSeconds(observeTime);
            }

            currentStation = (currentStation + 1) % stations.Length;
        }
    }

    private IEnumerator WalkToTarget(Transform target)
    {
        if (target == null) yield break;
        if (navAgent == null || !navAgent.isOnNavMesh) yield break;

        SetAnimator("IsTalking", false);
        navAgent.isStopped = false;
        navAgent.speed = walkSpeed;
        navAgent.SetDestination(target.position);

        yield return new WaitForSeconds(0.3f);

        while (navAgent != null && navAgent.isOnNavMesh &&
               (navAgent.pathPending || navAgent.remainingDistance > navAgent.stoppingDistance + 0.1f))
        {
            yield return null;
        }

        navAgent.ResetPath();
        navAgent.isStopped = true;
        SetAnimator("IsWalking", false);
    }

    public void WalkToStationWithClip(string stationName, int clipIndex)
    {
        StopAllCoroutines();

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        SetAnimator("IsTalking", false);
        SetAnimator("IsWalking", false);
        isCommandedToStation = false;

        Transform target = GetTarget(stationName);
        if (target == null)
        {
            Debug.LogWarning("[ScienceAgent] No target found for: " + stationName);
            return;
        }

        Debug.Log("[ScienceAgent] Walking to " + stationName + " with clip index " + clipIndex);
        isCommandedToStation = true;

        // Always walk to target even if already nearby
        navAgent.isStopped = false;
        StartCoroutine(WalkToStationAndTalk(target, clipIndex));
    }

    public void WalkToStation(string stationName)
    {
        WalkToStationWithClip(stationName, GetStationIndex(stationName));
    }

    public void WalkToPlayer()
    {
        StopAllCoroutines();

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        SetAnimator("IsTalking", false);
        SetAnimator("IsWalking", false);
        isCommandedToStation = false;
        isCommandedToStation = true;

        Debug.Log("[ScienceAgent] Walking to player.");
        StartCoroutine(WalkToPlayerAndTalk());
    }

    private IEnumerator WalkToPlayerAndTalk()
    {
        Transform player = Camera.main.transform;
        if (player == null) yield break;

        Vector3 targetPos = player.position + player.forward * 1.5f;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, 3f, NavMesh.AllAreas))
        {
            navAgent.isStopped = false;
            navAgent.SetDestination(hit.position);
            yield return new WaitForSeconds(0.3f);

            while (navAgent != null && navAgent.isOnNavMesh &&
                   (navAgent.pathPending || navAgent.remainingDistance > navAgent.stoppingDistance + 0.1f))
            {
                yield return null;
            }
        }

        navAgent.isStopped = true;
        SetAnimator("IsWalking", false);

        Vector3 lookDir = (player.position - transform.position);
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookDir);

        SetAnimator("IsTalking", true);

        if (audioSource != null && stationClips != null && stationClips.Length > 0 && stationClips[0] != null)
        {
            Debug.Log("[ScienceAgent] Playing Agent Intro clip.");
            audioSource.clip = stationClips[0];
            audioSource.Play();
            yield return new WaitForSeconds(stationClips[0].length);
        }
        else
        {
            Debug.LogWarning("[ScienceAgent] Agent Intro clip missing.");
            yield return new WaitForSeconds(waitTimeAtStation);
        }

        SetAnimator("IsTalking", false);
        isCommandedToStation = false;
        StartCoroutine(NPCPatrol());
    }

    private IEnumerator WalkToStationAndTalk(Transform target, int clipIndex)
    {
        yield return StartCoroutine(WalkToTarget(target));

        SetAnimator("IsWalking", false);
        SetAnimator("IsTalking", true);

        if (audioSource != null && stationClips != null && clipIndex < stationClips.Length && stationClips[clipIndex] != null)
        {
            Debug.Log("[ScienceAgent] Playing clip index: " + clipIndex + " clip: " + stationClips[clipIndex].name);
            audioSource.clip = stationClips[clipIndex];
            audioSource.Play();
            yield return new WaitForSeconds(stationClips[clipIndex].length);
        }
        else
        {
            Debug.LogWarning("[ScienceAgent] Clip missing at index: " + clipIndex);
            yield return new WaitForSeconds(waitTimeAtStation);
        }

        SetAnimator("IsTalking", false);
        isCommandedToStation = false;
        StartCoroutine(NPCPatrol());
    }

    private int GetStationIndex(string name)
    {
        if (name == "Biology") return 1;
        if (name == "Chemistry") return 3;
        if (name == "Physics") return 4;
        if (name == "Analytics") return 5;
        return 0;
    }

    private Transform GetTarget(string name)
    {
        if (name == "Biology") return biologyTarget;
        if (name == "Chemistry") return chemistryTarget;
        if (name == "Physics") return physicsTarget;
        if (name == "Analytics") return analyticsTarget;
        return null;
    }

    private void SetAnimator(string param, bool value)
    {
        if (animator != null) animator.SetBool(param, value);
    }
}