using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseController : MonoBehaviour
{
    [SerializeField] private InputActionAsset InputActions;
    public bool isPaused { get; private set; } = false;
    private bool win = false;
    private bool loadingScene = false;
    private InputAction pauseAction;
    private InputAction playAction;
    private InputAction restartAction;
    private InputAction restartUIAction;
    private InputAction navigation;

    private Vector2 navigationAmt = Vector2.zero;
    private InputAction back;
    private NavNode current = null;
    private List<NavNode> currentList;

    private VisualElement pauseRoot;
    private Button mainMenuButton;
    private Button playButton;
    private Button settingsButton;
    private Button restartButton;
    private Button nextLevelButton;
    private Button noFocus;

    private VisualElement crosshairRoot;
    private VisualElement gameplayRoot;
    private VisualElement secretRoot;
    private VisualElement winRoot;
    private VisualElement iconRoot;
    private VisualElement keyboardRebindRoot;
    private VisualElement gamepadRebindRoot;

    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject gameplayScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject secretScreen;
    [SerializeField] private GameObject iconScreen;
    [SerializeField] private GameObject keyboardRebindScreen;
    [SerializeField] private GameObject gamepadRebindScreen;
    [SerializeField] private GameObject damageScreen;
    [SerializeField] private float damageOpacity = 0.5f;
    [SerializeField] private float damageFadeOut = 0.5f;
    [SerializeField] private float secretFadeIn = 1f;
    [SerializeField] private float secretHold = 1f;
    [SerializeField] private float secretFadeOut = 4f;
    private PlayerStats playerStatsController;
    private VictoryController victoryController;
    private bool gamepad;
    private bool damageActive = false;

    private bool pauseBuilt = false;
    private bool winBuilt = false;
    private bool settingsBuilt = false;
    private bool keyboardBuilt = false;
    private bool gamepadBuilt = false;
    private bool initial = true;
    private float initialDelay = 0.2f;
    private float navigationDelay = 0.1f;
    private float navigationTimer = 0f;

    private Button keyboardBackButton;
    private Button keyboardToGamepad;
    private Button gamepadToKeyboard;
    private Button gamepadBackButton;

    private VisualElement healthContainer;
    private VisualElement manaContainer;

    private VisualElement damageRoot;

    private VisualElement settingsRoot;
    private Button backButton;
    private Button keyboardRebindButton;
    private Button gamepadRebindButton;
    private Button leftRestart;
    private Button rightRestart;

    private Button sensitivityDownButton;
    private Button sensitivityUpButton;
    private Label sensitivityLabel;

    private Button volumeDownButton;
    private Button volumeUpButton;
    private Label volumeLabel;

    private Label bestTimeLabel;
    private Label currentTimeLabel;
    private Label gameplayTimeLabel;
    private Label objectiveLabel;

    private Dictionary<int, string> actionNames = new Dictionary<int, string>
    {
        {1, "Spell1"},
        {2, "Spell2"},
        {3, "Spell3"},
        {4, "Spell4"},
        {5, "Jump"},
        {6, "Pause"},
        {7, "Restart"}
    };

    public static PauseController Instance { get; private set; }

    private void Awake()
    {
        playerStatsController = PlayerStats.Instance;
        victoryController = VictoryController.Instance;
        Instance = this;
        win = false;

        gamepad = Gamepad.current != null;

        gameplayRoot = gameplayScreen.GetComponent<UIDocument>().rootVisualElement;
        crosshairRoot = crosshair.GetComponent<UIDocument>().rootVisualElement;
        pauseRoot = pauseScreen.GetComponent<UIDocument>().rootVisualElement;
        settingsRoot = settingsScreen.GetComponent<UIDocument>().rootVisualElement;
        secretRoot = secretScreen.GetComponent<UIDocument>().rootVisualElement;
        winRoot = winScreen.GetComponent<UIDocument>().rootVisualElement;
        iconRoot = iconScreen.GetComponent<UIDocument>().rootVisualElement;
        keyboardRebindRoot = keyboardRebindScreen.GetComponent<UIDocument>().rootVisualElement;
        gamepadRebindRoot = gamepadRebindScreen.GetComponent<UIDocument>().rootVisualElement;
        damageRoot = damageScreen.GetComponent<UIDocument>().rootVisualElement;
        noFocus = pauseRoot.Q<Button>("NoFocus");

        crosshairRoot.style.display = DisplayStyle.Flex;
        pauseRoot.style.display = DisplayStyle.None;
        settingsRoot.style.display = DisplayStyle.None;
        winRoot.style.display = DisplayStyle.None;
        secretRoot.style.display = DisplayStyle.Flex;
        iconRoot.style.display = DisplayStyle.Flex;
        keyboardRebindRoot.style.display = DisplayStyle.None;
        gamepadRebindRoot.style.display = DisplayStyle.None;
        damageRoot.style.display = DisplayStyle.Flex;
        secretRoot.style.opacity = 0;
        damageRoot.style.opacity = 0;
        playerStatsController.OnHealthChanged += OnHealthChanged;
        playerStatsController.OnManaChanged += OnManaChanged;
        victoryController.OnUpdateObjective += OnUpdateObjective;

        pauseAction = InputSystem.actions.FindAction("Pause");
        playAction = InputSystem.actions.FindAction("Play");
        restartAction = InputSystem.actions.FindAction("Restart");
        restartUIAction = InputSystem.actions.FindAction("RestartUI");
        navigation = InputSystem.actions.FindAction("Navigate");
        back = InputSystem.actions.FindAction("Cancel");

        InputActions.FindActionMap("Player").Enable();
        InputActions.FindActionMap("UI").Disable();
        InputActions.FindActionMap("UI Nav").Enable();

        pauseRoot.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            if (!pauseBuilt)
            {
                MenuNavigation.Instance.SetPause(MainMenu.SetupNavigation(pauseRoot));
                pauseBuilt = true;

                currentList = MenuNavigation.Instance.pauseNodes;

                if (Gamepad.current != null && current != null)
                {
                    current = MainMenu.FindTopLeft(currentList);
                    current.button.Focus();
                }
            }
        });

        winRoot.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            if (!winBuilt)
            {
                MenuNavigation.Instance.SetWin(MainMenu.SetupNavigation(winRoot));
                winBuilt = true;

                currentList = MenuNavigation.Instance.winNodes;

                if (Gamepad.current != null && current != null)
                {
                    current = MainMenu.FindTopLeft(currentList);
                    current.button.Focus();
                }
            }
        });

        settingsRoot.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            if (!settingsBuilt)
            {
                MenuNavigation.Instance.SetSettings(MainMenu.SetupNavigation(settingsRoot));
                settingsBuilt = true;

                currentList = MenuNavigation.Instance.settingsNodes;

                if (Gamepad.current != null && current != null)
                {
                    current = MainMenu.FindTopLeft(currentList);
                    current.button.Focus();
                }
            }
        });

        keyboardRebindRoot.RegisterCallback<GeometryChangedEvent>(evt =>
         {
             if (!keyboardBuilt)
             {
                 MenuNavigation.Instance.SetKeyboard(MainMenu.SetupNavigation(keyboardRebindRoot));
                 keyboardBuilt = true;

                 currentList = MenuNavigation.Instance.keyboardNodes;

                 if (Gamepad.current != null && current != null)
                 {
                     current = MainMenu.FindTopLeft(currentList);
                     current.button.Focus();
                 }
             }
         });

        gamepadRebindRoot.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            if (!gamepadBuilt)
            {
                MenuNavigation.Instance.SetController(MainMenu.SetupNavigation(gamepadRebindRoot));
                gamepadBuilt = true;

                currentList = MenuNavigation.Instance.controllerNodes;

                if (Gamepad.current != null && current != null)
                {
                    current = MainMenu.FindTopLeft(currentList);
                    current.button.Focus();
                }
            }
        });

        Time.timeScale = 1f;

        SetupLabels();
    }

    private void OnEnable()
    {
        SetupButtons();

        restartAction.performed += ctx => OnRestart();
        restartUIAction.performed += ctx => OnRestart();
        back.performed += ctx => OnBackAction();
    }

    private void OnDisable()
    {
        restartAction.performed -= ctx => OnRestart();
        restartUIAction.performed -= ctx => OnRestart();
        back.performed -= ctx => OnBackAction();
    }

    private void Update()
    {
        if (UnityEngine.Cursor.visible && !isPaused)
        {
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }

        if (!UnityEngine.Cursor.visible && isPaused)
        {
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }

        if (pauseAction.WasPressedThisFrame())
        {
            OnPause();
        }
        else if (playAction.WasPressedThisFrame() && winRoot.style.display == DisplayStyle.None)
        {
            OnPlay();
        }
        else if (playAction.WasPressedThisFrame())
        {
            OnRestart();
        }


        if (gamepad && Gamepad.current == null)
        {
            gamepad = false;
            for (int i = 1; i <= 4; i++)
            {
                int index = i;
                Label bindingLabel = iconRoot.Q<Label>(index.ToString() + "l");
                bindingLabel.text = playerStatsController.persistant.bindingNames[index - 1];
            }
        }
        else if (!gamepad && Gamepad.current != null)
        {
            gamepad = true;
            for (int i = 1; i <= 4; i++)
            {
                int index = i;
                Label bindingLabel = iconRoot.Q<Label>(index.ToString() + "l");
                bindingLabel.text = playerStatsController.persistant.controllerBindingNames[index - 1];
            }
        }

        if (isPaused)
        {
            navigationAmt = navigation.ReadValue<Vector2>();
            if (navigationAmt != null && navigationAmt.sqrMagnitude > 0.01)
            {
                Navigate(navigationAmt);
            }
            else if (navigationAmt != null)
            {
                initial = true;
            }

            if (navigationTimer > 0f)
            {
                navigationTimer -= Time.unscaledDeltaTime;
            }
        }

        if (current == null)
        {
            noFocus.Focus();
        }
        else
        {
            current.button.Focus();
        }

        //Current timer
        int minutes = (int)playerStatsController.seconds / 60;
        float seconds = playerStatsController.seconds - (minutes * 60);
        seconds = Mathf.Floor(seconds * 100f) / 100f;
        currentTimeLabel.text = "Current Time: " + minutes + ":" + seconds.ToString("00.00");
        gameplayTimeLabel.text = minutes + ":" + seconds.ToString("00.00");
    }

    private void OnRestart()
    {
        if (isPaused)
        {
            OnPlay();
        }

        RespawnManager.Instance.Restart();
    }

    public void OnWin()
    {
        gameplayRoot.style.display = DisplayStyle.None;
        crosshairRoot.style.display = DisplayStyle.None;
        pauseRoot.style.display = DisplayStyle.None;
        settingsRoot.style.display = DisplayStyle.None;
        secretRoot.style.display = DisplayStyle.None;
        iconRoot.style.display = DisplayStyle.None;
        winRoot.style.display = DisplayStyle.Flex;
        keyboardRebindRoot.style.display = DisplayStyle.None;
        gamepadRebindRoot.style.display = DisplayStyle.None;
        damageRoot.style.display = DisplayStyle.None;
        InputActions.FindActionMap("Player").Disable();
        InputActions.FindActionMap("UI").Enable();
        InputActions.FindActionMap("UI Nav").Enable();
        isPaused = true;
        Time.timeScale = 0;
        currentList = MenuNavigation.Instance.winNodes;

        if (current != null && MenuNavigation.Instance.winNodes.Count > 0)
        {
            current = MainMenu.FindTopLeft(MenuNavigation.Instance.winNodes);
            current.button.Focus();
        }

        SetupWinScreen();
    }

    private void OnBackAction()
    {
        if (pauseRoot.style.display == DisplayStyle.Flex)
        {
            StartCoroutine(TryPlay());
        }
        else if (settingsRoot.style.display == DisplayStyle.Flex)
        {
            if (!win)
                OnBack();
            else
                OnBackWin();
        }
        else if (keyboardRebindRoot.style.display == DisplayStyle.Flex ||
                gamepadRebindRoot.style.display == DisplayStyle.Flex)
        {
            OnRebindBack();
        }
        else if (winRoot.style.display == DisplayStyle.Flex)
        {
            OnNextLevel();
        }
    }

    private void OnPause()
    {
        gameplayRoot.style.display = DisplayStyle.None;
        crosshairRoot.style.display = DisplayStyle.None;
        pauseRoot.style.display = DisplayStyle.Flex;
        settingsRoot.style.display = DisplayStyle.None;
        secretRoot.style.display = DisplayStyle.None;
        winRoot.style.display = DisplayStyle.None;
        iconRoot.style.display = DisplayStyle.None;
        keyboardRebindRoot.style.display = DisplayStyle.None;
        gamepadRebindRoot.style.display = DisplayStyle.None;
        damageRoot.style.display = DisplayStyle.None;
        InputActions.FindActionMap("Player").Disable();
        InputActions.FindActionMap("UI").Enable();
        InputActions.FindActionMap("UI Nav").Enable();
        isPaused = true;
        Time.timeScale = 0;
        currentList = MenuNavigation.Instance.pauseNodes;

        if (current != null && MenuNavigation.Instance.pauseNodes.Count > 0)
        {
            current = MainMenu.FindTopLeft(MenuNavigation.Instance.pauseNodes);
            current.button.Focus();
        }
    }

    private void OnPlay()
    {
        gameplayRoot.style.display = DisplayStyle.Flex;
        crosshairRoot.style.display = DisplayStyle.Flex;
        pauseRoot.style.display = DisplayStyle.None;
        settingsRoot.style.display = DisplayStyle.None;
        secretRoot.style.display = DisplayStyle.Flex;
        winRoot.style.display = DisplayStyle.None;
        iconRoot.style.display = DisplayStyle.Flex;
        keyboardRebindRoot.style.display = DisplayStyle.None;
        gamepadRebindRoot.style.display = DisplayStyle.None;
        damageRoot.style.display = DisplayStyle.Flex;
        InputActions.FindActionMap("Player").Enable();
        InputActions.FindActionMap("UI").Disable();
        InputActions.FindActionMap("UI Nav").Disable();
        isPaused = false;
        Time.timeScale = 1f;
    }

    private void OnSettings()
    {
        pauseRoot.style.display = DisplayStyle.None;
        winRoot.style.display = DisplayStyle.None;
        settingsRoot.style.display = DisplayStyle.Flex;
        currentList = MenuNavigation.Instance.settingsNodes;

        if (current != null && MenuNavigation.Instance.settingsNodes.Count > 0)
        {
            current = MainMenu.FindTopLeft(MenuNavigation.Instance.settingsNodes);
            current.button.Focus();
        }
    }

    private void OnBack()
    {
        pauseRoot.style.display = DisplayStyle.Flex;
        settingsRoot.style.display = DisplayStyle.None;
        currentList = MenuNavigation.Instance.pauseNodes;

        if (current != null && MenuNavigation.Instance.pauseNodes.Count > 0)
        {
            current = MainMenu.FindTopLeft(MenuNavigation.Instance.pauseNodes);
            current.button.Focus();
        }
    }

    private void OnRebindBack()
    {
        settingsRoot.style.display = DisplayStyle.Flex;
        keyboardRebindRoot.style.display = DisplayStyle.None;
        gamepadRebindRoot.style.display = DisplayStyle.None;
        currentList = MenuNavigation.Instance.settingsNodes;

        if (current != null && MenuNavigation.Instance.settingsNodes.Count > 0)
        {
            current = MainMenu.FindTopLeft(MenuNavigation.Instance.settingsNodes);
            current.button.Focus();
        }
    }

    private void OnBackWin()
    {
        winRoot.style.display = DisplayStyle.Flex;
        settingsRoot.style.display = DisplayStyle.None;
        currentList = MenuNavigation.Instance.winNodes;

        if (current != null && MenuNavigation.Instance.winNodes.Count > 0)
        {
            current = MainMenu.FindTopLeft(MenuNavigation.Instance.winNodes);
            current.button.Focus();
        }
    }

    private void OnKeyboardRebind()
    {
        settingsRoot.style.display = DisplayStyle.None;
        gamepadRebindRoot.style.display = DisplayStyle.None;
        keyboardRebindRoot.style.display = DisplayStyle.Flex;
        currentList = MenuNavigation.Instance.keyboardNodes;

        if (current != null && MenuNavigation.Instance.keyboardNodes.Count > 0)
        {
            current = MainMenu.FindTopLeft(MenuNavigation.Instance.keyboardNodes);
            current.button.Focus();
        }
    }

    private void OnGamepadRebind()
    {
        settingsRoot.style.display = DisplayStyle.None;
        keyboardRebindRoot.style.display = DisplayStyle.None;
        gamepadRebindRoot.style.display = DisplayStyle.Flex;
        currentList = MenuNavigation.Instance.controllerNodes;

        if (current != null && MenuNavigation.Instance.controllerNodes.Count > 0)
        {
            current = MainMenu.FindTopLeft(MenuNavigation.Instance.controllerNodes);
            current.button.Focus();
        }
    }

    private void SetupButtons()
    {
        nextLevelButton = winRoot.Q<Button>("NextLevel");
        nextLevelButton.clicked += OnNextLevel;

        mainMenuButton = pauseRoot.Q<Button>("Menu");
        mainMenuButton.clicked += OnMainMenu;

        playButton = pauseRoot.Q<Button>("Play");
        playButton.clicked += OnPlay;

        restartButton = pauseRoot.Q<Button>("Restart");
        restartButton.clicked += OnRestart;

        settingsButton = pauseRoot.Q<Button>("Settings");
        settingsButton.clicked += OnSettings;

        backButton = settingsRoot.Q<Button>("BackButton");
        backButton.clicked += OnBack;

        keyboardRebindButton = settingsRoot.Q<Button>("Keyboard");
        keyboardRebindButton.clicked += OnKeyboardRebind;

        keyboardBackButton = keyboardRebindRoot.Q<Button>("Back");
        keyboardBackButton.clicked += OnRebindBack;

        keyboardToGamepad = keyboardRebindRoot.Q<Button>("Controller");
        keyboardToGamepad.clicked += OnGamepadRebind;

        gamepadToKeyboard = gamepadRebindRoot.Q<Button>("Keyboard");
        gamepadToKeyboard.clicked += OnKeyboardRebind;

        gamepadRebindButton = settingsRoot.Q<Button>("Controller");
        gamepadRebindButton.clicked += OnGamepadRebind;

        gamepadBackButton = gamepadRebindRoot.Q<Button>("Back");
        gamepadBackButton.clicked += OnRebindBack;

        sensitivityDownButton = settingsRoot.Q<Button>("SensitivityDown");
        sensitivityDownButton.clicked += OnSensitivityDown;

        sensitivityUpButton = settingsRoot.Q<Button>("SensitivityUp");
        sensitivityUpButton.clicked += OnSensitivityUp;

        volumeDownButton = settingsRoot.Q<Button>("VolumeDown");
        volumeDownButton.clicked += OnVolumeDown;

        volumeUpButton = settingsRoot.Q<Button>("VolumeUp");
        volumeUpButton.clicked += OnVolumeUp;

        for (int i = 1; i <= 7; i++)
        {
            int index = i;
            Button leftButton = keyboardRebindRoot.Q<Button>(index.ToString() + "l");
            Button rightButton = keyboardRebindRoot.Q<Button>(index.ToString() + "r");
            Label label = keyboardRebindRoot.Q<Label>(index.ToString());
            label.text = playerStatsController.persistant.bindingNames[index - 1];

            rightButton.clicked += () =>
            {
                playerStatsController.persistant.UpdateBindingName(index - 1, KeyboardRebinder.Instance.Increment(InputActions, actionNames[index]));
                label.text = playerStatsController.persistant.bindingNames[index - 1];

                if (index < 4)
                {
                    Label bindingLabel = iconRoot.Q<Label>(index.ToString() + "l");
                    bindingLabel.text = playerStatsController.persistant.bindingNames[index - 1];
                }
            };

            leftButton.clicked += () =>
            {
                playerStatsController.persistant.UpdateBindingName(index - 1, KeyboardRebinder.Instance.Decrement(InputActions, actionNames[index]));
                label.text = playerStatsController.persistant.bindingNames[index - 1];

                if (index < 4)
                {
                    Label bindingLabel = iconRoot.Q<Label>(index.ToString() + "l");
                    bindingLabel.text = playerStatsController.persistant.bindingNames[index - 1];
                }
            };
        }

        for (int i = 1; i <= 5; i++)
        {
            int index = i;
            Button leftButton = gamepadRebindRoot.Q<Button>(index.ToString() + "l");
            Button rightButton = gamepadRebindRoot.Q<Button>(index.ToString() + "r");
            Label label = gamepadRebindRoot.Q<Label>(index.ToString());

            if (Gamepad.current != null)
            {
                label.text = playerStatsController.persistant.controllerBindingNames[index - 1];
            }

            rightButton.clicked += () =>
            {
                playerStatsController.persistant.UpdateControllerBindingName(index - 1, GamepadRebinder.Instance.Increment(InputActions, actionNames[index]));
                label.text = playerStatsController.persistant.controllerBindingNames[index - 1];

            };

            leftButton.clicked += () =>
            {
                playerStatsController.persistant.UpdateControllerBindingName(index - 1, GamepadRebinder.Instance.Decrement(InputActions, actionNames[index]));
                label.text = playerStatsController.persistant.controllerBindingNames[index - 1];
            };
        }

        rightRestart = gamepadRebindRoot.Q<Button>("6r");
        rightRestart.clicked += ToggleRestartAction;

        leftRestart = gamepadRebindRoot.Q<Button>("6l");
        leftRestart.clicked += ToggleRestartAction;
    }

    private void SetupWinScreen()
    {
        mainMenuButton.clicked -= OnMainMenu;
        mainMenuButton = winRoot.Q<Button>("Menu");
        mainMenuButton.clicked += OnMainMenu;

        restartButton.clicked -= OnRestart;
        restartButton = winRoot.Q<Button>("Restart");
        restartButton.clicked += OnRestart;

        settingsButton.clicked -= OnSettings;
        settingsButton = winRoot.Q<Button>("Settings");
        settingsButton.clicked += OnSettings;

        backButton.clicked -= OnBack;
        backButton.clicked += OnBackWin;

        win = true;

        int bestMinutes = (int)playerStatsController.persistant.bestTime[playerStatsController.level - 1] / 60;
        float bestSeconds = playerStatsController.persistant.bestTime[playerStatsController.level - 1] - (bestMinutes * 60);
        bestSeconds = Mathf.Floor(bestSeconds * 100f) / 100f;
        bestTimeLabel.text = "Best Time: " + bestMinutes + ":" + bestSeconds.ToString("00.00");
        playerStatsController.persistant.LevelComplete(playerStatsController.level);
    }

    private void SetupLabels()
    {
        sensitivityLabel = settingsRoot.Q<Label>("SensitivityDisplay");
        volumeLabel = settingsRoot.Q<Label>("VolumeDisplay");

        healthContainer = gameplayRoot.Q<VisualElement>("HealthContainer");
        manaContainer = gameplayRoot.Q<VisualElement>("ManaContainer");

        bestTimeLabel = winRoot.Q<Label>("BestTimeDisplay");
        currentTimeLabel = winRoot.Q<Label>("TimeDisplay");
        gameplayTimeLabel = gameplayRoot.Q<Label>("Timer");
        objectiveLabel = gameplayRoot.Q<Label>("ObjectiveLabel");

        for (int i = 1; i <= 4; i++)
        {
            int index = i;
            Label iconLabel = iconRoot.Q<Label>(index.ToString());
            Label bindingLabel = iconRoot.Q<Label>(index.ToString() + "l");

            if (Gamepad.current != null)
            {
                bindingLabel.text = playerStatsController.persistant.controllerBindingNames[index - 1];
            }
            else
            {
                bindingLabel.text = playerStatsController.persistant.bindingNames[index - 1];
            }

            SpellType spellType = playerStatsController.persistant.spells[index - 1];

            switch (spellType)
            {
                case SpellType.None:
                    iconLabel.text = "";
                    break;
                case SpellType.MagicMissile:
                    iconLabel.text = "MM";
                    break;
                case SpellType.Fireball:
                    iconLabel.text = "FB";
                    break;
                case SpellType.ForceLance:
                    iconLabel.text = "FL";
                    break;
                case SpellType.LightningStrike:
                    iconLabel.text = "LS";
                    break;
                case SpellType.ManaDrain:
                    iconLabel.text = "MD";
                    break;
                case SpellType.Launch:
                    iconLabel.text = "LA";
                    break;
                case SpellType.Gust:
                    iconLabel.text = "GU";
                    break;
                case SpellType.Tether:
                    iconLabel.text = "TE";
                    break;
                case SpellType.Draw:
                    iconLabel.text = "DR";
                    break;
                case SpellType.Portal:
                    iconLabel.text = "PO";
                    break;
            }
        }

        sensitivityLabel.text = (playerStatsController.persistant.sensitivity * 100f).ToString("0");
        volumeLabel.text = (playerStatsController.persistant.musicVolume * 100f).ToString("0");
        healthContainer.style.paddingRight = 0;
        manaContainer.style.paddingRight = 0;

        healthContainer.style.width = playerStatsController.persistant.maxHealth * 4;
        manaContainer.style.width = playerStatsController.persistant.maxMana * 4;

        objectiveLabel.text = victoryController.ToString();
    }

    private void Navigate(Vector2 d)
    {
        if (navigationTimer > 0f || d == Vector2.zero)
        {
            return;
        }

        if (current == null)
        {
            current = MainMenu.FindTopLeft(currentList);
            current.button.Focus();
            if (initial)
            {
                navigationTimer = initialDelay;
                initial = false;
            }
            else
            {
                navigationTimer = navigationDelay;
            }
            return;
        }

        Vector2 direction;
        if (Mathf.Abs(d.x) >= Mathf.Abs(d.y))
        {
            direction = new Vector2(d.x, 0).normalized;
        }
        else
        {
            direction = new Vector2(0, d.y).normalized;
        }

        if (Vector2.Dot(direction, Vector2.up) > 0.99f)
        {
            if (current.up != null)
            {
                current = current.up;
            }
        }
        else if (Vector2.Dot(direction, Vector2.down) > 0.99f)
        {
            if (current.down != null)
            {
                current = current.down;
            }
        }
        else if (Vector2.Dot(direction, Vector2.right) > 0.99f)
        {
            if (current.right != null)
            {
                current = current.right;
            }
        }
        else if (Vector2.Dot(direction, Vector2.left) > 0.99f)
        {
            if (current.left != null)
            {
                current = current.left;
            }
        }

        current.button.Focus();
        if (initial)
        {
            navigationTimer = initialDelay;
            initial = false;
        }
        else
        {
            navigationTimer = navigationDelay;
        }
    }

    private void OnMainMenu()
    {
        if (loadingScene)
        {
            return;
        }

        loadingScene = true;
        SceneLoader.Instance.LoadMainMenu();
    }

    private void OnNextLevel()
    {
        if (loadingScene)
        {
            return;
        }

        loadingScene = true;
        SceneLoader.Instance.NextLevel();
    }

    private void OnHealthChanged(int newHealth)
    {
        healthContainer.style.paddingRight = 4 * (playerStatsController.persistant.maxHealth - newHealth);

        if (newHealth > 0)
        {
            StartCoroutine(DamageRoutine());
        }
    }

    private void OnManaChanged(int newMana)
    {
        manaContainer.style.paddingRight = 4 * (playerStatsController.persistant.maxMana - newMana);
    }

    private void OnUpdateObjective(string newObjective)
    {
        objectiveLabel.text = newObjective;
    }

    private void OnSensitivityUp()
    {
        playerStatsController.persistant.IncreaseSensitivity(0.01f);
        sensitivityLabel.text = (playerStatsController.persistant.sensitivity * 100f).ToString("0");
    }

    private void OnSensitivityDown()
    {
        playerStatsController.persistant.DecreaseSensitivity(0.01f);
        sensitivityLabel.text = (playerStatsController.persistant.sensitivity * 100f).ToString("0");
    }

    private void OnVolumeUp()
    {
        playerStatsController.persistant.IncreaseMusicVolume(0.1f);
        volumeLabel.text = (playerStatsController.persistant.musicVolume * 100f).ToString("0");
    }

    private void OnVolumeDown()
    {
        playerStatsController.persistant.DecreaseMusicVolume(0.1f);
        volumeLabel.text = (playerStatsController.persistant.musicVolume * 100f).ToString("0");
    }

    private void ToggleRestartAction()
    {
        Label lbl = gamepadRebindRoot.Q<Label>("6");

        if (playerStatsController.persistant.controllerBindingNames[6].Equals("None"))
        {
            playerStatsController.persistant.UpdateControllerBindingName(6, "D-Pad (Any)");
            ActionRebinder.Rebind(InputActions, "Restart", "", "<Gamepad>/dpad/up");
            ActionRebinder.Rebind(InputActions, "Restart", "", "<Gamepad>/dpad/down");
            ActionRebinder.Rebind(InputActions, "Restart", "", "<Gamepad>/dpad/left");
            ActionRebinder.Rebind(InputActions, "Restart", "", "<Gamepad>/dpad/right");
            lbl.text = "D-Pad (Any)";
        }
        else
        {
            playerStatsController.persistant.UpdateControllerBindingName(6, "None");
            ActionRebinder.Rebind(InputActions, "Restart", "<Gamepad>/dpad/up", "");
            ActionRebinder.Rebind(InputActions, "Restart", "<Gamepad>/dpad/down", "");
            ActionRebinder.Rebind(InputActions, "Restart", "<Gamepad>/dpad/left", "");
            ActionRebinder.Rebind(InputActions, "Restart", "<Gamepad>/dpad/right", "");
            lbl.text = "None";
        }
    }

    private void OnDestroy()
    {
        playerStatsController.OnHealthChanged -= OnHealthChanged;
        playerStatsController.OnManaChanged -= OnManaChanged;
        victoryController.OnUpdateObjective -= OnUpdateObjective;
        playButton.clicked -= OnPlay;
        settingsButton.clicked -= OnSettings;
        backButton.clicked -= OnBack;
        backButton.clicked -= OnBackWin;
        restartButton.clicked -= OnRestart;
        sensitivityDownButton.clicked -= OnSensitivityDown;
        sensitivityUpButton.clicked -= OnSensitivityUp;
        volumeDownButton.clicked -= OnVolumeDown;
        volumeUpButton.clicked -= OnVolumeUp;
    }

    public void ShowSecret()
    {
        StartCoroutine(SecretRoutine(secretFadeIn, secretHold, secretFadeOut));
    }

    private IEnumerator TryPlay()
    {
        float timer = 0f;

        while (timer < 0.2)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        if (pauseRoot.style.display == DisplayStyle.Flex)
        {
            OnPlay();
        }
    }

    private IEnumerator SecretRoutine(float fadeIn, float hold, float fadeOut)
    {
        float timer = 0f;
        objectiveLabel.style.display = DisplayStyle.None;

        while (timer < fadeIn)
        {
            timer += Time.deltaTime;
            secretRoot.style.opacity = timer / fadeIn;
            yield return null;
        }

        timer = 0f;
        secretRoot.style.opacity = 1f;

        while (timer < hold)
        {
            timer += Time.deltaTime;
            yield return null;
        }


        timer = fadeOut;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            secretRoot.style.opacity = timer / fadeOut;
            yield return null;
        }

        objectiveLabel.style.display = DisplayStyle.Flex;
    }

    private IEnumerator DamageRoutine()
    {
        if (damageActive)
        {
            yield break;
        }

        float timer = 0f;
        damageActive = true;
        damageRoot.style.opacity = damageOpacity;
        while (timer < damageFadeOut)
        {
            timer += Time.deltaTime;
            damageRoot.style.opacity = Mathf.Lerp(damageOpacity, 0f, timer / damageFadeOut);
            yield return null;
        }

        damageRoot.style.opacity = 0f;
        damageActive = false;
    }
}