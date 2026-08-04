using UnityEngine;
using UnityEngine.InputSystem;

public class ActionRebinder : MonoBehaviour
{
    public static void Rebind(InputActionAsset asset, string actionName, string oldBinding, string newBinding)
    {
        // Get the action
        var action = asset.FindAction(actionName);
        if (action == null)
        {
            Debug.LogError(actionName + " action not found.");
            return;
        }

        // Find the binding that currently uses the old binding
        int bindingIndex = -1;
        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (action.bindings[i].effectivePath == oldBinding)
            {
                bindingIndex = i;
                break;
            }
        }

        if (bindingIndex == -1)
        {
            Debug.LogError(actionName + " does not have a " + oldBinding + " binding.");
            return;
        }

        // Apply override
        action.ApplyBindingOverride(bindingIndex, newBinding);
    }
}