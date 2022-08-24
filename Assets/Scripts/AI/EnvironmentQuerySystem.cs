using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Environment Query System")]
public class EnvironmentQuerySystem : ScriptableObject
{
    TilemapController tilemapController;
    Dictionary<Vector2Int, float> tileScore;
    public List<Rule> rules;
    GameObject caller;

    public void Initialise(TilemapController tilemapController, Vector2Int[] tileCoords, GameObject caller)
    {
        this.tilemapController = tilemapController;
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
        Vector2Int bestTile = Vector2Int.zero;
        foreach (var tile in tileScore)
        {
            if (tile.Value < lowestScore)
            {
                lowestScore = tile.Value;
                bestTile = tile.Key;
            }

            // DEBUG - draw a line and the lighter the red the more ideal the tile was
            if (caller.GetComponent<BaseMob>().DebugMode)
                Debug.DrawLine(new Vector3(tile.Key.x, tile.Key.y, 0.0f), new Vector3(tile.Key.x + 1.0f, tile.Key.y + 1.0f, 0), new Color(1.0f - (tile.Value / 8.0f), 0.0f, 0.0f), 20.0f);
        }

        return new Vector2(bestTile.x, bestTile.y);
    }

}

public abstract class Rule : ScriptableObject
{
    [SerializeField] protected bool ignoreFailedTiles = false;
    [SerializeField] protected float scoreModifier = 1f;
    [SerializeField] protected RuleTarget target;

    protected TilemapController tilemapController;

    public abstract Dictionary<Vector2Int, float> Run(Dictionary<Vector2Int, float> tiles, GameObject caller);

    public void Initialise()
    {
        tilemapController = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<TilemapController>();
    }

    public Vector2Int GetTargetTile(GameObject caller)
    {

        Vector2Int tileCoord;
        Vector3 pos;
        switch (this.target)
        {
            case RuleTarget.PLAYER:
                pos = GameObject.FindGameObjectWithTag("Player").transform.position;
                tileCoord = new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
                break;
            case RuleTarget.CALLER:
                pos = caller.transform.position;
                tileCoord = new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
                break;
            default:
                tileCoord = Vector2Int.zero;
                break;
        }
        return tileCoord;
    }
}

public enum RuleTarget
{
    PLAYER,
    CALLER
}
