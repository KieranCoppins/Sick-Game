using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores information for each node
public class Node
{
    // The x coordinate of this node
    public readonly int x;

    // The y coordinate of this node
    public readonly int y;

    // A list of neighbours connected to this node
    public List<Node> neighbours;

    // The movement cost to enter this node
    public readonly float movementCost;

    public Node(int x, int y, List<Node> neighbours, float movementCost)
    {
        this.x = x;
        this.y = y;
        this.neighbours = neighbours;
        this.movementCost = movementCost;
    }

    // Use the x and y values of the node for a vector 2
    public static implicit operator Vector2(Node n)
    {
        return new Vector2(n.x + 0.5f, n.y + 0.5f);
    }

    public static Vector2 operator -(Node a, Node b)
    {
        return new Vector2(a.x - b.x, a.y - b.y);
    }
}
