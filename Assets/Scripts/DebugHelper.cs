using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    [SerializeField] Vector2 startPoint;
    [SerializeField] Vector2 endPoint;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            DrawPathfindingDebug();
    }

    void DrawPathfindingDebug()
    {
        Vector2[] path = GetComponent<PathfindingComponent>().CalculateDijkstraPath(startPoint, endPoint);
        Debug.Log("Path length: " + path.Length);
        Vector2 prevP = path[0];
        foreach (Vector2 p in path)
        {
            if (prevP != null)
                Debug.DrawLine(p, prevP, Color.blue, 5000.0f, false);

            prevP = p;
        }

        Vector2[] path2 = GetComponent<PathfindingComponent>().CalculateAStarPath(startPoint, endPoint);
        Debug.Log("Path length: " + path.Length);
        Vector2 prevP2 = path[0];
        foreach (Vector2 p in path2)
        {
            if (prevP2 != null)
                Debug.DrawLine(p, prevP2, Color.red, 5000.0f, false);

            prevP2 = p;
        }
    }
}
