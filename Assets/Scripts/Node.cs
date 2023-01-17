using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores information for each node
public class Node
{
    // The x coordinate of this node
    public int X { get; }

    // The y coordinate of this node
    public int Y { get; }

    // A list of neighbours connected to this node
    public List<Node> Neighbours { get; }

    // The movement cost to enter this node
    public float MovementCost { get; }

    public Vector2 Position { get; }

    public Node(int x, int y, List<Node> neighbours, float movementCost)
    {
        X = x;
        Y = y;
        Neighbours = neighbours;
        MovementCost = movementCost;
    }

    // Use the x and y values of the node for a vector 2
    public static implicit operator Vector2(Node n)
    {
        return new Vector2(n.X + 0.5f, n.Y + 0.5f);
    }

    public static Vector2 operator -(Node a, Node b)
    {
        return new Vector2(a.X - b.X, a.Y - b.Y);
    }

    public Vector2 LowerLeft()
    {
        return new Vector2(X + 0.01f, Y + 0.01f);
    }

    public Vector2 LowerRight()
    {
        return new Vector2(X + 0.99f, Y + 0.01f);
    }
    public Vector2 UpperLeft()
    {
        return new Vector2(X + 0.01f, Y + 0.99f);
    }

    public Vector2 UpperRight()
    {
        return new Vector2(X + 0.99f, Y + 0.99f);
    }
}
