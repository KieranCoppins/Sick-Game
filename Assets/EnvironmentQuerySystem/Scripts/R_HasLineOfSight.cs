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
        Vector2 t = GetTargetPos(caller);

        foreach (var tile in tiles)
        {
            RaycastHit2D hit;
            Vector2 tilePos = new Vector3(tile.Key.x + 0.5f, tile.Key.y + 0.5f, 0);
            Vector2 direction = t - tilePos;
            Debug.DrawRay(tilePos, direction, Color.white, 1.0f);
            hit = Physics2D.Raycast(tilePos, direction);
            if (hit && target == RuleTarget.PLAYER ? hit.collider.CompareTag("Player") : hit.collider == caller.GetComponent<Collider2D>())
            {
                newTiles[tile.Key] = tiles[tile.Key] + scoreModifier;
                Debug.Log(hit.collider);
            }
            else if (!ignoreFailedTiles)
            {
                newTiles[tile.Key] = tiles[tile.Key];
            }
        }
        return newTiles;
    }
}
