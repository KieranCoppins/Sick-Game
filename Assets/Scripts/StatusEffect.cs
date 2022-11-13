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
    [SerializeField] PlayerStat stat;
    [Tooltip("The amount this Status Effect will modify the stat")]
    [SerializeField] int value;
    [Tooltip("The duration in seconds of this Status Effect")]
    [SerializeField] float duration;
    [EnumFlags]
    [SerializeField] StatusEffectFlags flags;
    [Tooltip("The icon to display this status effect on the affected (Currently unused)")]
    public Image icon;

    public IEnumerator Apply(BaseCharacter character)
    {
        System.Reflection.BindingFlags getFieldFlags = System.Reflection.BindingFlags.Default;
        getFieldFlags |= System.Reflection.BindingFlags.Public;
        getFieldFlags |= System.Reflection.BindingFlags.NonPublic;
        getFieldFlags |= System.Reflection.BindingFlags.Instance;
        System.Reflection.FieldInfo field = typeof(BaseCharacter).GetField(stat, getFieldFlags);
        int previousValue = (int)field.GetValue(character);
        if ((flags & StatusEffectFlags.shouldHappenOverTime) == StatusEffectFlags.shouldHappenOverTime)
        {
            float timeLeft = duration;
            int newValue = value;
            while (timeLeft > 0)
            {
                field.SetValue(character, previousValue + newValue);
                newValue += value;
                timeLeft -= Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            field.SetValue(character, previousValue + value);
            yield return new WaitForSeconds(duration);
        }
        if ((flags & StatusEffectFlags.shouldReset) == StatusEffectFlags.shouldReset)
        {
            field.SetValue(character, previousValue);
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
