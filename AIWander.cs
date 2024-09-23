using UnityEngine;
using UnityEngine.AI;

public class AIWander : MonoBehaviour
{
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetRandomDestination();
    }

    void SetRandomDestination()
    {
        // Get a random point on the NavMesh
        Vector3 randomPoint = RandomNavmeshPoint(10f); // Adjust the radius as needed

        // Set the destination for the AI
        agent.SetDestination(randomPoint);
    }

    Vector3 RandomNavmeshPoint(float radius)
    {
        // Get a random point inside a sphere with the given radius
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;

        // Find the nearest point on the NavMesh to the random direction
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas);

        return hit.position;
    }

    void Update()
    {
        // Check if the AI has reached its destination
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetRandomDestination();
        }
    }
}
