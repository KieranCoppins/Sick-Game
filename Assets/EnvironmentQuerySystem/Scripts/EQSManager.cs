using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EQSManager : MonoBehaviour
{
    [SerializeField] List<EnvironmentQuerySystem> EQSSystems;

    public EnvironmentQuerySystem GetEQS(EQSSystem eqs)
    {
        return EQSSystems[(int)eqs];
    }
}

public enum EQSSystem
{
    RangedMobMoveToPlayer,

}
