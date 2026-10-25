using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CarAgent : Agent
{
    private Rigidbody carRb;
    
    [Header("Movement Settings")]
    public float moveSpeed = 15f;
    public float steerSpeed = 150f;
    
    private Vector3 startPosition;
    private Quaternion startRotation;

    private int nextCheckpointIndex = 0;
    
    public Transform checkpointsParent;
    private Transform[] checkpoints;

    public override void Initialize()
    {
        carRb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        startRotation = transform.rotation;

        if (checkpointsParent != null)
        {
            checkpoints = new Transform[checkpointsParent.childCount];
            for (int i = 0; i < checkpointsParent.childCount; i++)
            {
                checkpoints[i] = checkpointsParent.GetChild(i);
            }
        }
    }

    public override void OnEpisodeBegin()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        carRb.linearVelocity = Vector3.zero;
        carRb.angularVelocity = Vector3.zero;

        nextCheckpointIndex = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(carRb.linearVelocity.magnitude);

        if (checkpoints != null && checkpoints.Length > 0 && nextCheckpointIndex < checkpoints.Length)
        {
            Vector3 toCheckpoint = checkpoints[nextCheckpointIndex].position - transform.position;
            sensor.AddObservation(toCheckpoint.normalized);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = actions.ContinuousActions[0];
        float turnAmount = actions.ContinuousActions[1];

        Vector3 moveDirection = transform.forward * forwardAmount * moveSpeed;
        carRb.AddForce(moveDirection, ForceMode.Acceleration);

        transform.Rotate(0f, turnAmount * steerSpeed * Time.deltaTime, 0f);

        AddReward(-1f / MaxStep);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            AddReward(-1f);
            EndEpisode();
        }
        else if (checkpoints != null && nextCheckpointIndex < checkpoints.Length && other.transform == checkpoints[nextCheckpointIndex])
        {
            AddReward(1f);
            nextCheckpointIndex++;

            if (nextCheckpointIndex >= checkpoints.Length)
            {
                AddReward(5f);
                EndEpisode();
            }
        }
    }
}