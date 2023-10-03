using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using KieranCoppins.GenericHelpers;

[CreateAssetMenu(menuName = "Environment Queries/Distance To")]
public class R_DistanceTo : Rule
{

    [SerializeField] private float _distance;
    [SerializeField] private Operators _operation;

    public override Dictionary<Vector2Int, float> Run(TilemapController tilemap, Dictionary<Vector2Int, float> tiles, GameObject caller)
    {
        Dictionary<Vector2Int, float> newTiles = new Dictionary<Vector2Int, float>();

        Vector2 target = GetTargetPos(caller);

        foreach (var tile in tiles)
        {
            // Check the distance from the tile's mid point instead of the origin
            float d = Vector2.Distance(tilemap.GetGlobalPositionFromTile(tile.Key), target);
            if (Decision(d))
                newTiles[tile.Key] = tiles[tile.Key] + scoreModifier;
            else if (!ignoreFailedTiles)
                newTiles[tile.Key] = tiles[tile.Key];
        }

        return newTiles;
    }

    public bool Decision(float d)
    {
        if (_operation == Operators.LessThan)
            return d < _distance;
        else if (_operation == Operators.GreaterThan)
            return d > _distance;
        else if (_operation == Operators.EqualTo)
            return d == _distance;
        return false;
    }

    public override string GetSummary()
    {
        return $"The tile's distance is {GenericHelpers.SplitCamelCase(_operation.ToString()).ToLower()} {_distance}";
    }
}
