using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using KieranCoppins.DecisionTrees;

[CreateAssetMenu(menuName = "Environment Query System")]
public class EnvironmentQuerySystem : ScriptableObject
{
    public EQSTarget Target
    {
        get;
        protected set;
    }
    public float TileRange
    {
        get { return _tileRange; }
        protected set { _tileRange = value; }
    }

    protected BaseMob Mob { get; set; }

    [Tooltip("An array of EQS Rules to run everytime this query is ran")]
    [SerializeField] protected List<Rule> Rules;
    [Tooltip("The target to check tiles around")]
    [SerializeField] private EQSTarget _target;
    [Tooltip("The radius of tiles around the target to run the EQS Rules on")]
    [SerializeField] private float _tileRange;

    private Dictionary<Vector2Int, float> _tileScore;
    private GameObject _caller;

    public Vector2 Run()
    {
        // Get our controller
        TilemapController controller = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<TilemapController>();

        Transform target;

        // Get the target for this given eqs system
        switch (Target)
        {
            case (EQSTarget.TARGET):
                target = Mob.Target;
                break;
            case (EQSTarget.CALLER):
                target = Mob.transform;
                break;
            default:
                Debug.LogError("EQS system does not have a valid EQSTarget");
                return Vector2.zero;
        }

        if (target== null) { return Vector2.zero; }

        // Convert our float vector3 to a int vector 3 by flooring out ints to get the right tile coord
        Vector3Int pos = new Vector3Int(Mathf.FloorToInt(target.position.x), Mathf.FloorToInt(target.position.y));

        // Use the tilemap controller to get all tiles within range
        Vector2Int[] tiles = controller.GetTilesInRange(pos, Mathf.CeilToInt(TileRange));

        //Initialise our EQS system with these parameters
        _caller = Mob.gameObject;

        _tileScore = new Dictionary<Vector2Int, float>();

        // Default all tiles in the query to have a score of 1;
        for (int i = 0; i < tiles.Length; i++)
        {
            this._tileScore.Add(tiles[i], 1f);
        }


        foreach (Rule rule in Rules)
        {
            _tileScore = rule.Run(_tileScore, _caller);
        }
        float lowestScore = Mathf.Infinity;
        float highestScore = 0;
        Vector2Int bestTile = Vector2Int.zero;
        foreach (var tile in _tileScore)
        {
            if (tile.Value < lowestScore)
            {
                lowestScore = tile.Value;
                bestTile = tile.Key;
            }

            if (tile.Value > highestScore)
                highestScore = tile.Value;
        }

        if ((_caller.GetComponent<BaseMob>().DebugFlags & DebugFlags.EQS) == DebugFlags.EQS)
        {
            foreach (var tile in _tileScore)
            {
                if (tile.Key == bestTile)
                    Debug.DrawLine(new Vector3(tile.Key.x, tile.Key.y, 0.0f), new Vector3(tile.Key.x + 1.0f, tile.Key.y + 1.0f, 0), Color.yellow, 1f);
                else
                    Debug.DrawLine(new Vector3(tile.Key.x, tile.Key.y, 0.0f), new Vector3(tile.Key.x + 1.0f, tile.Key.y + 1.0f, 0), Color.Lerp(Color.red, Color.green, 1 - ((tile.Value - lowestScore) / (highestScore - lowestScore))), 1f);
            }
        }

        return new Vector2(bestTile.x + 0.5f, bestTile.y + 0.5f);
    }

    public void Initialise(BaseMob mob)
    {
        Mob = mob;
    }

    public EnvironmentQuerySystem Clone()
    {
        return Instantiate(this) as EnvironmentQuerySystem;
    }

    public string GetDescription(BaseNodeView nodeView)
    {
        if (Rules.Count == 0)
        {
            nodeView.Error = "There are no rules in this Environment Query System!";
            return "";
        }
        nodeView.Error = "";
        string desc = "Returns the location that satisfies these rules:\n";
        foreach (var rule in Rules)
        {
            desc += $"{rule.GetSummary()}. \n";
        }
        return desc;
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
            case EQSTarget.TARGET:
                BaseMob mob = caller.GetComponent<BaseMob>();
                pos = mob.Target.position;
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

    public abstract string GetSummary();
}

public enum EQSTarget
{
    TARGET,
    CALLER
}
