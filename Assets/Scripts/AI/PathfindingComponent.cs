using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
public class PathfindingComponent : MonoBehaviour
{
    private TilemapController _tilemapController;

    private void Awake()
    {
        _tilemapController = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<TilemapController>();
        if (_tilemapController == null)
            throw new MissingComponentException("Can't find Tilemap Controller on Tilemap tagged GameObject");
    }

    private float CalculateHeristicEstimate(Node n, Node target)
    {
        return Vector2.Distance(_tilemapController.GetGlobalPositionFromNode(n), _tilemapController.GetGlobalPositionFromNode(target));
    }

    private Vector2[] FormatPath(List<Node> path, Vector2 endPosition)
    {
        List<Vector2> waypoints = new List<Vector2>();
        foreach (Node n in path)
        {
            waypoints.Add(_tilemapController.GetGlobalPositionFromNode(n));
        }
        waypoints.Add(endPosition);
        return waypoints.ToArray();
    }

    private Vector2[] ReconstructPath(Dictionary<Node, Node> cameFrom, Node current, Vector3 start, Vector3 end)
    {
        List<Node> path = new List<Node>();
        path.Add(current);

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }

        return SmoothPath(path, start, end);
    }

    // Calculates a path using A Star Algorithm from start to end and returns a list of points
    public Vector2[] CalculateAStarPath(Vector3 start, Vector3 end)
    {
        if (_tilemapController == null)
        {
            Debug.LogError("Can't make a path with no tilemap controller");
            return null;
        }
        // Get our start and end nodes
        Node startNode = _tilemapController.GetNodeFromGlobalPosition(start);
        Node endNode = _tilemapController.GetNodeFromGlobalPosition(end);

        List<Node> open = new List<Node>();
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, float> gScore = new Dictionary<Node, float>();
        Dictionary<Node, float> fScore = new Dictionary<Node, float>();

        foreach (Node n in _tilemapController.PathfindingGraph)
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
                return ReconstructPath(cameFrom, curr, start, end);

            open.Remove(curr);

            foreach (Node n in curr.Neighbours)
            {
                float tentativeGScore = gScore[curr] + n.MovementCost;

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
        return ReconstructPath(cameFrom, curr, start, end);
    }

    //Smooth our path
    private Vector2[] SmoothPath(List<Node> path, Vector2 start, Vector2 end)
    {
        // Lets smooth this path since our movement isn't locked to each tile

        // To store our Vector3 positions
        List<Vector2> waypoints = new() { _tilemapController.GetGlobalPositionFromNode(path[0]) };

        // Our current node is the first in the list
        Node currentNode = path[0];

        // Our previous node is null
        Node prevN = null;

        float castRadius = 0.2f;

        // Iterate through each node in the current path
        foreach (Node n in path)
        {
            // Calculate a direction vector
            Vector2 direction = _tilemapController.GetGlobalPositionFromNode(currentNode) - _tilemapController.GetGlobalPositionFromNode(n);
            float distance = Vector2.Distance(_tilemapController.GetGlobalPositionFromNode(currentNode), _tilemapController.GetGlobalPositionFromNode(n));

            // TODO: determine if nodes (tiles) have line of sight without using raycasts, there must be something mathematical we can use to reduce our raycasts
            RaycastHit2D hit = Physics2D.CircleCast(_tilemapController.GetGlobalPositionFromNode(n), castRadius, direction.normalized, distance - castRadius);

            // If the raycast hit the tilemap (we cannot move in a straight line to the waypoint) and our previous N isnt null
            // We check hit.collider to see if we have hit before trying to call compare tag - if we didnt hit we cant still call it and we'll get a null reference exception - this prevents that
            if (hit.collider && hit.collider.CompareTag("Tilemap") && prevN != null)
            {
                // Add the previous point (as it didnt get a hit with the raycast
                waypoints.Add(_tilemapController.GetGlobalPositionFromNode(prevN));

                // Update our currentNodes
                currentNode = prevN;
            }
            // If we didnt get a hit but we're at our last node anyway we should be able to move to it
            else if (n == path[path.Count - 1])
            {
                waypoints.Add(_tilemapController.GetGlobalPositionFromNode(n));
            }

            prevN = n;

        }

        // Add the last position to the end of the waypoints so we reach the desired location
        waypoints.Add(end);

        // Convert our list to an array and return it
        return waypoints.ToArray();
    }
}
