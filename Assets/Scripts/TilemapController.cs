using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
[DisallowMultipleComponent]
public class TilemapController : MonoBehaviour
{
    [Tooltip("!EXPERIMENTAL! can cause bugs with pathfinding component")]
    [SerializeField] private bool _eightNeighbours = false;

    [Tooltip("Determines if the tilemap is isometric or not")]
    [SerializeField] private bool _isometric = false;

    public bool Isometric { get => _isometric; }

    public Tilemap Tilemap { get; private set; }
    public Node[,] PathfindingGraph { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        Tilemap = GetComponent<Tilemap>();
        GenerateGraph();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateGraph()
    {
        Tilemap.CompressBounds();

        // Initialise our 2D list of pathfinding nodes
        PathfindingGraph = new Node[Tilemap.cellBounds.size.x, Tilemap.cellBounds.size.y];

        TileBase[] allTiles = Tilemap.GetTilesBlock(Tilemap.cellBounds);

        // Iterate through each tile in the tilemap
        for (int x = 0; x < Tilemap.cellBounds.size.x; x++)
        {
            for (int y = 0; y < Tilemap.cellBounds.size.y; y++)
            {
                // Get the tile object at x, y
                Tile tile = (Tile)allTiles[x + y * Tilemap.cellBounds.size.x];

                // Default the movementCost to 1
                float movementCost = 1.0f;

                // If the tile has a collider we cant pass it therefore treat it as infinite cost to "enter" this tile
                if (tile?.colliderType == Tile.ColliderType.Sprite || tile?.colliderType == Tile.ColliderType.Grid)
                    movementCost = Mathf.Infinity;

                // Create a new node
                Node pathfindingNode = new Node(x + Tilemap.cellBounds.x, y + Tilemap.cellBounds.y, new List<Node>(), movementCost, this);

                // Store node at x, y
                PathfindingGraph[x, y] = pathfindingNode;
            }
        }

        // Iterate thrugh each tile again and apply neighbours
        for (int x = 0; x < Tilemap.cellBounds.size.x; x++)
        {
            for (int y = 0; y < Tilemap.cellBounds.size.y; y++)
            {
                // Assign neighbours
                if (x > 0)  // We have a tile in our negative x
                    PathfindingGraph[x, y].Neighbours.Add(PathfindingGraph[x - 1, y]);

                if (y > 0)  // We have a tile in our negative y
                    PathfindingGraph[x, y].Neighbours.Add(PathfindingGraph[x, y - 1]);

                if (x < Tilemap.cellBounds.size.x - 1) // We have a tile to our positive x
                    PathfindingGraph[x, y].Neighbours.Add(PathfindingGraph[x + 1, y]);

                if (y < Tilemap.cellBounds.size.y - 1) // We have a tile to our positive y
                    PathfindingGraph[x, y].Neighbours.Add(PathfindingGraph[x, y + 1]);

                // Only add these neighbours if we want eight way movement
                if (_eightNeighbours)
                {
                    if (x > 0 && y > 0) // We have a tile in our diagonal negative x and y
                        PathfindingGraph[x, y].Neighbours.Add(PathfindingGraph[x - 1, y - 1]);

                    if (x < Tilemap.cellBounds.size.x - 1 && y < Tilemap.cellBounds.size.y - 1) // We have a tile in our diagonal positive x and y
                        PathfindingGraph[x, y].Neighbours.Add(PathfindingGraph[x + 1, y + 1]);

                    if (x > 0 && y < Tilemap.cellBounds.size.y - 1) // We have a tile in our diagonal negative x and positive y
                        PathfindingGraph[x, y].Neighbours.Add(PathfindingGraph[x - 1, y + 1]);

                    if (x < Tilemap.cellBounds.size.x - 1 && y > 0) // We have a tile in our diagonal positive x and negative y
                        PathfindingGraph[x, y].Neighbours.Add(PathfindingGraph[x + 1, y - 1]);
                }
            }
        }
    }

    // Takes a global coordinate and returns the tile that coordinate is in
    public TileBase GetTileFromGlobalPosition(Vector3 position)
    {
        Vector3Int tileCoords = Tilemap.WorldToCell(position);
        return Tilemap.GetTile(tileCoords);
    }

    public Node GetNodeFromGlobalPosition(Vector3 position)
    {
        Vector3Int tileCoords = Tilemap.WorldToCell(position);
        try
        {
            return PathfindingGraph[tileCoords.x - Tilemap.cellBounds.x, tileCoords.y - Tilemap.cellBounds.y];
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.DrawLine(position, Vector2.up * 0.5f, Color.red, 100f);
            return PathfindingGraph[0, 0];
        }
    }

    public Vector2 GetGlobalPositionFromNode(Node node)
    {
        return GetGlobalPositionFromTile(new Vector3Int(node.X, node.Y, 0));
    }

    public Vector2 GetGlobalPositionFromTile(Vector2Int position)
    {
        return GetGlobalPositionFromTile(new Vector3Int(position.x, position.y, 0));
    }

    public Vector2 GetGlobalPositionFromTile(Vector3Int position)
    {
        return Tilemap.GetCellCenterWorld(position);
    }

    public Vector2Int[] GetTilesInRange(Vector3 position, int radius)
    {
        Vector3Int size = new Vector3Int(radius * 2, radius * 2, 1);
        Vector3Int tileIndex = Tilemap.WorldToCell(position);
        BoundsInt bounds = new BoundsInt(tileIndex - size / 2, size);
        List<Vector2Int> points = new List<Vector2Int>();
        Vector2 tileOffset = new Vector2(Tilemap.cellBounds.y / 2, .0f);

        foreach (Vector2Int point in bounds.allPositionsWithin)
        {
            if (Tilemap.cellBounds.Contains(new Vector3Int(point.x, point.y, 0)) && (Tilemap.GetCellCenterWorld(tileIndex) - position).sqrMagnitude <= Mathf.Pow(radius, 2))
                points.Add(point);
        }
        return points.ToArray();
    }
}