using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityRadialMenu : RadialMenu
{
    [SerializeField] CharacterMovement character;

    public override void LoadData()
    {
        if (menuOptions.Count > 0)
            menuOptions.Clear();

        Debug.Log(character);
        Debug.Log(character.abilityQuickbar);

        foreach (var ability in character.abilityQuickbar)
        {
            RadialMenuOption menuOption = new RadialMenuOption(ability.name, ability.GetDescription(), ability.icon);
            menuOptions.Add(menuOption);
        }
    }

    public override void Close()
    {
        if (Open)
        {
            character.selectedAbility = character.abilityQuickbar[menuOptions.IndexOf(selectedOption)];
            base.Close();
        }
    }
}
