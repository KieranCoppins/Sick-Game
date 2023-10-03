using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Environment Queries/Has Line Of Sight")]
public class R_HasLineOfSight : Rule
{
    public override Dictionary<Vector2Int, float> Run(TilemapController tilemap, Dictionary<Vector2Int, float> tiles, GameObject caller)
    {
        Dictionary<Vector2Int, float> newTiles = new Dictionary<Vector2Int, float>();
        Vector2 t = GetTargetPos(caller);

        foreach (var tile in tiles)
        {

            Vector2 tilePos = tilemap.GetGlobalPositionFromTile(tile.Key);
            Vector3 direction = t - tilePos;

            RaycastHit2D hit = Physics2D.CircleCast(tilePos, .1f, direction);

            BaseMob mob = caller.GetComponent<BaseMob>();
            if (hit && target == EQSTarget.TARGET ?
            hit.collider.transform == mob.Target :
            hit.collider == caller.GetComponent<Collider2D>())
            {
                newTiles[tile.Key] = tiles[tile.Key] + scoreModifier;
            }
            else if (!ignoreFailedTiles)
            {
                newTiles[tile.Key] = tiles[tile.Key];
            }
        }
        return newTiles;
    }

    public override string GetSummary()
    {
        return $"The tile has line of sight to the {target.ToString().ToLower()}";
    }
}
