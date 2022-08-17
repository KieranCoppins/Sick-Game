using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Environment Queries/Ignore Walls")]
public class R_IgnoreWalls : Rule
{
    public override Dictionary<Vector2Int, float> Run(Dictionary<Vector2Int, float> tiles, GameObject caller)
    {
        Dictionary<Vector2Int, float> newTiles = new Dictionary<Vector2Int, float>();

        tilemapController = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<TilemapController>();
        
        foreach (var tile in tiles)
        {
            if (((Tile)tilemapController.GetTileFromGlobalPosition(new Vector3(tile.Key.x, tile.Key.y, 0.0f))).colliderType == Tile.ColliderType.None)
                newTiles[tile.Key] = tiles[tile.Key];
        }

        return newTiles;
    }
}
