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

        Vector2 target = GetTargetPos(caller);
        foreach (var tile in tiles)
        {
            // Currently does NAIVE distance - maybe we want to use our pathfinding component to get the shortest cost to path
            newTiles[tile.Key] = tiles[tile.Key] + Vector2.Distance(target, tile.Key) * scoreModifier;
        }
        return newTiles;

    }
}
