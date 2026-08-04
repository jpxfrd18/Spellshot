using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardRebinder : MonoBehaviour
{
    public static KeyboardRebinder Instance { get; private set; }

    private List<string> validPaths = new List<string>
    {
        // High Frequency
        "<Mouse>/leftButton",
        "<Mouse>/rightButton",
        "<Keyboard>/q",
        "<Keyboard>/e",
        "<Keyboard>/r",
        "<Keyboard>/space",
        "<Keyboard>/escape",
        // Meduim Frequency
        "<Keyboard>/f",
        "<Keyboard>/g",
        "<Keyboard>/c",
        "<Keyboard>/v",
        // Low Frequency
        "<Keyboard>/z",
        "<Keyboard>/x",
        "<Keyboard>/t",
        "<Keyboard>/b",
        "<Keyboard>/y",
        "<Keyboard>/h",
        "<Keyboard>/n",
        "<Keyboard>/u",
        "<Keyboard>/j",
        "<Keyboard>/m",
        "<Keyboard>/i",
        "<Keyboard>/k",
        "<Keyboard>/o",
        "<Keyboard>/l",
        "<Keyboard>/p",
        "<Keyboard>/1",
        "<Keyboard>/2",
        "<Keyboard>/3",
        "<Keyboard>/4",
        "<Keyboard>/5",
        "<Keyboard>/6",
        "<Keyboard>/7",
        "<Keyboard>/8",
        "<Keyboard>/9",
        "<Keyboard>/0",
        // Medium Frequency
        "<Keyboard>/rightAlt",
        "<Keyboard>/rightCtrl",
        "<Keyboard>/rightShift",
        "<Mouse>/middleButton",
        "<Keyboard>/tab",
        "<Keyboard>/backspace",
        "<Keyboard>/enter",
        "<Keyboard>/leftAlt",
        "<Keyboard>/leftCtrl",
        "<Keyboard>/leftShift"
    };

    private int spell1Index;
    private int spell2Index;
    private int spell3Index;
    private int spell4Index;
    private int jumpIndex;
    private int pauseIndex;
    private int restartIndex;

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
                case "<Mouse>/leftButton":
                    spell1Index = i;
                    break;
                case "<Mouse>/rightButton":
                    spell2Index = i;
                    break;
                case "<Keyboard>/q":
                    spell3Index = i;
                    break;
                case "<Keyboard>/e":
                    spell4Index = i;
                    break;
                case "<Keyboard>/space":
                    jumpIndex = i;
                    break;
                case "<Keyboard>/escape":
                    pauseIndex = i;
                    break;
                case "<Keyboard>/r":
                    restartIndex = i;
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
            case "Pause":
                ActionRebinder.Rebind(asset, actionName, validPaths[pauseIndex], validPaths[GetNextIndex(pauseIndex)]);
                ActionRebinder.Rebind(asset, "Play", validPaths[pauseIndex], validPaths[GetNextIndex(pauseIndex)]);
                pauseIndex = GetNextIndex(pauseIndex);
                return GetShortName(validPaths[pauseIndex]);
            case "Restart":
                ActionRebinder.Rebind(asset, actionName, validPaths[restartIndex], validPaths[GetNextIndex(restartIndex)]);
                ActionRebinder.Rebind(asset, "RestartUI", validPaths[restartIndex], validPaths[GetNextIndex(restartIndex)]);
                restartIndex = GetNextIndex(restartIndex);
                return GetShortName(validPaths[restartIndex]);
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
            case "Pause":
                ActionRebinder.Rebind(asset, actionName, validPaths[pauseIndex], validPaths[GetPreviousIndex(pauseIndex)]);
                ActionRebinder.Rebind(asset, "Play", validPaths[pauseIndex], validPaths[GetPreviousIndex(pauseIndex)]);
                pauseIndex = GetPreviousIndex(pauseIndex);
                return GetShortName(validPaths[pauseIndex]);
            case "Restart":
                ActionRebinder.Rebind(asset, actionName, validPaths[restartIndex], validPaths[GetPreviousIndex(restartIndex)]);
                ActionRebinder.Rebind(asset, "RestartUI", validPaths[restartIndex], validPaths[GetPreviousIndex(restartIndex)]);
                restartIndex = GetPreviousIndex(restartIndex);
                return GetShortName(validPaths[restartIndex]);
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
            || validPaths[nextIndex] == validPaths[jumpIndex]
            || validPaths[nextIndex] == validPaths[pauseIndex]
            || validPaths[nextIndex] == validPaths[restartIndex])
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
            || validPaths[previousIndex] == validPaths[jumpIndex]
            || validPaths[previousIndex] == validPaths[pauseIndex]
            || validPaths[previousIndex] == validPaths[restartIndex])
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
        int slashIndex = path.LastIndexOf('/');
        if (slashIndex < 0 || slashIndex >= path.Length - 1)
        {
            return path; // Return the original path if no '/' found or it's the last character
        }

        string shortName = path.Substring(slashIndex + 1);

        switch (shortName)
        {
            case "leftButton":
                return "Left Click";
            case "rightButton":
                return "Right Click";
            case "middleButton":
                return "Mouse Middle";
            case "leftShift":
                return "Left Shift";
            case "leftCtrl":
                return "Left Control";
            case "leftAlt":
                return "Left Alt";
            case "rightShift":
                return "Right Shift";
            case "rightCtrl":
                return "Right Control";
            case "rightAlt":
                return "Right Alt";
        }

        if (shortName.Length == 1 && char.IsLetter(shortName[0]))
        {
            return shortName.ToUpper();
        }

        return char.ToUpper(shortName[0]) + shortName.Substring(1);
    }
}