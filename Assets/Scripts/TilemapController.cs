using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
[DisallowMultipleComponent]
public class TilemapController : MonoBehaviour
{
    [Tooltip("!EXPERIMENTAL! can cause bugs with pathfinding component")]
    [SerializeField] bool eightNeighbours = false;

    Tilemap tilemap;
    public Node[,] PathfindingGraph { get; private set; }

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


        tilemap.CompressBounds();

        Debug.Log("Generating graph of " + tilemap.cellBounds.size.x + "x" + tilemap.cellBounds.size.y + " tilemap");
        Debug.Log("Tile origin " + tilemap.origin.x + "x" + tilemap.origin.y);

        // Initialise our 2D list of pathfinding nodes
        PathfindingGraph = new Node[tilemap.cellBounds.size.x, tilemap.cellBounds.size.y];

        TileBase[] allTiles = tilemap.GetTilesBlock(tilemap.cellBounds);

        // Iterate through each tile in the tilemap
        for (int x = 0; x < tilemap.cellBounds.size.x; x++)
        {
            for (int y = 0; y < tilemap.cellBounds.size.y; y++)
            {
                // Get the tile object at x, y
                Tile tile = (Tile)allTiles[x + y * tilemap.cellBounds.size.x];

                // Default the movementCost to 1
                float movementCost = 1.0f;

                // If the tile has a collider we cant pass it therefore treat it as infinite cost to "enter" this tile
                if (tile.colliderType != Tile.ColliderType.None)
                    movementCost = Mathf.Infinity;

                // Create a new node
                Node pathfindingNode = new Node(x + tilemap.cellBounds.min.x, y + tilemap.cellBounds.min.y, new List<Node>(), movementCost);

                // Store node at x, y
                PathfindingGraph[x, y] = pathfindingNode; 
            }
        }

        // Iterate thrugh each tile again and apply neighbours
        for (int x = 0; x < tilemap.cellBounds.size.x; x++)
        {
            for (int y = 0; y < tilemap.cellBounds.size.y; y++)
            {
                // Assign neighbours
                if (x > 0)  // We have a tile in our negative x
                    PathfindingGraph[x, y].neighbours.Add(PathfindingGraph[x - 1, y]);

                if (y > 0)  // We have a tile in our negative y
                    PathfindingGraph[x, y].neighbours.Add(PathfindingGraph[x, y - 1]);

                if (x < tilemap.cellBounds.max.x - 1) // We have a tile to our positive x
                    PathfindingGraph[x, y].neighbours.Add(PathfindingGraph[x + 1, y]);

                if (y < tilemap.cellBounds.max.y - 1) // We have a tile to our positive y
                    PathfindingGraph[x, y].neighbours.Add(PathfindingGraph[x, y + 1]);

                // Only add these neighbours if we want eight way movement
                if (eightNeighbours)
                {
                    if (x > 0 && y > 0) // We have a tile in our diagonal negative x and y
                        PathfindingGraph[x, y].neighbours.Add(PathfindingGraph[x - 1, y - 1]);

                    if (x < tilemap.cellBounds.max.x - 1 && y < tilemap.cellBounds.max.y - 1) // We have a tile in our diagonal positive x and y
                        PathfindingGraph[x, y].neighbours.Add(PathfindingGraph[x + 1, y + 1]);

                    if (x > 0 && y < tilemap.cellBounds.max.y - 1) // We have a tile in our diagonal negative x and positive y
                        PathfindingGraph[x, y].neighbours.Add(PathfindingGraph[x - 1, y + 1]);

                    if (x < tilemap.cellBounds.max.x - 1 && y > 0) // We have a tile in our diagonal positive x and negative y
                        PathfindingGraph[x, y].neighbours.Add(PathfindingGraph[x + 1, y - 1]);
                }
            }
        }
    }

    // Takes a global coordinate and returns the tile that coordinate is in
    public TileBase GetTileFromGlobalPosition(Vector3 position)
    {
        Vector3Int tileCoords = tilemap.WorldToCell(position);
        Debug.Log(tileCoords);
        return tilemap.GetTile(tileCoords);
    }

    public Node GetNodeFromGlobalPosition(Vector3 position)
    {
        Vector3Int tileCoords = tilemap.WorldToCell(position);
        return PathfindingGraph[tileCoords.x - tilemap.cellBounds.x, tileCoords.y - tilemap.cellBounds.y];
    }

    public Vector2 GetGlobalPositionFromNode(Node node)
    {
        return tilemap.CellToWorld(new Vector3Int(node.x, node.y, 0));
    }
}