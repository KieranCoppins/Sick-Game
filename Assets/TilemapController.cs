using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TilemapController : MonoBehaviour
{
    [SerializeField] bool eightNeighbours = false;

    Tilemap tilemap;
    Node[,] pathfindingGraph;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        GenerateGraph();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateGraph()
    {
        Debug.Log("Generating graph of " + tilemap.size.x + "x" + tilemap.size.y + " tilemap");

        // Initialise our 2D list of pathfinding nodes
        pathfindingGraph = new Node[tilemap.size.x, tilemap.size.y];

        // Iterate through each tile in the tilemap
        for (int x = tilemap.cellBounds.min.x; x <= tilemap.cellBounds.max.x; x++)
        {
            for (int y = tilemap.cellBounds.min.y; y <= tilemap.cellBounds.max.y; y++)
            {
                // Get the tile object at x, y
                Tile tile = tilemap.GetTile<Tile>(new Vector3Int(x, y, 0));

                // Default the movementCost to 1
                float movementCost = 1.0f;

                // If the tile has a collider we cant pass it therefore treat it as infinite cost to "enter" this tile
                if (tile.colliderType != Tile.ColliderType.None)
                    movementCost = float.PositiveInfinity;

                // Create a new node
                Node pathfindingNode = new Node(x, y, new List<Node>(), movementCost);

                // Store node at x, y
                pathfindingGraph[x, y] = pathfindingNode; 
            }
        }

        // Iterate thrugh each tile again and apply neighbours
        for (int x = tilemap.cellBounds.min.x; x <= tilemap.cellBounds.max.x; x++)
        {
            for (int y = tilemap.cellBounds.min.y; y <= tilemap.cellBounds.max.y; y++)
            {
                // Assign neighbours
                if (x > 0)  // We have a tile in our negative x
                    pathfindingGraph[x, y].neighbours.Add(pathfindingGraph[x - 1, y]);

                if (y > 0)  // We have a tile in our negative y
                    pathfindingGraph[x, y].neighbours.Add(pathfindingGraph[x, y - 1]);

                if (x <= tilemap.cellBounds.max.x) // We have a tile to our positive x
                    pathfindingGraph[x, y].neighbours.Add(pathfindingGraph[x + 1, y]);

                if (y <= tilemap.cellBounds.max.y) // We have a tile to our positive y
                    pathfindingGraph[x, y].neighbours.Add(pathfindingGraph[x, y + 1]);

                // Only add these neighbours if we want eight way movement
                if (eightNeighbours)
                {
                    if (x > 0 && y > 0) // We have a tile in our diagonal negative x and y
                        pathfindingGraph[x, y].neighbours.Add(pathfindingGraph[x - 1, y - 1]);

                    if (x <= tilemap.cellBounds.max.x && y <= tilemap.cellBounds.max.y) // We have a tile in our diagonal positive x and y
                        pathfindingGraph[x, y].neighbours.Add(pathfindingGraph[x + 1, y + 1]);

                    if (x > 0 && y <= tilemap.cellBounds.max.y) // We have a tile in our diagonal negative x and positive y
                        pathfindingGraph[x, y].neighbours.Add(pathfindingGraph[x - 1, y + 1]);

                    if (x <= tilemap.cellBounds.max.x && y > 0) // We have a tile in our diagonal positive x and negative y
                        pathfindingGraph[x, y].neighbours.Add(pathfindingGraph[x + 1, y - 1]);
                }
            }
        }
    }

    // Uses A* to calculate the path from start to end
    void CalculatePath(Vector2 start, Vector2 end)
    {

    }
}

// Stores information for each node
struct Node
{
    // The x coordinate of this node
    public int x;

    // The y coordinate of this node
    public int y;

    // A list of neighbours connected to this node
    public List<Node> neighbours;

    // The movement cost to enter this node
    public float movementCost;

    public Node(int x, int y, List<Node> neighbours, float movementCost)
    {
        this.x = x;
        this.y = y;
        this.neighbours = neighbours;
        this.movementCost = movementCost;
    }
}