using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class AbilityListMenu : ListMenu
{
    [SerializeField] private CharacterMovement _character;
    public override void LoadData()
    {
        MenuOptions.Clear();
        foreach (var ability in _character.AllAbilities)
        {
            string favourited = _character.AbilityQuickbar.Contains(ability) ? "*" : "";
            ListMenuOption lmo = new ListMenuOption($"{ability.name}{favourited} <pos=80%>{ability.ManaCost} MP</pos>", ability.GetDescription(), ability.Icon);
            lmo.Metadata = ability;
            MenuOptions.Add(lmo);
        }
    }

    public void FavouriteSelectedItem()
    {
        AbilityBase ability = (AbilityBase)SelectedOption.Metadata;
        if (ability != null)
        {
            if (_character.AbilityQuickbar.Contains(ability))
                _character.AbilityQuickbar.Remove(ability);
            else
                _character.AbilityQuickbar.Add(ability);
        }
    }
}
