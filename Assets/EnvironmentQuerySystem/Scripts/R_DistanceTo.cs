using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Environment Queries/Distance To")]
public class R_DistanceTo : Rule
{
    enum Operators
    {
        LessThan,
        GreaterThan,
        EqualTo
    }

    [SerializeField] float distance;
    [SerializeField] Operators op;

    public override Dictionary<Vector2Int, float> Run(Dictionary<Vector2Int, float> tiles, GameObject caller)
    {
        Dictionary<Vector2Int, float> newTiles = new Dictionary<Vector2Int, float>();

        tilemapController = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<TilemapController>();

        Vector2 target = GetTargetPos(caller);

        foreach (var tile in tiles)
        {
            float d = Vector2.Distance(tile.Key, target);
            if (Decision(d))
                newTiles[tile.Key] = tiles[tile.Key] + scoreModifier;
            else if (!ignoreFailedTiles)
                newTiles[tile.Key] = tiles[tile.Key];
        }

        return newTiles;
    }

    public bool Decision(float d)
    {
        if (op == Operators.LessThan)
            return d < distance;
        else if (op == Operators.GreaterThan)
            return d > distance;
        else if (op == Operators.EqualTo)
            return d == distance;
        return false;
    }
}
