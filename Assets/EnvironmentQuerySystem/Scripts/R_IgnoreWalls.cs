using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

[CreateAssetMenu(menuName = "Environment Queries/Ignore Walls")]
public class R_IgnoreWalls : Rule
{
    public override Dictionary<Vector2Int, float> Run(Dictionary<Vector2Int, float> tiles, GameObject caller)
    {
        Dictionary<Vector2Int, float> newTiles = new Dictionary<Vector2Int, float>();

        TilemapController tilemapController = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<TilemapController>();
        
        foreach (var tile in tiles)
        {
            Tile tileObj = (Tile)tilemapController.GetTileFromGlobalPosition(new Vector3(tile.Key.x, tile.Key.y, 0.0f));
            if (tileObj != null && tileObj.colliderType == Tile.ColliderType.None)
                newTiles[tile.Key] = tiles[tile.Key];
        }

        return newTiles;
    }

    public override string GetSummary()
    {
        return "The tile is not a wall";
    }
}

// A custom inspector to hide attributes for this rule
[CustomEditor(typeof(R_IgnoreWalls))]
public class R_IgnoreWallsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        return;
    }
}
