using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.InputSystem;

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
        if (checkpoints != null && checkpoints.Length > 0)
        {
            startPosition = checkpoints[0].position;
            startRotation = checkpoints[0].rotation;
        }
        else
        {
            startPosition = transform.position;
            startRotation = transform.rotation;
        }
    }

    public override void OnEpisodeBegin()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        carRb.linearVelocity = Vector3.zero;
        carRb.angularVelocity = Vector3.zero;

        nextCheckpointIndex = (checkpoints != null && checkpoints.Length > 1) ? 1 : 0;
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

        Vector3 targetVelocity = transform.forward * forwardAmount * moveSpeed;
        Vector3 currentHorizontalVel = new Vector3(carRb.linearVelocity.x, 0, carRb.linearVelocity.z);
        Vector3 newHorizontalVel = Vector3.Lerp(currentHorizontalVel, targetVelocity, Time.deltaTime * 5f);
        carRb.linearVelocity = new Vector3(newHorizontalVel.x, carRb.linearVelocity.y, newHorizontalVel.z);

        if (currentHorizontalVel.magnitude > 0.5f)
        {
            float steerDirection = (forwardAmount < 0f) ? -turnAmount : turnAmount;
            transform.Rotate(0f, steerDirection * steerSpeed * Time.deltaTime, 0f);
        }

        if (MaxStep > 0)
        {
            AddReward(-1f / MaxStep);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;

        if (Keyboard.current != null)
        {
            float forward = 0f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) forward = 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) forward = -1f;
            continuousActions[0] = forward;

            float turn = 0f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) turn = 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) turn = -1f;
            continuousActions[1] = turn;
        }
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