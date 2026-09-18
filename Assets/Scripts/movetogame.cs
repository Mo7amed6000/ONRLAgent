using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.InputSystem;

public class movetogame : Agent
{
    private float episodeTimer = 0f;
    public float maxEpisodeTime = 40f;

    [SerializeField] private Transform wall1;
    [SerializeField] private Transform wall2;
    [SerializeField] private Transform wall3;
    [SerializeField] private Transform wall4;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform ObsTransform;
    [SerializeField] private Transform ObsTransform2;
    [SerializeField] private Transform ObsTransform3;

    private float previousDistanceToTarget;

    public override void OnEpisodeBegin()
    {
        episodeTimer = 0f;

        transform.localPosition = new Vector3(Random.Range(6f, 14f), 0f, Random.Range(-11f, -6.5f));
        targetTransform.localPosition = new Vector3(Random.Range(6f, 14f), 0f, Random.Range(0f, 4f));
        ObsTransform.localPosition = new Vector3(Random.Range(6f, 14f), 0f, -1f);
        ObsTransform2.localPosition = new Vector3(Random.Range(6f, 14f), 0f, -3f);
        ObsTransform3.localPosition = new Vector3(Random.Range(6f, 14f), 0f, -5f);

        previousDistanceToTarget = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 dirToTarget = targetTransform.localPosition - transform.localPosition;

        sensor.AddObservation(dirToTarget.x);
        sensor.AddObservation(dirToTarget.z);
        sensor.AddObservation(dirToTarget.magnitude);

        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);

        sensor.AddObservation(ObsTransform.localPosition.x - transform.localPosition.x);
        sensor.AddObservation(ObsTransform.localPosition.z - transform.localPosition.z);

        sensor.AddObservation(ObsTransform2.localPosition.x - transform.localPosition.x);
        sensor.AddObservation(ObsTransform2.localPosition.z - transform.localPosition.z);

        sensor.AddObservation(ObsTransform3.localPosition.x - transform.localPosition.x);
        sensor.AddObservation(ObsTransform3.localPosition.z - transform.localPosition.z);

        sensor.AddObservation(wall1.localPosition.x - transform.localPosition.x);
        sensor.AddObservation(wall1.localPosition.z - transform.localPosition.z);

        sensor.AddObservation(wall2.localPosition.x - transform.localPosition.x);
        sensor.AddObservation(wall2.localPosition.z - transform.localPosition.z);

        sensor.AddObservation(wall3.localPosition.x - transform.localPosition.x);
        sensor.AddObservation(wall3.localPosition.z - transform.localPosition.z);

        sensor.AddObservation(wall4.localPosition.x - transform.localPosition.x);
        sensor.AddObservation(wall4.localPosition.z - transform.localPosition.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float x = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float z = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        float moveSpeed = 3f;

        float currentDistanceBeforeMove = Vector3.Distance(transform.localPosition, targetTransform.localPosition);

        transform.localPosition += new Vector3(x, 0f, z) * Time.deltaTime * moveSpeed;

        float currentDistanceAfterMove = Vector3.Distance(transform.localPosition, targetTransform.localPosition);

        AddReward(-0.001f);

        float progress = currentDistanceBeforeMove - currentDistanceAfterMove;
        AddReward(progress * 0.1f);

        episodeTimer += Time.deltaTime;

        if (episodeTimer >= maxEpisodeTime)
        {
            Debug.Log("Time out nigga");
            AddReward(-0.5f);
            EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);

        if (other.CompareTag("Goal"))
        {
            AddReward(1f);
            EndEpisode();
        }
        else if (other.CompareTag("Wall"))
        {
            AddReward(-1f);
            EndEpisode();
        }
        else if (other.CompareTag("Obs"))
        {
            AddReward(-1f);
            EndEpisode();
        }
    }

    // public override void Heuristic(in ActionBuffers actionsOut)
    // {
    //     ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
    //     continuousActions[0] = Input.GetAxisRaw("Horizontal");
    //     continuousActions[1] = Input.GetAxisRaw("Horizontal");
    // }
}