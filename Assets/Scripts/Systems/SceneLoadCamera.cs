using UnityEngine;
using UnityEngine.UIElements;

public class SceneLoadCamera : MonoBehaviour
{
    public static SceneLoadCamera Instance { get; private set; }
    private Camera cam;
    private AudioListener listener;
    private VisualElement root;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }

        cam = GetComponent<Camera>();
        listener = GetComponent<AudioListener>();
        root = GetComponent<UIDocument>().rootVisualElement;

        Disable();
    }

    public void Enable()
    {
        cam.enabled = true;
        listener.enabled = true;
        root.style.display = DisplayStyle.Flex;
    }

    public void Disable()
    {
        cam.enabled = false;
        listener.enabled = false;
        root.style.display = DisplayStyle.None;
    }
}
