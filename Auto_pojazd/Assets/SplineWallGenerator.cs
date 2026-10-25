using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class SplineWallGenerator : MonoBehaviour
{
    [Header("Ustawienia Toru")]
    public float trackWidth = 8f; 
    public float wallHeight = 2f;
    public int segmentsCount = 50;

    // void Start()
    // {
    //     GenerateWalls();
    // }

    [ContextMenu("Generuj Sciany Teraz")]
    public void GenerateWalls()
    {
        SplineContainer container = GetComponent<SplineContainer>();
        if (container == null || container.Spline == null) return;

        Transform wallsParent = new GameObject("GeneratedWalls").transform;
        wallsParent.SetParent(transform);

        float step = 1f / segmentsCount;

        for (int i = 0; i < segmentsCount; i++)
        {
            float t1 = i * step;
            float t2 = (i + 1) * step;

            Vector3 pos1 = container.EvaluatePosition(t1);
            Vector3 pos2 = container.EvaluatePosition(t2);

            Vector3 forward1 = Vector3.Normalize(container.EvaluateTangent(t1));
            Vector3 forward2 = Vector3.Normalize(container.EvaluateTangent(t2));

            Vector3 right1 = Vector3.Cross(Vector3.up, forward1).normalized;
            Vector3 right2 = Vector3.Cross(Vector3.up, forward2).normalized;

            BuildSegment(pos1 + right1 * (trackWidth / 2f), pos2 + right2 * (trackWidth / 2f), wallsParent, "Wall_Right");
            BuildSegment(pos1 - right1 * (trackWidth / 2f), pos2 - right2 * (trackWidth / 2f), wallsParent, "Wall_Left");
        }
    }

    void BuildSegment(Vector3 start, Vector3 end, Transform parent, string name)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.SetParent(parent);
        wall.tag = "Wall";

        Vector3 center = (start + end) / 2f;
        wall.transform.position = center;
        wall.transform.LookAt(end);

        wall.transform.localScale = new Vector3(0.5f, wallHeight, Vector3.Distance(start, end));

        BoxCollider col = wall.GetComponent<BoxCollider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }
}
