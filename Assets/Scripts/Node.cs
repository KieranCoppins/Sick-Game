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

    // If the node is part of an isometric map
    private readonly TilemapController _parentController;

    public Node(int x, int y, List<Node> neighbours, float movementCost, TilemapController parentController)
    {
        X = x;
        Y = y;
        Neighbours = neighbours;
        MovementCost = movementCost;
        _parentController = parentController;
    }

    public static Vector2 operator -(Node a, Node b)
    {
        return new Vector2(a.X - b.X, a.Y - b.Y);
    }
}
