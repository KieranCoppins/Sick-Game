using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Environment Queries/Has Line Of Sight")]
public class R_HasLineOfSight : Rule
{
    public override Dictionary<Vector2Int, float> Run(Dictionary<Vector2Int, float> tiles, GameObject caller)
    {
        Dictionary<Vector2Int, float> newTiles = new Dictionary<Vector2Int, float>();
        Vector2Int target = GetTargetTile(caller);

        foreach (var tile in tiles)
        {
            RaycastHit2D hit;
            Vector3 direction = new Vector3(tile.Key.x + 0.5f, tile.Key.y + 0.5f, 0) - new Vector3(target.x + 0.5f, target.y + 0.5f);
            hit = Physics2D.Raycast(target, direction);
            if (hit)
                newTiles[tile.Key] = tiles[tile.Key] + scoreModifier;
            else if (!hit && !ignoreFailedTiles)
            {
                newTiles[tile.Key] = tiles[tile.Key];
            }
        }
        return newTiles;
    }
}
