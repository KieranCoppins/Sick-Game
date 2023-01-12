using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Environment Queries/Has Line Of Sight")]
public class R_HasLineOfSight : Rule
{
    public override Dictionary<Vector2Int, float> Run(Dictionary<Vector2Int, float> tiles, GameObject caller)
    {
        Dictionary<Vector2Int, float> newTiles = new Dictionary<Vector2Int, float>();
        Vector2 t = GetTargetPos(caller);

        foreach (var tile in tiles)
        {

            Vector2 tilePos = new Vector3(tile.Key.x + 0.5f, tile.Key.y + 0.5f, 0);
            Vector3 direction = t - tilePos;

            RaycastHit2D lowerHit;
            RaycastHit2D upperHit;
            float angle = Vector2.Angle(Vector2.up, (Vector2)direction);
            angle = direction.x > 0 ? angle : -angle;
            Vector2 lowerStart = tilePos + (Vector2)(Quaternion.AngleAxis(angle, Vector3.back) * new Vector2(-0.49f, 0));
            Vector2 upperStart = tilePos + (Vector2)(Quaternion.AngleAxis(angle, Vector3.back) * new Vector2(0.49f, 0));
            lowerHit = Physics2D.Raycast(lowerStart, t - lowerStart);
            upperHit = Physics2D.Raycast(upperStart, t - upperStart);
            BaseMob mob = caller.GetComponent<BaseMob>();
            if (lowerHit && upperHit && target == EQSTarget.TARGET ? lowerHit.collider.transform == mob.Target && upperHit.collider.transform == mob.Target : lowerHit.collider == caller.GetComponent<Collider2D>() && upperHit.collider == caller.GetComponent<Collider2D>())
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
