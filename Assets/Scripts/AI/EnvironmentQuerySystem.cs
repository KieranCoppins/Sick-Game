using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Environment Query System")]
public class EnvironmentQuerySystem : ScriptableObject
{
    Dictionary<Vector2Int, float> tileScore;
    public List<Rule> rules;
    GameObject caller;

    public void Initialise(Vector2Int[] tileCoords, GameObject caller)
    {
        this.caller = caller;

        tileScore = new Dictionary<Vector2Int, float>();

        // Default all tiles in the query to have a score of 1;
        for (int i = 0; i < tileCoords.Length; i++)
        {
            this.tileScore.Add(tileCoords[i], 1f);
        }

    }

    public Vector2 Run()
    {
        foreach(Rule rule in rules)
        {
            tileScore = rule.Run(tileScore, caller);
        }
        float lowestScore = Mathf.Infinity;
        float highestScore = 0;
        Vector2Int bestTile = Vector2Int.zero;
        foreach (var tile in tileScore)
        {
            if (tile.Value < lowestScore)
            {
                lowestScore = tile.Value;
                bestTile = tile.Key;
            }

            if (tile.Value > highestScore)
                highestScore = tile.Value;
        }

        if ((caller.GetComponent<BaseMob>().debugFlags & DebugFlags.EQS) == DebugFlags.EQS)
        {
            foreach (var tile in tileScore)
            {
                Debug.DrawLine(new Vector3(tile.Key.x, tile.Key.y, 0.0f), new Vector3(tile.Key.x + 1.0f, tile.Key.y + 1.0f, 0), Color.Lerp(Color.red, Color.green, 1 - ((tile.Value - lowestScore) / (highestScore - lowestScore))), 1f);
            }
        }

        return new Vector2(bestTile.x + 0.5f, bestTile.y + 0.5f);
    }

}

public abstract class Rule : ScriptableObject
{
    [SerializeField] protected bool ignoreFailedTiles = false;
    [SerializeField] protected float scoreModifier = 1f;
    [SerializeField] protected RuleTarget target;

    public abstract Dictionary<Vector2Int, float> Run(Dictionary<Vector2Int, float> tiles, GameObject caller);

    public void Initialise()
    {
    }

    public Vector2 GetTargetPos(GameObject caller)
    {
        Vector3 pos;
        switch (target)
        {
            case RuleTarget.PLAYER:
                pos = GameObject.FindGameObjectWithTag("Player").transform.position;
                break;
            case RuleTarget.CALLER:
                pos = caller.transform.position;
                break;
            default:
                pos = Vector2.zero;
                break;
        }
        return pos;
    }
}

public enum RuleTarget
{
    PLAYER,
    CALLER
}
