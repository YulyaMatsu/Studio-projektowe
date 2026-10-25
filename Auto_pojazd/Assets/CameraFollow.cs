using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Cel i przesunięcie")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 3f, -7f);

    [Header("Ustawienia płynności")]
    public float followSpeed = 10f;
    public float rotationSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        Vector3 lookTarget = target.position + Vector3.up * 1.5f;
        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}