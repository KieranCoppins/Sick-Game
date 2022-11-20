using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] public PlayerStat stat;
    [Tooltip("The amount this Status Effect will modify the stat")]
    [SerializeField] public int value;
    [Tooltip("The duration in seconds of this Status Effect")]
    [SerializeField] public float duration;
    [EnumFlags]
    [SerializeField] public StatusEffectFlags flags;
    [Tooltip("The icon to display this status effect on the affected (Currently unused)")]
    public Image icon;

    public IEnumerator Apply(BaseCharacter character)
    {
        System.Reflection.PropertyInfo property = typeof(BaseCharacter).GetProperty(stat, GenericHelpers.getFieldFlags);
        int previousValue = (int)property.GetValue(character);
        if ((flags & StatusEffectFlags.shouldHappenOverTime) == StatusEffectFlags.shouldHappenOverTime)
        {
            float timeStart = Time.time;
            float timeSince = 0f;
            int newValue = value;
            while (timeSince < duration)
            {
                property.SetValue(character, previousValue + newValue);
                newValue += value;
                timeSince = Time.time - timeStart;
                yield return new WaitForSeconds(1);
            }
        }
        else
        {
            property.SetValue(character, previousValue + value);
            yield return new WaitForSeconds(duration);
        }
        if ((flags & StatusEffectFlags.shouldReset) == StatusEffectFlags.shouldReset)
        {
            property.SetValue(character, previousValue);
        }
        yield return null;
    }
}

[System.Serializable]
public class PlayerStat
{
    [SerializeField] string statName;

    public override string ToString() => statName;
    public static implicit operator string(PlayerStat playerStat) => playerStat.statName;
}
