using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EQSManager : MonoBehaviour
{
    [SerializeField] List<EnvironmentQuerySystem> EQSSystems;

    public EnvironmentQuerySystem GetEQS(EQSystem eqs)
    {
        return EQSSystems[(int)eqs];
    }

    public Vector2 RunEQSystem(EQSystem eqs, GameObject caller)
    {
        // Get our controller
        TilemapController controller = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<TilemapController>();

        Transform target;

        EnvironmentQuerySystem EQS = GetEQS(eqs);

        // Get the target for this given eqs system
        switch (EQS.Target)
        {
            case (EQSTarget.PLAYER):
                target = GameObject.FindGameObjectWithTag("Player").transform;
                break;
            case (EQSTarget.CALLER):
                target = caller.transform;
                break;
            default:
                Debug.LogError("EQS system does not have a valid EQSTarget");
                return Vector2.zero;
        }

        // Convert our float vector3 to a int vector 3 by flooring out ints to get the right tile coord
        Vector3Int pos = new Vector3Int(Mathf.FloorToInt(target.position.x), Mathf.FloorToInt(target.position.y));

        // Use the tilemap controller to get all tiles within range
        Vector2Int[] tiles = controller.GetTilesInRange(pos, Mathf.CeilToInt(EQS.TileRange));

        //Initialise our EQS system with these parameters
        EQS.Initialise(tiles, caller);
        // Run the EQS system to get our tile and get the postiion of the tile
        return EQS.Run();
    }
}

public enum EQSystem
{
    RangedMobMoveToPlayer,
    MeleeMobMoveToPlayer,

}
