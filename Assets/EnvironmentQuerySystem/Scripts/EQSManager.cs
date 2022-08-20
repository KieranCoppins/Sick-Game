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

    public Vector2 RunEQSystem(EQSystem eqs, float range, Vector2 target, GameObject caller)
    {
        // Get our controller
        TilemapController controller = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<TilemapController>();

        // Convert our float vector3 to a int vector 3 by flooring out ints to get the right tile coord
        Vector3Int pos = new Vector3Int(Mathf.FloorToInt(target.x), Mathf.FloorToInt(target.y));

        // Use the tilemap controller to get all tiles within range
        Vector2Int[] tiles = controller.GetTilesInRange(pos, Mathf.CeilToInt(range));

        //Initialise our EQS system with these parameters
        GetEQS(EQSystem.RangedMobMoveToPlayer).Initialise(controller, tiles, caller);
        // Run the EQS system to get our tile and get the postiion of the tile
        return GetEQS(EQSystem.RangedMobMoveToPlayer).Run();
    }
}

public enum EQSystem
{
    RangedMobMoveToPlayer,

}
