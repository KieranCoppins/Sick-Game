using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
public class PathfindingComponent : MonoBehaviour
{
    TilemapController tilemapController;

    private void Awake()
    {
        tilemapController = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<TilemapController>();
        if (tilemapController == null)
            throw new MissingComponentException("Can't find Tilemap Controller on Tilemap tagged GameObject");
    }

    float CalculateHeristicEstimate(Node n, Node target)
    {
        return Vector2.Distance(n, target);
    }

    Vector2[] FormatPath(List<Node> path, Vector2 endPosition)
    {
        List<Vector2> waypoints = new List<Vector2>();
        foreach (Node n in path)
        {
            waypoints.Add(n);
        }
        waypoints.Add(endPosition);
        return waypoints.ToArray();
    }

    Vector2[] ReconstructPath(Dictionary<Node, Node> cameFrom, Node current, Vector3 endPosition)
    {
        List<Node> path = new List<Node>();
        path.Add(current);

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }

        return SmoothPath(path, endPosition);
    }

    // Calculates a path using A Star Algorithm from start to end and returns a list of points
    public Vector2[] CalculateAStarPath(Vector3 start, Vector3 end)
    {
        if (tilemapController == null)
        {
            Debug.LogError("Can't make a path with no tilemap controller");
            return null;
        }
        // Get our start and end nodes
        Node startNode = tilemapController.GetNodeFromGlobalPosition(start);
        Node endNode = tilemapController.GetNodeFromGlobalPosition(end);

        List<Node> open = new List<Node>();
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, float> gScore = new Dictionary<Node, float>();
        Dictionary<Node, float> fScore = new Dictionary<Node, float>();

        foreach (Node n in tilemapController.PathfindingGraph)
        {
            gScore[n] = Mathf.Infinity;
            fScore[n] = Mathf.Infinity;
        }

        gScore[startNode] = 0;
        fScore[startNode] = CalculateHeristicEstimate(startNode, endNode);

        open.Add(startNode);

        Node curr = null;

        while (open.Count > 0)
        {
            curr = null;
            foreach (Node n in open)
            {
                if (curr == null || fScore[curr] > fScore[n])
                    curr = n;
            }

            if (curr == endNode)
                return ReconstructPath(cameFrom, curr, end);

            open.Remove(curr);

            foreach (Node n in curr.neighbours)
            {                   
                float tentativeGScore = gScore[curr] + n.movementCost;

                if (tentativeGScore < gScore[n])
                {
                    cameFrom[n] = curr;
                    gScore[n] = tentativeGScore;
                    fScore[n] = tentativeGScore + CalculateHeristicEstimate(n, endNode);
                    if (!open.Contains(n))
                    {
                        open.Add(n);
                    }
                }
            }
        }
        return ReconstructPath(cameFrom, curr, end);
    }

    //Smooth our path
    Vector2[] SmoothPath(List<Node> path, Vector2 endPosition)
    {
        // Lets smooth this path since our movement isn't locked to each tile

        // To store our Vector3 positions
        List<Vector2> waypoints = new List<Vector2>();

        // Our current node is the first in the list
        Node currentNode = path[0];

        // Our previous node is null
        Node prevN = null;

        // Add our current node to our waypoints list
        waypoints.Add(currentNode);

        // Iterate through each node in the current path
        foreach (Node n in path)
        {
            // Calculate a direction vector
            Vector2 direction = currentNode - n;

            Vector2 castLower;
            Vector2 castUpper;

            // Do the cross product of the current direction and a diagonal vector
            float dotProd = Mathf.Abs(Vector2.Dot(direction, new Vector2(0.5f, 0.5f).normalized));

            // We're in a forward slash
            if (dotProd >= 0.75f)
            {
                castLower = n.lowerRight();
                castUpper = n.upperLeft();
            }
            // Otherwise its a backward slash
            else
            {
                castLower = n.lowerLeft();
                castUpper = n.upperRight();
            }

            // Calculate the distance for our raycast to be
            float distance = Vector2.Distance(currentNode, n);

            // Cast a ray from currentNode to the node in iteration
            RaycastHit2D hitLower = Physics2D.Raycast(castLower, direction.normalized, distance);
            RaycastHit2D hitUpper = Physics2D.Raycast(castUpper, direction.normalized, distance);

            // If the raycast hit something (we cannot move in a straight line to the waypoint) and our previous N isnt null
            if ((hitLower || hitUpper) && prevN != null)
            {
                // Add the previous point (as it didnt get a hit with the raycast
                waypoints.Add(prevN);

                // Update our currentNodes
                currentNode = prevN;
            }
            // If we didnt get a hit but we're at our last node anyway we should be able to move to it
            else if (n == path[path.Count - 1])
            {
                waypoints.Add(n);
            }

            prevN = n;

        }

        // Add the last position to the end of the waypoints so we reach the desired location
        waypoints.Add(endPosition);

        // Convert our list to an array and return it
        return waypoints.ToArray();
    }
}
