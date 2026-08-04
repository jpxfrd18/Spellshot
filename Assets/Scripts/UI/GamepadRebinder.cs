using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadRebinder : MonoBehaviour
{
    public static GamepadRebinder Instance { get; private set; }

    private List<string> validPaths = new List<string>
    {
        "<Gamepad>/leftTrigger",
        "<Gamepad>/rightTrigger",
        "<Gamepad>/leftShoulder",
        "<Gamepad>/rightShoulder",
        "<Gamepad>/buttonSouth",
        "<Gamepad>/buttonWest",
        "<Gamepad>/buttonNorth",
        "<Gamepad>/buttonEast"
    };

    private int spell1Index;
    private int spell2Index;
    private int spell3Index;
    private int spell4Index;
    private int jumpIndex;

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

        for (int i = 0; i < validPaths.Count; i++)
        {
            switch (validPaths[i])
            {
                case "<Gamepad>/leftTrigger":
                    spell1Index = i;
                    break;
                case "<Gamepad>/rightTrigger":
                    spell2Index = i;
                    break;
                case "<Gamepad>/leftShoulder":
                    spell3Index = i;
                    break;
                case "<Gamepad>/rightShoulder":
                    spell4Index = i;
                    break;
                case "<Gamepad>/buttonSouth":
                    jumpIndex = i;
                    break;
            }
        }
    }

    public string Increment(InputActionAsset asset, string actionName)
    {
        switch (actionName)
        {
            case "Spell1":
                ActionRebinder.Rebind(asset, actionName, validPaths[spell1Index], validPaths[GetNextIndex(spell1Index)]);
                spell1Index = GetNextIndex(spell1Index);
                return GetShortName(validPaths[spell1Index]);
            case "Spell2":
                ActionRebinder.Rebind(asset, actionName, validPaths[spell2Index], validPaths[GetNextIndex(spell2Index)]);
                spell2Index = GetNextIndex(spell2Index);
                return GetShortName(validPaths[spell2Index]);
            case "Spell3":
                ActionRebinder.Rebind(asset, actionName, validPaths[spell3Index], validPaths[GetNextIndex(spell3Index)]);
                spell3Index = GetNextIndex(spell3Index);
                return GetShortName(validPaths[spell3Index]);
            case "Spell4":
                ActionRebinder.Rebind(asset, actionName, validPaths[spell4Index], validPaths[GetNextIndex(spell4Index)]);
                spell4Index = GetNextIndex(spell4Index);
                return GetShortName(validPaths[spell4Index]);
            case "Jump":
                ActionRebinder.Rebind(asset, actionName, validPaths[jumpIndex], validPaths[GetNextIndex(jumpIndex)]);
                jumpIndex = GetNextIndex(jumpIndex);
                return GetShortName(validPaths[jumpIndex]);
            default:
                Debug.LogError("Invalid action name: " + actionName);
                return "";
        }
    }

    public string Decrement(InputActionAsset asset, string actionName)
    {
        switch (actionName)
        {
            case "Spell1":
                ActionRebinder.Rebind(asset, actionName, validPaths[spell1Index], validPaths[GetPreviousIndex(spell1Index)]);
                spell1Index = GetPreviousIndex(spell1Index);
                return GetShortName(validPaths[spell1Index]);
            case "Spell2":
                ActionRebinder.Rebind(asset, actionName, validPaths[spell2Index], validPaths[GetPreviousIndex(spell2Index)]);
                spell2Index = GetPreviousIndex(spell2Index);
                return GetShortName(validPaths[spell2Index]);
            case "Spell3":
                ActionRebinder.Rebind(asset, actionName, validPaths[spell3Index], validPaths[GetPreviousIndex(spell3Index)]);
                spell3Index = GetPreviousIndex(spell3Index);
                return GetShortName(validPaths[spell3Index]);
            case "Spell4":
                ActionRebinder.Rebind(asset, actionName, validPaths[spell4Index], validPaths[GetPreviousIndex(spell4Index)]);
                spell4Index = GetPreviousIndex(spell4Index);
                return GetShortName(validPaths[spell4Index]);
            case "Jump":
                ActionRebinder.Rebind(asset, actionName, validPaths[jumpIndex], validPaths[GetPreviousIndex(jumpIndex)]);
                jumpIndex = GetPreviousIndex(jumpIndex);
                return GetShortName(validPaths[jumpIndex]);
            default:
                Debug.LogError("Invalid action name: " + actionName);
                return "";
        }
    }

    private int GetNextIndex(int currentIndex)
    {
        int nextIndex = currentIndex + 1;
        if (nextIndex >= validPaths.Count)
        {

            nextIndex = 0;
        }

        while (validPaths[nextIndex] == validPaths[spell1Index]
            || validPaths[nextIndex] == validPaths[spell2Index]
            || validPaths[nextIndex] == validPaths[spell3Index]
            || validPaths[nextIndex] == validPaths[spell4Index]
            || validPaths[nextIndex] == validPaths[jumpIndex])
        {
            nextIndex++;

            if (nextIndex >= validPaths.Count)
            {
                nextIndex = 0;
            }
        }


        return nextIndex;
    }

    private int GetPreviousIndex(int currentIndex)
    {
        int previousIndex = currentIndex - 1;
        if (previousIndex < 0)
        {
            previousIndex = validPaths.Count - 1;
        }

        while (validPaths[previousIndex] == validPaths[spell1Index]
            || validPaths[previousIndex] == validPaths[spell2Index]
            || validPaths[previousIndex] == validPaths[spell3Index]
            || validPaths[previousIndex] == validPaths[spell4Index]
            || validPaths[previousIndex] == validPaths[jumpIndex])
        {
            previousIndex--;

            if (previousIndex < 0)
            {
                previousIndex = validPaths.Count - 1;
            }
        }

        return previousIndex;
    }

    private string GetShortName(string path)
    {
        switch (path)
        {
            case "<Gamepad>/leftTrigger":
                return "LT";
            case "<Gamepad>/rightTrigger":
                return "RT";
            case "<Gamepad>/leftShoulder":
                return "LB";
            case "<Gamepad>/rightShoulder":
                return "RB";
            case "<Gamepad>/buttonSouth":
                return "A";
            case "<Gamepad>/buttonEast":
                return "B";
            case "<Gamepad>/buttonWest":
                return "X";
            case "<Gamepad>/buttonNorth":
                return "Y";
            default:
                return "None";
        }
    }
}