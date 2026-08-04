using System.Collections.Generic;
using UnityEngine;

public class MenuNavigation : MonoBehaviour
{
    public static MenuNavigation Instance { get; private set; }
    public List<NavNode> mainNodes { get; private set; } = new List<NavNode>();
    public List<NavNode> levelSelectNodes { get; private set; } = new List<NavNode>();
    public List<NavNode> popupNodes { get; private set; } = new List<NavNode>();
    public List<NavNode> levelNodes { get; private set; } = new List<NavNode>();
    public List<NavNode> settingsNodes { get; private set; } = new List<NavNode>();
    public List<NavNode> controlsNodes { get; private set; } = new List<NavNode>();
    public List<NavNode> keyboardNodes { get; private set; } = new List<NavNode>();
    public List<NavNode> controllerNodes { get; private set; } = new List<NavNode>();
    public List<NavNode> pauseNodes { get; private set; } = new List<NavNode>();
    public List<NavNode> winNodes { get; private set; } = new List<NavNode>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMain(List<NavNode> newList)
    {
        mainNodes = newList;
    }

    public void SetLevelSelect(List<NavNode> newList)
    {
        levelSelectNodes = newList;
    }

    public void SetPopup(List<NavNode> newList)
    {
        popupNodes = newList;
    }

    public void SetLevel(List<NavNode> newList)
    {
        levelNodes = newList;
    }

    public void SetSettings(List<NavNode> newList)
    {
        settingsNodes = newList;
    }

    public void SetControls(List<NavNode> newList)
    {
        controlsNodes = newList;
    }

    public void SetKeyboard(List<NavNode> newList)
    {
        keyboardNodes = newList;
    }

    public void SetController(List<NavNode> newList)
    {
        controllerNodes = newList;
    }

    public void SetPause(List<NavNode> newList)
    {
        pauseNodes = newList;
    }

    public void SetWin(List<NavNode> newList)
    {
        winNodes = newList;
    }
}