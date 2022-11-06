using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Environment Query System")]
public class EnvironmentQuerySystem : DecisionTreeEditorNode
{

    public EQSTarget Target
    {
        get { return _target; }
        protected set { _target = value; }
    }
    public float TileRange
    {
        get { return _tileRange; }
        protected set { _tileRange = value; }
    }

    [Tooltip("An array of EQS Rules to run everytime this query is ran")]
    [SerializeField] protected List<Rule> rules;
    [Tooltip("The target to check tiles around")]
    [SerializeField] protected EQSTarget _target;
    [Tooltip("The radius of tiles around the target to run the EQS Rules on")]
    [SerializeField] protected float _tileRange;

    Dictionary<Vector2Int, float> tileScore;
    GameObject caller;

    /// Editor Values
    [HideInInspector] public Dictionary<string, DecisionTreeNode> connections = new Dictionary<string, DecisionTreeNode>();

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
                if (tile.Key == bestTile)
                    Debug.DrawLine(new Vector3(tile.Key.x, tile.Key.y, 0.0f), new Vector3(tile.Key.x + 1.0f, tile.Key.y + 1.0f, 0), Color.yellow, 1f);
                else
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
    [SerializeField] protected EQSTarget target;

    public abstract Dictionary<Vector2Int, float> Run(Dictionary<Vector2Int, float> tiles, GameObject caller);

    public void Initialise()
    {
    }

    public Vector2 GetTargetPos(GameObject caller)
    {
        Vector3 pos;
        switch (target)
        {
            case EQSTarget.PLAYER:
                pos = GameObject.FindGameObjectWithTag("Player").transform.position;
                break;
            case EQSTarget.CALLER:
                pos = caller.transform.position;
                break;
            default:
                pos = Vector2.zero;
                break;
        }
        return pos;
    }
}

public enum EQSTarget
{
    PLAYER,
    CALLER
}
