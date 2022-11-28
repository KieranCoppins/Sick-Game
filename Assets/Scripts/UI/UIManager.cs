using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The main manager to handle all of our UI. This includes transitioning between UI elements, 
/// ensuring correct elements are not active when other elements are
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject inventoryRadialMenu;
    [SerializeField] GameObject abilityRadialMenu;
    [SerializeField] GameObject playerHUD;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] List<GameObject> pauseMenuScreens = new List<GameObject>();

    int CurrentMenuIndex
    {
        get { return _currentMenuIndex; }
        set
        {
            if (value >= pauseMenuScreens.Count)
                _currentMenuIndex = 0;
            else if (value < 0)
                _currentMenuIndex = pauseMenuScreens.Count - 1;
            else
                _currentMenuIndex = value;
        }
    }
    int _currentMenuIndex = 0;

    Dictionary<UIElement, GameObject> uiElements= new Dictionary<UIElement, GameObject>();

    private void Start()
    {
        uiElements.Add(UIElement.InventoryRadialMenu, inventoryRadialMenu);
        uiElements.Add(UIElement.AbilityRadialMenu, abilityRadialMenu);
        uiElements.Add(UIElement.PauseMenu, pauseMenu);

    }

    void Open(UIElement uiElement)
    {
        foreach (var item in uiElements) 
        {
            if (item.Key == uiElement)
            {
                // Open this one
                RadialMenu radialMenu = item.Value.GetComponent<RadialMenu>();
                if (radialMenu != null)
                    radialMenu.Display();
                else if (item.Key == UIElement.PauseMenu)
                {
                    playerHUD.SetActive(false);
                    pauseMenu.SetActive(true);
                    pauseMenuScreens[CurrentMenuIndex].SetActive(true);
                    pauseMenuScreens[CurrentMenuIndex].GetComponentInChildren<ListMenu>().Display();
                }
                else
                    item.Value.SetActive(true);
            }
            else
            {
                // Close them - we don't mind not playing any closing animation if we're trying to open a different menu
                item.Value.SetActive(false);
            }
        }
    }

    void Close(UIElement uiElement)
    {
        foreach (var item in uiElements)
        {
            if (item.Key == uiElement)
            {
                // Close this one
                if (item.Value.activeSelf == true)
                {
                    RadialMenu radialMenu = item.Value.GetComponent<RadialMenu>();
                    if (radialMenu != null)
                        radialMenu.Close();
                    else if (item.Key == UIElement.PauseMenu)
                    {
                        playerHUD.SetActive(true);
                        item.Value.SetActive(false);
                        pauseMenuScreens[CurrentMenuIndex].SetActive(false);
                    }
                    else
                        item.Value.SetActive(false);
                }
            }
        }
    }

    /// Input Functions (Until there is a better method)
    
    public void OpenInventoryRadial(InputAction.CallbackContext context)
    {
        if (!pauseMenu.activeSelf)
        {
            if (context.performed)
                Open(UIElement.InventoryRadialMenu);
            if (context.canceled)
                Close(UIElement.InventoryRadialMenu);
        }
    }

    public void OpenAbilityRadial(InputAction.CallbackContext context)
    {
        if (!pauseMenu.activeSelf)
        {
            if (context.performed)
                Open(UIElement.AbilityRadialMenu);
            if (context.canceled)
                Close(UIElement.AbilityRadialMenu);
        }
    }

    public void TogglePauseMenu(InputAction.CallbackContext context)
    {
        if (context.started)
        {

            if (pauseMenu.activeSelf)
                Close(UIElement.PauseMenu);
            else
                Open(UIElement.PauseMenu);
        }
    }

    public void NextPauseMenu(InputAction.CallbackContext context)
    {
        if (context.started && pauseMenu.activeSelf)
        {
            pauseMenuScreens[CurrentMenuIndex].SetActive(false);
            CurrentMenuIndex++;
            pauseMenuScreens[CurrentMenuIndex].SetActive(true);
            pauseMenuScreens[CurrentMenuIndex].GetComponentInChildren<ListMenu>().Display();
        }
    }

    public void PrevPauseMenu(InputAction.CallbackContext context)
    {
        if (context.started && pauseMenu.activeSelf)
        {
            pauseMenuScreens[CurrentMenuIndex].SetActive(false);
            CurrentMenuIndex--;
            pauseMenuScreens[CurrentMenuIndex].SetActive(true);
            pauseMenuScreens[CurrentMenuIndex].GetComponentInChildren<ListMenu>().Display();
        }
    }

    public void SelectItem(InputAction.CallbackContext context)
    {
        // If we're using our radial menu, then we want to use the right stick for manouvering the radial menu
        if (!context.canceled)
        {
            InventoryRadialMenu inventoryMenu = inventoryRadialMenu.GetComponent<InventoryRadialMenu>();
            if (inventoryMenu && inventoryMenu.Open)
            {
                inventoryMenu.SelectItem(context.ReadValue<Vector2>());
                return;
            }
            AbilityRadialMenu abilityMenu = abilityRadialMenu.GetComponent<AbilityRadialMenu>();
            if (abilityMenu && abilityMenu.Open)
            {
                abilityMenu.SelectItem(context.ReadValue<Vector2>());
            }
        }
    }

    public void NavigateList(InputAction.CallbackContext context)
    {
        if (context.started && pauseMenu.activeInHierarchy)
            pauseMenuScreens[CurrentMenuIndex].GetComponentInChildren<ListMenu>().SelectItem(context.ReadValue<Vector2>());
    }

    public void Select(InputAction.CallbackContext context)
    {
        if (context.started && pauseMenu.activeInHierarchy)
        {
            ListMenu listMenu = pauseMenuScreens[CurrentMenuIndex].GetComponentInChildren<ListMenu>();
            if (listMenu.IsSubmenuOpen())
                listMenu.CloseSubmenu();
            else
                listMenu.OpenSubmenu();
        }
    }

}

public enum UIElement
{
    InventoryRadialMenu,
    AbilityRadialMenu,
    PauseMenu,
}
