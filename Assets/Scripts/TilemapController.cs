using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
[DisallowMultipleComponent]
public class TilemapController : MonoBehaviour
{
    [Tooltip("!EXPERIMENTAL! can cause bugs with pathfinding component")]
    [SerializeField] private bool _eightNeighbours = false;

    private Tilemap _tilemap;
    public Node[,] PathfindingGraph { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        _tilemap = GetComponent<Tilemap>();
        GenerateGraph();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateGraph()
    {
        _tilemap.CompressBounds();

        // Initialise our 2D list of pathfinding nodes
        PathfindingGraph = new Node[_tilemap.cellBounds.size.x, _tilemap.cellBounds.size.y];

        TileBase[] allTiles = _tilemap.GetTilesBlock(_tilemap.cellBounds);

        // Iterate through each tile in the tilemap
        for (int x = 0; x < _tilemap.cellBounds.size.x; x++)
        {
            for (int y = 0; y < _tilemap.cellBounds.size.y; y++)
            {
                // Get the tile object at x, y
                Tile tile = (Tile)allTiles[x + y * _tilemap.cellBounds.size.x];

                // Default the movementCost to 1
                float movementCost = 1.0f;

                // If the tile has a collider we cant pass it therefore treat it as infinite cost to "enter" this tile
                if (tile.colliderType != Tile.ColliderType.None)
                    movementCost = Mathf.Infinity;

                // Create a new node
                Node pathfindingNode = new Node(x + _tilemap.cellBounds.min.x, y + _tilemap.cellBounds.min.y, new List<Node>(), movementCost);

                // Store node at x, y
                PathfindingGraph[x, y] = pathfindingNode; 
            }
        }

        // Iterate thrugh each tile again and apply neighbours
        for (int x = 0; x < _tilemap.cellBounds.size.x; x++)
        {
            for (int y = 0; y < _tilemap.cellBounds.size.y; y++)
            {
                // Assign neighbours
                if (x > 0)  // We have a tile in our negative x
                    PathfindingGraph[x, y].Neighbours.Add(PathfindingGraph[x - 1, y]);

                if (y > 0)  // We have a tile in our negative y
                    PathfindingGraph[x, y].Neighbours.Add(PathfindingGraph[x, y - 1]);

                if (x < _tilemap.cellBounds.size.x - 1) // We have a tile to our positive x
                    PathfindingGraph[x, y].Neighbours.Add(PathfindingGraph[x + 1, y]);

                if (y < _tilemap.cellBounds.size.y - 1) // We have a tile to our positive y
                    PathfindingGraph[x, y].Neighbours.Add(PathfindingGraph[x, y + 1]);

                // Only add these neighbours if we want eight way movement
                if (_eightNeighbours)
                {
                    if (x > 0 && y > 0) // We have a tile in our diagonal negative x and y
                        PathfindingGraph[x, y].Neighbours.Add(PathfindingGraph[x - 1, y - 1]);

                    if (x < _tilemap.cellBounds.size.x - 1 && y < _tilemap.cellBounds.size.y - 1) // We have a tile in our diagonal positive x and y
                        PathfindingGraph[x, y].Neighbours.Add(PathfindingGraph[x + 1, y + 1]);

                    if (x > 0 && y < _tilemap.cellBounds.size.y - 1) // We have a tile in our diagonal negative x and positive y
                        PathfindingGraph[x, y].Neighbours.Add(PathfindingGraph[x - 1, y + 1]);

                    if (x < _tilemap.cellBounds.size.x - 1 && y > 0) // We have a tile in our diagonal positive x and negative y
                        PathfindingGraph[x, y].Neighbours.Add(PathfindingGraph[x + 1, y - 1]);
                }
            }
        }
    }

    // Takes a global coordinate and returns the tile that coordinate is in
    public TileBase GetTileFromGlobalPosition(Vector3 position)
    {
        Vector3Int tileCoords = _tilemap.WorldToCell(position);
        return _tilemap.GetTile(tileCoords);
    }

    public Node GetNodeFromGlobalPosition(Vector3 position)
    {
        Vector3Int tileCoords = _tilemap.WorldToCell(position);
        return PathfindingGraph[tileCoords.x - _tilemap.cellBounds.x, tileCoords.y - _tilemap.cellBounds.y];
    }

    public Vector2 GetGlobalPositionFromNode(Node node)
    {
        return _tilemap.CellToWorld(new Vector3Int(node.X, node.Y, 0));
    }

    public Vector2Int[] GetTilesInRange(Vector3Int position, int radius)
    {
        Vector3Int size = new Vector3Int(radius * 2, radius * 2, 1);
        BoundsInt bounds = new BoundsInt(position - size / 2, size);
        List<Vector2Int> points = new List<Vector2Int>();
        Vector2 tileOffset = new Vector2(0.5f, 0.5f);
        foreach (Vector2Int point in bounds.allPositionsWithin)
        {
            if (((point + tileOffset) - (Vector2Int)position).sqrMagnitude <= Mathf.Pow(radius, 2))
                points.Add(point);
        }
        return points.ToArray();
    }
}