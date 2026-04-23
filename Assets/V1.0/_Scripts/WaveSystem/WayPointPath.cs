using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    public Transform[] waypoints;

    void Awake()
    {
        CacheWaypoints();
    }

    void OnValidate()
    {
        CacheWaypoints();
    }

    void CacheWaypoints()
    {
        int count = transform.childCount;
        waypoints = new Transform[count];

        for (int i = 0; i < count; i++)
        {
            waypoints[i] = transform.GetChild(i);
        }
    }

    public Transform GetWaypoint(int index)
    {
        if (index < 0 || index >= waypoints.Length)
            return null;

        return waypoints[index];
    }

    public int GetWaypointCount()
    {
        return waypoints != null ? waypoints.Length : 0;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform point = transform.GetChild(i);
            if (point == null) continue;

            Gizmos.DrawSphere(point.position, 0.1f);

            if (i < transform.childCount - 1)
            {
                Transform next = transform.GetChild(i + 1);
                if (next != null)
                {
                    Gizmos.DrawLine(point.position, next.position);
                }
            }
        }
    }
}