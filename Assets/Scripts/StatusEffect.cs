using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KieranCoppins.GenericHelpers;

[System.Flags]
public enum StatusEffectFlags
{
    shouldReset = 1 << 0,
    shouldHappenOverTime = 1 << 1,
}

[CreateAssetMenu(menuName = "Status Effect")]
public class StatusEffect : ScriptableObject
{
    [Header("Status Effect")]
    [Tooltip("The stat this Status Effect will modify")]
    [SerializeField] public PlayerStat Stat;
    [Tooltip("The amount this Status Effect will modify the stat")]
    [SerializeField] public int Value;
    [Tooltip("The duration in seconds of this Status Effect")]
    [SerializeField] public float Duration;
    [EnumFlags]
    [SerializeField] public StatusEffectFlags Flags;
    [Tooltip("The icon to display this status effect on the affected (Currently unused)")]
    public Image Icon;

    public IEnumerator Apply(BaseCharacter character)
    {
        System.Reflection.PropertyInfo property = typeof(BaseCharacter).GetProperty(Stat, GenericHelpers.GetFieldFlags);
        int previousValue = (int)property.GetValue(character);
        if ((Flags & StatusEffectFlags.shouldHappenOverTime) == StatusEffectFlags.shouldHappenOverTime)
        {
            float timeStart = Time.time;
            float timeSince = 0f;
            int newValue = Value;
            while (timeSince < Duration)
            {
                property.SetValue(character, previousValue + newValue);
                newValue += Value;
                timeSince = Time.time - timeStart;
                yield return new WaitForSeconds(1);
            }
        }
        else
        {
            property.SetValue(character, previousValue + Value);
            yield return new WaitForSeconds(Duration);
        }
        if ((Flags & StatusEffectFlags.shouldReset) == StatusEffectFlags.shouldReset)
        {
            property.SetValue(character, previousValue);
        }
        yield return null;
    }
}

[System.Serializable]
public class PlayerStat
{
    [SerializeField] private string _statName;

    public override string ToString() => _statName;
    public static implicit operator string(PlayerStat playerStat) => playerStat._statName;
}

public class EnumFlagsAttribute : PropertyAttribute
{

}
