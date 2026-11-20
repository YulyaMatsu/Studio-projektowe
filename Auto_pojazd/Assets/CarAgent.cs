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
    
    [Header("Checkpoints")]
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

    /// <summary>
    /// Łącznie 7 obserwacji wektorowych:
    /// 1-3. Prędkość lokalna pojazdu (X, Y, Z) - informuje o prędkości jazdy i poślizgu bocznym (3)
    /// 4-6. Znormalizowany wektor do kolejnego checkpointu w układzie lokalnym auta (3)
    /// 7.   Iloczyn skalarny (Dot product) kierunku przodu auta i kierunku do checkpointu (1)
    /// PAMIĘTAJ: Ustaw "Space Size = 7" w komponencie BehaviorParameters na obiekcie pojazdu!
    /// </summary>
    public override void CollectObservations(VectorSensor sensor)
    {
        // 1-3: Prędkość w układzie współrzędnych pojazdu (3 floaty)
        Vector3 localVelocity = transform.InverseTransformDirection(carRb.linearVelocity);
        sensor.AddObservation(localVelocity);

        // 4-7: Obserwacje celu / checkpointu (4 floaty)
        if (checkpoints != null && checkpoints.Length > 0 && nextCheckpointIndex < checkpoints.Length)
        {
            Vector3 worldToCheckpoint = (checkpoints[nextCheckpointIndex].position - transform.position).normalized;
            Vector3 localToCheckpoint = transform.InverseTransformDirection(worldToCheckpoint);
            
            // 4-6: Kierunek do checkpointu z perspektywy samochodu (3 floaty)
            sensor.AddObservation(localToCheckpoint);

            // 7: Jak dobrze przód pojazdu jest skierowany na checkpoint (-1 = tyłem, 1 = idealnie przodem) (1 float)
            float alignmentWithCheckpoint = Vector3.Dot(transform.forward, worldToCheckpoint);
            sensor.AddObservation(alignmentWithCheckpoint);
        }
        else
        {
            sensor.AddObservation(Vector3.zero); // 3 floaty
            sensor.AddObservation(0f);           // 1 float
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float turnAmount = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        // Płynna zmiana prędkości z użyciem fizyki
        Vector3 targetVelocity = transform.forward * forwardAmount * moveSpeed;
        Vector3 currentHorizontalVel = new Vector3(carRb.linearVelocity.x, 0f, carRb.linearVelocity.z);
        Vector3 newHorizontalVel = Vector3.Lerp(currentHorizontalVel, targetVelocity, Time.fixedDeltaTime * 5f);
        carRb.linearVelocity = new Vector3(newHorizontalVel.x, carRb.linearVelocity.y, newHorizontalVel.z);

        // Skręcanie pojazdu w trakcie ruchu
        if (currentHorizontalVel.magnitude > 0.5f)
        {
            float steerDirection = (forwardAmount < 0f) ? -turnAmount : turnAmount;
            transform.Rotate(0f, steerDirection * steerSpeed * Time.fixedDeltaTime, 0f);
        }

        // Nagradzanie za prędkość w kierunku kolejnego checkpointu (Reward Shaping)
        if (checkpoints != null && nextCheckpointIndex < checkpoints.Length)
        {
            Vector3 worldToCheckpoint = (checkpoints[nextCheckpointIndex].position - transform.position).normalized;
            float velocityTowardsCheckpoint = Vector3.Dot(carRb.linearVelocity, worldToCheckpoint);
            // Dodajemy małą nagrodę za posuwanie się w stronę celu (lub małą karę za jazdę tyłem/oddalanie się)
            AddReward(velocityTowardsCheckpoint * 0.001f);
        }

        // Drobna kara czasowa zachęcająca do szybkiej jazdy
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
        // Kolizja ze ścianą - kara i zakończenie epizodu
        if (other.CompareTag("Wall"))
        {
            AddReward(-1.5f);
            EndEpisode();
        }
        // Zdobycie kolejnego checkpointu
        else if (checkpoints != null && nextCheckpointIndex < checkpoints.Length && other.transform == checkpoints[nextCheckpointIndex])
        {
            AddReward(1.0f);
            nextCheckpointIndex++;

            // Ukończenie całego okrążenia
            if (nextCheckpointIndex >= checkpoints.Length)
            {
                AddReward(5.0f);
                EndEpisode();
            }
        }
    }
}
