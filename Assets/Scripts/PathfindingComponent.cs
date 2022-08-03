using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingComponent : MonoBehaviour
{
    [SerializeField] TilemapController tilemapController;

    float CalculateHeristicEstimate(Node n, Node target)
    {
        return Vector2.Distance(n, target);
    }

    Vector2[] FormatPath(List<Node> path)
    {
        List<Vector2> waypoints = new List<Vector2>();
        foreach (Node n in path)
        {
            waypoints.Add(n);
        }
        return waypoints.ToArray();
    }

    Vector2[] ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
    {
        List<Node> path = new List<Node>();
        path.Add(current);

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }

        return SmoothPath(path);
    }

    // Calculates a path using Dijkstra's Algorithm from start to end and returns a list of points
    public Vector2[] CalculateAStarPath(Vector3 start, Vector3 end)
    {
        // Get our start and end nodes
        Node startNode = tilemapController.GetNodeFromGlobalPosition(start);
        Node endNode = tilemapController.GetNodeFromGlobalPosition(end);

        List<Node> open = new List<Node>();
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, float> gScore = new Dictionary<Node, float>();
        Dictionary<Node, float> fScore = new Dictionary<Node, float>();

        foreach (Node n in tilemapController.PathfindingGraph)
        {
            open.Add(n);
            gScore[n] = Mathf.Infinity;
            fScore[n] = Mathf.Infinity;
        }

        gScore[startNode] = 0;
        fScore[startNode] = CalculateHeristicEstimate(startNode, endNode);

        while (open.Count > 0)
        {
            Node curr = null;
            foreach (Node n in open)
            {
                if (curr == null || fScore[curr] > fScore[n])
                    curr = n;
            }

            if (curr == endNode)
                return ReconstructPath(cameFrom, curr);

            open.Remove(curr);

            foreach (Node n in curr.neighbours)
            {
                float tentativeGScore = Mathf.Infinity;

                if (n.movementCost != Mathf.Infinity)
                    tentativeGScore = gScore[curr] + n.movementCost;

                if (tentativeGScore < gScore[n])
                {
                    cameFrom[n] = curr;
                    gScore[n] = tentativeGScore;
                    fScore[n] = gScore[n] + CalculateHeristicEstimate(n, endNode);
                    if (!open.Contains(n))
                    {
                        open.Add(n);
                    }
                }
            }
        }
        return null;
    }

    // Calculates a path using Dijkstra's Algorithm from start to end and returns a list of points
    // NOTE: This was added because I was following the wrong pseudocode....
    public Vector2[] CalculateDijkstraPath(Vector3 start, Vector3 end)
    {
        // Get our start and end nodes
        Node startNode = tilemapController.GetNodeFromGlobalPosition(start);
        Node endNode = tilemapController.GetNodeFromGlobalPosition(end);

        // Check if we can actually enter our desired tile
        if (endNode.movementCost == Mathf.Infinity)
            return null;

        // Dictionary containing the total cost (value) of the path up to node (key)
        Dictionary<Node, float> dist = new Dictionary<Node, float>();

        // Dictionary containing the previous node (value) to node (key)
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

        List<Node> unvisited = new List<Node> ();

        dist[startNode] = 0;
        prev[startNode] = null;

        foreach (Node n in tilemapController.PathfindingGraph)
        {
            unvisited.Add(n);
            if (n == startNode)
                continue;
            dist[n] = Mathf.Infinity;
            prev[n] = null;
        }

        while (unvisited.Count > 0)
        {
            Node u = null;

            foreach(Node possibleU in unvisited)
            {
                if (u == null || dist[possibleU] < dist[u])
                {
                    u = possibleU;
                }
            }

            // If we have reached our end node - break
            if (u == endNode)
                break;

            unvisited.Remove(u);

            // Iterate through each nodes neighbours
            foreach (Node n in u.neighbours)
            {
                float cost = dist[u] + n.movementCost;
                if (cost < dist[n])
                {
                    dist[n] = cost;
                    prev[n] = u;
                }
            }
        }

        // Check that we have a path to our goal
        if (prev[endNode] == null)
            return null;

        // A list to store our path
        List<Node> currentPath = new List<Node>();

        // Make our current node our target
        Node curr = endNode;

        // Until he hit null (our beginging node)
        while (curr != null)
        {
            currentPath.Add(curr);
            curr = prev[curr];
        }
        currentPath.Reverse();

        return SmoothPath(currentPath);
    }

    //Smooth our path
    Vector2[] SmoothPath(List<Node> path)
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

        // Convert our list to an array and return it
        return waypoints.ToArray();
    }
}
