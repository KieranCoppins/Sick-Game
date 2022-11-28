using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class AbilityListMenu : ListMenu
{
    [SerializeField] CharacterMovement character;
    public override void LoadData()
    {
        menuOptions.Clear();
        foreach (var ability in character.AllAbilities)
        {
            string favourited = character.abilityQuickbar.Contains(ability) ? "*" : "";
            ListMenuOption lmo = new ListMenuOption($"{ability.name}{favourited} <pos=80%>{ability.ManaCost} MP</pos>", ability.GetDescription(), ability.icon);
            lmo.Metadata = ability;
            menuOptions.Add(lmo);
        }
    }

    public void FavouriteSelectedItem()
    {
        AbilityBase ability = (AbilityBase)SelectedOption.Metadata;
        if (ability != null)
        {
            if (character.abilityQuickbar.Contains(ability))
                character.abilityQuickbar.Remove(ability);
            else
                character.abilityQuickbar.Add(ability);
        }
    }
}
