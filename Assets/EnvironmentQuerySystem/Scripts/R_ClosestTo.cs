using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Environment Queries/Closest To")]
public class R_ClosestTo : Rule
{
    public override Dictionary<Vector2Int, float> Run(Dictionary<Vector2Int, float> tiles, GameObject caller)
    {
        Dictionary<Vector2Int, float> newTiles = new Dictionary<Vector2Int, float>();

        Vector2Int target = GetTargetTile(caller);
        foreach (var tile in tiles)
        {
            newTiles[tile.Key] = tiles[tile.Key] + Vector2.Distance(target, tile.Key) * scoreModifier;
        }
        return newTiles;

    }
}
