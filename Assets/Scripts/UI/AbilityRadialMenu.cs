using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityRadialMenu : RadialMenu
{
    [SerializeField] private CharacterMovement _character;

    public override void LoadData()
    {
        if (MenuOptions.Count > 0)
            MenuOptions.Clear();

        foreach (var ability in _character.AbilityQuickbar)
        {
            RadialMenuOption menuOption = new RadialMenuOption(ability.name, ability.GetDescription(), ability.Icon);
            MenuOptions.Add(menuOption);
        }
    }

    public override void Close()
    {
        if (Open)
        {
            _character.SelectedAbility = _character.AbilityQuickbar[MenuOptions.IndexOf(SelectedOption)];
            base.Close();
        }
    }
}
