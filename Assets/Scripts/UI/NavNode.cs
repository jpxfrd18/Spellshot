using UnityEngine.UIElements;

public class NavNode
{
    public Button button;
    public NavNode up, down, left, right;

    public NavNode(Button b)
    {
        button = b;
        up = null;
        down = null;
        left = null;
        right = null;
    }
};
