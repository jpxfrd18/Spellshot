using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [field: SerializeField] private InputActionAsset InputActions;
    private InputAction navigation;
    private Vector2 navigationAmt = Vector2.zero;
    private InputAction back;
    private NavNode current = null;
    private List<NavNode> currentList;
    private PersistantPlayerStats playerStats;

    [SerializeField] private GameObject mainScreen;
    private VisualElement mainRoot;
    private Button noFocus;
    private Button levelSelectButton;
    private Button settingsButton;
    private Button controlsButton;
    private Button quitButton;
    private bool mainBuilt = false;
    private float initialDelay = 0.2f;
    private bool initial = true;
    private float navigationDelay = 0.1f;
    private float navigationTimer = 0f;


    [SerializeField] private GameObject levelSelectScreen;
    private VisualElement levelSelectRoot;
    private Button levelSelectBackButton;
    private int level = -1;
    private bool levelSelectBuilt = false;

    [SerializeField] private GameObject levelScreen;
    private VisualElement levelRoot;
    private VisualElement levelBackground;
    private VisualElement grid;
    private VisualElement popup;
    private Button popupBackButton;
    private Button popupStartButton;
    private Button levelBackButton;
    private Button startButton;
    private Label title;
    private Label subtitle;
    private Label description;
    private bool levelBuilt = false;

    [SerializeField] private GameObject settingsScreen;
    private VisualElement settingsRoot;
    private VisualElement settingsBackground;
    private bool settingsBuilt = false;

    private Button sensitivityDownButton;
    private Button sensitivityUpButton;
    private Label sensitivityLabel;

    private Button volumeDownButton;
    private Button volumeUpButton;
    private Label volumeLabel;

    private Button settingBackButton;
    private Button settingsKeyboardButton;
    private Button settingsControllerButton;

    [SerializeField] private GameObject controlsScreen;
    private VisualElement controlsRoot;
    private Button controlsKeyboardButton;
    private Button controlBackButton;
    private Button controlsControllerButton;
    private Label movement;
    private Label look;
    private bool controlsBuilt = false;

    [SerializeField] private GameObject keyboardRebindScreen;
    private VisualElement keyboardRebindRoot;
    private VisualElement keyboardRebindBackground;
    private Button keyboardRebindBackButton;
    private Button keyboardToControllerButton;
    private bool keyboardBuilt = false;

    [SerializeField] private GameObject controllerRebindScreen;
    private VisualElement controllerRebindRoot;
    private VisualElement controllerRebindBackground;
    private Button controllerRebindBackButton;
    private Button controllerToKeyboardButton;
    private Button rightRestart;
    private Button leftRestart;
    private bool controllerBuilt = false;

    private bool gamepad;

    private Dictionary<int, LevelInfo> levelInfo = new Dictionary<int, LevelInfo>
    {
        {1, new LevelInfo("Peasant Uprising", "In a small town deep into the mountains, the peasants have begun to actively rebel against the authority of the crown. The king requests that you crush the rebellion before it spreads. ",
        "Objective: Kill 25 peasant militia\nOR Destroy their munitions")},
        {2, new LevelInfo("Occupied Ruins", "Before the King's army could reach the town that you handled, a nearby warlord snatched up the high ground and started digging in. The king needs you to weaken the resistance.",
        "Objective: Kill 35 enemy soldiers\nOR Kill the warlord")},
        {3, new LevelInfo("Ogre Extermination", "A camp of ogres have been causing mayhem and assaulting travelers. The king can't tax people who don't arrive.",
        "Objective: Kill 5 Ogres")},
        {4, new LevelInfo("Castle Invasion", "The king is running low on gold and asks you to requisition some from his neighbor",
        "Objective: Reach the treasury and recover the gold")}
    };

    private Dictionary<int, SpellInfo> spellInfo = new Dictionary<int, SpellInfo>
    {
        {1, new SpellInfo("Magic Missile", "Cost: 4 mana\nDamage: 5 damage\nFire a small dart of magical energy that homes on the nearest target, resulting in minor damage.\nThis is the first spell an initiate learns at The Academy.")},
        {2, new SpellInfo("Fireball", "Cost: 20 mana\nDamage: 30 damage\nSummon a large ball of fire that explodes on impact.\nKnowledge of this spell is restricted to senior mages because the caster can get caught in the blast.")},
        {3, new SpellInfo("Force Lance", "Cost: 10 mana\nDamage: 15 damage\nFire a spear of pure magical energy, piercing through all matter.\nIn the Academy, wizards are taught that matter includes everything from stone to flesh.")},
        {4, new SpellInfo("Lightning Strike", "Cost: 20 mana\nDamage: 50 damage\nCall down a bolt of lightning from the heavens to instantly smite your enemies.\nFew are known to have survived a direct hit.")},
        {5, new SpellInfo("Mana Drain", "Cost: 0 mana\nDrain the mana from a nearby creature, restoring your own.\nThis spell is unique in that is uses the target's mana to power the spell.")},
        {6, new SpellInfo("Launch", "Cost: 20 mana\nLaunches the caster at incredible speeds in any direction they choose.\nInterestingly, instead of applying force, this spell appears to directly overwrite your velocity.\nFurther study is required to determine the exact speed.")},
        {7, new SpellInfo("Gust", "Cost: 8 mana\nSummon a small burst of air, pushing the caster up into the sky.\n")},
        {8, new SpellInfo("Tether", "Cost: 4 mana per second\nBind yourself to the ground or a nearby inanimate object, pulling yourself towards it.\nFew realize that Tether can also be used to pull mana orbs towards the caster.")},
        {9, new SpellInfo("Shield", "Cost: 10 mana per second\nCreate a barrier of magical energy around the caster, resulting in complete invincibility.\nSenior mages understand that overuse of this spell quickly drains mana needed to fight")},
        {10, new SpellInfo("Portal", "Cost: 10 mana\n Create a portal at the caster's location. Recasting the spell will teleport the caster to the portal.")}
    };

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

    private Dictionary<int, Description> controlDescriptions = new Dictionary<int, Description>
    {
        {1, new Description("Spell 1: Press ", " to cast.")},
        {2, new Description("Spell 2: Press ", " to cast.")},
        {3, new Description("Spell 3: Press ", " to cast.")},
        {4, new Description("Spell 4: Press ", " to cast.")},
        {5, new Description("Jump: Press ", " to jump. Hold for repeated jumps.")},
        {6, new Description("Pause: Press ", " to pause or play.")},
        {7, new Description("Restart: Press and hold ", " to restart the level.")}
    };

    private void Awake()
    {
        mainRoot = mainScreen.GetComponent<UIDocument>().rootVisualElement;
        levelSelectRoot = levelSelectScreen.GetComponent<UIDocument>().rootVisualElement;
        levelRoot = levelScreen.GetComponent<UIDocument>().rootVisualElement;
        settingsRoot = settingsScreen.GetComponent<UIDocument>().rootVisualElement;
        controlsRoot = controlsScreen.GetComponent<UIDocument>().rootVisualElement;
        keyboardRebindRoot = keyboardRebindScreen.GetComponent<UIDocument>().rootVisualElement;
        controllerRebindRoot = controllerRebindScreen.GetComponent<UIDocument>().rootVisualElement;

        noFocus = mainRoot.Q<Button>("NoFocus");
        popup = levelRoot.Q<VisualElement>("Popup");
        popup.style.display = DisplayStyle.None;

        levelBackground = levelRoot.Q<VisualElement>("Background");

        grid = levelRoot.Q<VisualElement>("Grid");
        grid.RegisterCallback<PointerLeaveEvent>(evt =>
        {
            LevelDescription();
        });

        settingsBackground = settingsRoot.Q<VisualElement>("Background");
        settingsBackground.style.backgroundColor = new Color(0, 0, 0, 1f);

        keyboardRebindBackground = keyboardRebindRoot.Q<VisualElement>("Background");
        keyboardRebindBackground.style.backgroundColor = new Color(0, 0, 0, 1f);

        controllerRebindBackground = controllerRebindRoot.Q<VisualElement>("Background");
        controllerRebindBackground.style.backgroundColor = new Color(0, 0, 0, 1f);

        InputActions.FindActionMap("Player").Disable();
        InputActions.FindActionMap("UI").Disable();
        InputActions.FindActionMap("UI Nav").Enable();

        navigation = InputSystem.actions.FindAction("Navigate");
        back = InputSystem.actions.FindAction("Cancel");

        gamepad = GamepadActive();

        playerStats = PersistantPlayerStats.Instance;

        SetupLabels();

        mainRoot.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            if (!mainBuilt)
            {
                MenuNavigation.Instance.SetMain(SetupNavigation(mainRoot));
                mainBuilt = true;
                currentList = MenuNavigation.Instance.mainNodes;

                if (GamepadActive() && current != null)
                {
                    current = FindTopLeft(currentList);
                    current.button.Focus();
                }
            }
        });

        levelSelectRoot.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            if (!levelSelectBuilt)
            {
                MenuNavigation.Instance.SetLevelSelect(SetupNavigation(levelSelectRoot));
                levelSelectBuilt = true;

                currentList = MenuNavigation.Instance.levelSelectNodes;

                if (GamepadActive() && current != null)
                {
                    current = FindTopLeft(currentList);
                    current.button.Focus();
                }
            }
        });

        levelBackground.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            if (!levelBuilt)
            {
                MenuNavigation.Instance.SetLevel(SetupNavigation(levelBackground));
                levelBuilt = true;
                currentList = MenuNavigation.Instance.levelNodes;

                if (GamepadActive() && current != null)
                {
                    current = FindTopLeft(currentList);
                    current.button.Focus();
                }
            }
        });

        settingsRoot.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            if (!settingsBuilt)
            {
                MenuNavigation.Instance.SetSettings(SetupNavigation(settingsRoot));
                settingsBuilt = true;

                currentList = MenuNavigation.Instance.settingsNodes;

                if (GamepadActive() && current != null)
                {
                    current = FindTopLeft(currentList);
                    current.button.Focus();
                }
            }
        });

        controlsRoot.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            if (!controlsBuilt)
            {
                MenuNavigation.Instance.SetControls(SetupNavigation(controlsRoot));
                controlsBuilt = true;

                currentList = MenuNavigation.Instance.controlsNodes;

                if (GamepadActive() && current != null)
                {
                    current = FindTopLeft(currentList);
                    current.button.Focus();
                }
            }
        });

        keyboardRebindRoot.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            if (!keyboardBuilt)
            {
                MenuNavigation.Instance.SetKeyboard(SetupNavigation(keyboardRebindRoot));
                keyboardBuilt = true;

                currentList = MenuNavigation.Instance.keyboardNodes;

                if (GamepadActive() && current != null)
                {
                    current = FindTopLeft(currentList);
                    current.button.Focus();
                }
            }
        });

        controllerRebindRoot.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            if (!controllerBuilt)
            {
                MenuNavigation.Instance.SetController(SetupNavigation(controllerRebindRoot));
                controllerBuilt = true;

                currentList = MenuNavigation.Instance.controllerNodes;

                if (GamepadActive() && current != null)
                {
                    current = FindTopLeft(currentList);
                    current.button.Focus();
                }
            }
        });

        if (SceneLoader.Instance.nextLevel)
        {
            ToLevelSelect();
        }
        else
        {
            ToMain();
        }
    }

    private void OnEnable()
    {
        SetupButtons();
        back.performed += ctx => OnBack();
    }

    private void OnDisable()
    {
        back.performed -= ctx => OnBack();
    }

    private void Update()
    {
        if (gamepad && !GamepadActive())
        {
            gamepad = false;
            for (int i = 1; i <= 7; i++)
            {
                int index = i;
                Label lbl = controlsRoot.Q<Label>(index.ToString());
                lbl.text = controlDescriptions[index].start + playerStats.bindingNames[index - 1] + controlDescriptions[index].end;
            }

            look.text = "Looking Around: Move the mouse or use the arrow keys to look around.";
            movement.text = "Movement: Use WASD to move.";
            current = null;
        }
        else if (!gamepad && GamepadActive())
        {
            gamepad = true;
            for (int i = 1; i <= 7; i++)
            {
                int index = i;
                Label lbl = controlsRoot.Q<Label>(index.ToString());
                lbl.text = controlDescriptions[index].start + playerStats.controllerBindingNames[index - 1] + controlDescriptions[index].end;
            }

            look.text = "Looking Around: Move the Right Stick to look around.";
            movement.text = "Movement: Use the Left Stick to move.";
        }

        navigationAmt = navigation.ReadValue<Vector2>();
        if (navigationAmt != null && navigationAmt.sqrMagnitude > 0.01)
        {
            Navigate(navigationAmt);
        }
        else if (navigationAmt != null)
        {
            initial = true;
        }

        if (current == null)
        {
            noFocus.Focus();
        }
        else
        {
            current.button.Focus();
        }

        if (navigationTimer > 0f)
        {
            navigationTimer -= Time.unscaledDeltaTime;
        }
    }

    #region Buttons
    private void SetupButtons()
    {
        levelSelectButton = mainRoot.Q<Button>("LevelSelect");
        levelSelectButton.clicked += ToLevelSelect;

        //Level Select buttons
        for (int i = 1; i <= 10; i++)
        {
            int index = i;
            Button btn = levelSelectRoot.Q<Button>(index.ToString());

            if (playerStats.latestLevelUnlocked >= index)
            {
                btn.clicked += () =>
                {
                    level = index;
                    LevelDescription();
                    ToLevel();
                };
            }
            else
            {
                btn.AddToClassList("grayscale");
            }
        }

        levelSelectBackButton = levelSelectRoot.Q<Button>("Back");
        levelSelectBackButton.clicked += ToMain;

        levelBackButton = levelRoot.Q<Button>("Back");
        levelBackButton.clicked += ToLevelSelect;

        startButton = levelRoot.Q<Button>("Start");
        startButton.clicked += StartLevel;

        //Incantation Buttons
        for (int i = 1; i <= 10; i++)
        {
            int spellId = i;
            Button btn = levelRoot.Q<Button>(i.ToString());

            btn.RegisterCallback<FocusOutEvent>(evt =>
            {
                LevelDescription();
            });


            if (playerStats.unlockedSpells.Contains((SpellType)i))
            {
                btn.RegisterCallback<PointerEnterEvent>(evt =>
                {
                    IncantationDescription(spellId);
                });

                btn.RegisterCallback<FocusInEvent>(evt =>
                {
                    IncantationDescription(spellId);
                });

                btn.clicked += () => SpellClicked(spellId);

                int index = playerStats.ContainsSpell((SpellType)i);
                if (index > -1)
                {
                    Label lbl = levelRoot.Q<Label>(spellId.ToString() + "i");
                    btn.AddToClassList("selected");
                    lbl.style.display = DisplayStyle.Flex;
                    lbl.text = (index + 1).ToString();
                }
            }
            else
            {
                btn.AddToClassList("grayscale");

                btn.RegisterCallback<PointerEnterEvent>(evt =>
                {
                    subtitle.style.display = DisplayStyle.Flex;

                    if (btn.text == "?")
                    {
                        subtitle.text = "???";
                        description.text = "This spell does not appear in the demo.";
                    }
                    else
                    {
                        subtitle.text = "Unknown Spell";
                        description.text = "This spell has not been learned yet.\nSeek hidden secrets to acquire it.";
                    }
                });

                btn.RegisterCallback<FocusInEvent>(evt =>
                {
                    subtitle.style.display = DisplayStyle.Flex;

                    if (btn.text == "?")
                    {
                        subtitle.text = "???";
                        description.text = "This spell does not appear in the demo.";
                    }
                    else
                    {
                        subtitle.text = "Unknown Spell";
                        description.text = "This spell has not been learned yet.\nSeek hidden secrets to acquire it.";
                    }
                });
            }
        }

        settingsButton = mainRoot.Q<Button>("Settings");
        settingsButton.clicked += ToSettings;

        settingBackButton = settingsRoot.Q<Button>("BackButton");
        settingBackButton.clicked += ToMain;

        sensitivityDownButton = settingsRoot.Q<Button>("SensitivityDown");
        sensitivityDownButton.clicked += OnSensitivityDown;

        sensitivityUpButton = settingsRoot.Q<Button>("SensitivityUp");
        sensitivityUpButton.clicked += OnSensitivityUp;

        volumeDownButton = settingsRoot.Q<Button>("VolumeDown");
        volumeDownButton.clicked += OnVolumeDown;

        volumeUpButton = settingsRoot.Q<Button>("VolumeUp");
        volumeUpButton.clicked += OnVolumeUp;

        settingsKeyboardButton = settingsRoot.Q<Button>("Keyboard");
        settingsKeyboardButton.clicked += ToKeyboardRebind;

        settingsControllerButton = settingsRoot.Q<Button>("Controller");
        settingsControllerButton.clicked += ToControllerRebind;

        controlsButton = mainRoot.Q<Button>("Controls");
        controlsButton.clicked += ToControls;

        controlsKeyboardButton = controlsRoot.Q<Button>("Keyboard");
        controlsKeyboardButton.clicked += ToKeyboardRebind;

        controlsControllerButton = controlsRoot.Q<Button>("Controller");
        controlsControllerButton.clicked += ToControllerRebind;

        controlBackButton = controlsRoot.Q<Button>("Back");
        controlBackButton.clicked += ToMain;

        keyboardRebindBackButton = keyboardRebindRoot.Q<Button>("Back");
        keyboardRebindBackButton.clicked += ToSettings;

        keyboardToControllerButton = keyboardRebindRoot.Q<Button>("Controller");
        keyboardToControllerButton.clicked += ToControllerRebind;

        controllerRebindBackButton = controllerRebindRoot.Q<Button>("Back");
        controllerRebindBackButton.clicked += ToSettings;

        controllerToKeyboardButton = controllerRebindRoot.Q<Button>("Keyboard");
        controllerToKeyboardButton.clicked += ToKeyboardRebind;

        popupStartButton = levelRoot.Q<Button>("Continue");
        popupStartButton.clicked += LoadLevel;

        popupBackButton = levelRoot.Q<Button>("Back2");
        popupBackButton.clicked += () =>
        {
            popup.style.display = DisplayStyle.None;
            currentList = MenuNavigation.Instance.levelNodes;

            if (current != null && MenuNavigation.Instance.levelNodes.Count > 0)
            {
                current = FindTopLeft(MenuNavigation.Instance.levelNodes);
                current.button.Focus();
            }
        };

        List<NavNode> list = new List<NavNode>();
        NavNode back = new NavNode(popupBackButton);
        NavNode start = new NavNode(popupStartButton);
        back.right = start;
        start.left = back;
        list.Add(back);
        list.Add(start);
        MenuNavigation.Instance.SetPopup(list);

        //Keyboard Rebind Buttons
        for (int i = 1; i <= 7; i++)
        {
            int index = i;
            Button leftButton = keyboardRebindRoot.Q<Button>(index.ToString() + "l");
            Button rightButton = keyboardRebindRoot.Q<Button>(index.ToString() + "r");
            Label label = keyboardRebindRoot.Q<Label>(index.ToString());
            Label lbl = controlsRoot.Q<Label>(index.ToString());

            if (!GamepadActive())
            {
                label.text = playerStats.bindingNames[index - 1];
            }

            rightButton.clicked += () =>
            {
                playerStats.UpdateBindingName(index - 1, KeyboardRebinder.Instance.Increment(InputActions, actionNames[index]));
                label.text = playerStats.bindingNames[index - 1];
                if (!GamepadActive())
                {
                    lbl.text = controlDescriptions[index].start + playerStats.bindingNames[index - 1] + controlDescriptions[index].end;
                }
            };

            leftButton.clicked += () =>
            {
                playerStats.UpdateBindingName(index - 1, KeyboardRebinder.Instance.Decrement(InputActions, actionNames[index]));
                label.text = playerStats.bindingNames[index - 1];
                if (!GamepadActive())
                {
                    lbl.text = controlDescriptions[index].start + playerStats.bindingNames[index - 1] + controlDescriptions[index].end;
                }
            };
        }

        //Controller Rebind Buttons
        for (int i = 1; i <= 5; i++)
        {
            int index = i;
            Button leftButton = controllerRebindRoot.Q<Button>(index.ToString() + "l");
            Button rightButton = controllerRebindRoot.Q<Button>(index.ToString() + "r");
            Label label = controllerRebindRoot.Q<Label>(index.ToString());

            if (GamepadActive())
            {
                label.text = playerStats.controllerBindingNames[index - 1];
            }

            rightButton.clicked += () =>
            {
                playerStats.UpdateControllerBindingName(index - 1, GamepadRebinder.Instance.Increment(InputActions, actionNames[index]));
                label.text = playerStats.controllerBindingNames[index - 1];

            };

            leftButton.clicked += () =>
            {
                playerStats.UpdateControllerBindingName(index - 1, GamepadRebinder.Instance.Decrement(InputActions, actionNames[index]));
                label.text = playerStats.controllerBindingNames[index - 1];
            };
        }

        rightRestart = controllerRebindRoot.Q<Button>("6r");
        rightRestart.clicked += ToggleRestartAction;

        leftRestart = controllerRebindRoot.Q<Button>("6l");
        leftRestart.clicked += ToggleRestartAction;

        quitButton = mainRoot.Q<Button>("Quit");
        quitButton.clicked += OnQuit;
    }

    private void OnDestroy()
    {
        levelSelectButton.clicked -= ToLevelSelect;
        levelSelectBackButton.clicked -= ToMain;
        levelBackButton.clicked -= ToLevelSelect;
        startButton.clicked -= StartLevel;
        settingsButton.clicked -= ToSettings;
        settingBackButton.clicked -= ToMain;
        sensitivityDownButton.clicked -= OnSensitivityDown;
        sensitivityUpButton.clicked -= OnSensitivityUp;
        volumeDownButton.clicked -= OnVolumeDown;
        volumeUpButton.clicked -= OnVolumeUp;
        settingsKeyboardButton.clicked -= ToKeyboardRebind;
        settingsControllerButton.clicked -= ToControllerRebind;
        controlsButton.clicked -= ToControls;
        controlsKeyboardButton.clicked -= ToKeyboardRebind;
        controlsControllerButton.clicked -= ToControllerRebind;
        controlBackButton.clicked -= ToMain;
        keyboardRebindBackButton.clicked -= ToSettings;
        keyboardToControllerButton.clicked -= ToControllerRebind;
        controllerRebindBackButton.clicked -= ToSettings;
        controllerToKeyboardButton.clicked -= ToKeyboardRebind;
        popupStartButton.clicked -= LoadLevel;
        rightRestart.clicked -= ToggleRestartAction;
        leftRestart.clicked -= ToggleRestartAction;
        quitButton.clicked -= OnQuit;
    }
    #endregion

    #region Labels
    private void SetupLabels()
    {
        sensitivityLabel = settingsRoot.Q<Label>("SensitivityDisplay");
        volumeLabel = settingsRoot.Q<Label>("VolumeDisplay");

        sensitivityLabel.text = (playerStats.sensitivity * 100f).ToString("0");
        volumeLabel.text = (playerStats.musicVolume * 100f).ToString("0");

        for (int i = 1; i <= 10; i++)
        {
            Label lbl = levelRoot.Q<Label>(i.ToString() + "i");
            lbl.style.display = DisplayStyle.None;
        }

        for (int i = 1; i <= 7; i++)
        {
            int index = i;
            Label lbl = controlsRoot.Q<Label>(index.ToString());

            if (!GamepadActive())
            {
                lbl.text = controlDescriptions[index].start + playerStats.bindingNames[index - 1] + controlDescriptions[index].end;
            }
            else
            {
                lbl.text = controlDescriptions[index].start + playerStats.controllerBindingNames[index - 1] + controlDescriptions[index].end;
            }
        }

        title = levelRoot.Q<Label>("Title");
        subtitle = levelRoot.Q<Label>("Subtitle");
        description = levelRoot.Q<Label>("Description");

        movement = controlsRoot.Q<Label>("Movement");
        look = controlsRoot.Q<Label>("Look");
        if (!GamepadActive())
        {
            look.text = "Looking Around: Move the mouse or use the arrow keys to look around.";
            movement.text = "Movement: Use WASD to move.";
        }
        else
        {
            look.text = "Looking Around: Move the Right Stick to look around.";
            movement.text = "Movement: Use the Left Stick to move.";
        }
    }
    #endregion

    //<--- Navigation --->
    #region Navigation
    private void ToMain()
    {
        settingsRoot.style.display = DisplayStyle.None;
        controlsRoot.style.display = DisplayStyle.None;
        levelSelectRoot.style.display = DisplayStyle.None;
        levelRoot.style.display = DisplayStyle.None;
        mainRoot.style.display = DisplayStyle.Flex;
        keyboardRebindRoot.style.display = DisplayStyle.None;
        controllerRebindRoot.style.display = DisplayStyle.None;
        currentList = MenuNavigation.Instance.mainNodes;

        if (current != null && MenuNavigation.Instance.mainNodes.Count > 0)
        {
            current = FindTopLeft(MenuNavigation.Instance.mainNodes);
            current.button.Focus();
        }
    }

    private void ToLevelSelect()
    {
        levelSelectRoot.style.display = DisplayStyle.Flex;
        mainRoot.style.display = DisplayStyle.None;
        settingsRoot.style.display = DisplayStyle.None;
        controlsRoot.style.display = DisplayStyle.None;
        levelRoot.style.display = DisplayStyle.None;
        keyboardRebindRoot.style.display = DisplayStyle.None;
        controllerRebindRoot.style.display = DisplayStyle.None;
        currentList = MenuNavigation.Instance.levelSelectNodes;

        if (current != null && MenuNavigation.Instance.levelSelectNodes.Count > 0)
        {
            current = FindTopLeft(MenuNavigation.Instance.levelSelectNodes);
            current.button.Focus();
        }
    }

    private void ToLevel()
    {
        levelRoot.style.display = DisplayStyle.Flex;
        levelSelectRoot.style.display = DisplayStyle.None;
        mainRoot.style.display = DisplayStyle.None;
        settingsRoot.style.display = DisplayStyle.None;
        controlsRoot.style.display = DisplayStyle.None;
        keyboardRebindRoot.style.display = DisplayStyle.None;
        controllerRebindRoot.style.display = DisplayStyle.None;
        popup.style.display = DisplayStyle.None;
        currentList = MenuNavigation.Instance.levelNodes;

        if (current != null && MenuNavigation.Instance.levelNodes.Count > 0)
        {
            current = FindTopLeft(MenuNavigation.Instance.levelNodes);
            current.button.Focus();
        }
    }

    private void ToSettings()
    {
        settingsRoot.style.display = DisplayStyle.Flex;
        mainRoot.style.display = DisplayStyle.None;
        levelSelectRoot.style.display = DisplayStyle.None;
        controlsRoot.style.display = DisplayStyle.None;
        levelRoot.style.display = DisplayStyle.None;
        keyboardRebindRoot.style.display = DisplayStyle.None;
        controllerRebindRoot.style.display = DisplayStyle.None;
        currentList = MenuNavigation.Instance.settingsNodes;

        if (current != null && MenuNavigation.Instance.settingsNodes.Count > 0)
        {
            current = FindTopLeft(MenuNavigation.Instance.settingsNodes);
            current.button.Focus();
        }
    }

    private void ToControls()
    {
        controlsRoot.style.display = DisplayStyle.Flex;
        mainRoot.style.display = DisplayStyle.None;
        levelSelectRoot.style.display = DisplayStyle.None;
        settingsRoot.style.display = DisplayStyle.None;
        levelRoot.style.display = DisplayStyle.None;
        keyboardRebindRoot.style.display = DisplayStyle.None;
        controllerRebindRoot.style.display = DisplayStyle.None;
        currentList = MenuNavigation.Instance.controlsNodes;

        if (current != null && MenuNavigation.Instance.controlsNodes.Count > 0)
        {
            current = FindTopLeft(MenuNavigation.Instance.controlsNodes);
            current.button.Focus();
        }
    }

    private void ToKeyboardRebind()
    {
        keyboardRebindRoot.style.display = DisplayStyle.Flex;
        controlsRoot.style.display = DisplayStyle.None;
        mainRoot.style.display = DisplayStyle.None;
        levelSelectRoot.style.display = DisplayStyle.None;
        settingsRoot.style.display = DisplayStyle.None;
        levelRoot.style.display = DisplayStyle.None;
        controllerRebindRoot.style.display = DisplayStyle.None;
        currentList = MenuNavigation.Instance.keyboardNodes;

        if (current != null && MenuNavigation.Instance.keyboardNodes.Count > 0)
        {
            current = FindTopLeft(MenuNavigation.Instance.keyboardNodes);
            current.button.Focus();
        }
    }

    private void ToControllerRebind()
    {
        controllerRebindRoot.style.display = DisplayStyle.Flex;
        keyboardRebindRoot.style.display = DisplayStyle.None;
        controlsRoot.style.display = DisplayStyle.None;
        mainRoot.style.display = DisplayStyle.None;
        levelSelectRoot.style.display = DisplayStyle.None;
        settingsRoot.style.display = DisplayStyle.None;
        levelRoot.style.display = DisplayStyle.None;
        currentList = MenuNavigation.Instance.controllerNodes;

        if (current != null && MenuNavigation.Instance.controllerNodes.Count > 0)
        {
            current = FindTopLeft(MenuNavigation.Instance.controllerNodes);
            current.button.Focus();
        }
    }

    private void OnQuit()
    {
        Application.Quit();
    }

    public static List<NavNode> SetupNavigation(VisualElement root)
    {
        var buttons = root.Query<Button>().ToList();
        List<NavNode> list = new List<NavNode>();

        foreach (Button button in buttons)
        {
            list.Add(new NavNode(button));
        }

        foreach (NavNode node in list)
        {
            node.up = FindClosestInDirection(node, list, Vector2.up);
            node.down = FindClosestInDirection(node, list, Vector2.down);
            node.left = FindClosestInDirection(node, list, Vector2.right);
            node.right = FindClosestInDirection(node, list, Vector2.left);
        }

        return list;
    }

    public static NavNode FindClosestInDirection(NavNode current, List<NavNode> all, Vector2 direction)
    {
        Vector2 center = current.button.worldBound.center;

        NavNode best = null;
        float bestScore = float.MinValue;

        foreach (NavNode other in all)
        {
            if (other == current || other.button.name.Equals("NoFocus"))
            {
                continue;
            }

            Vector2 otherCenter = other.button.worldBound.center;
            Vector2 distance = center - otherCenter;

            float dot = Vector2.Dot(distance.normalized, direction.normalized);

            if (dot < 0.7f)
            {
                continue;
            }

            float score = dot / Mathf.Pow(distance.magnitude, 0.05f);

            if (score > bestScore)
            {
                bestScore = score;
                best = other;
            }
        }
        return best;
    }

    private void Navigate(Vector2 d)
    {
        if (navigationTimer > 0f || d == Vector2.zero)
        {
            return;
        }

        if (current == null)
        {
            current = FindTopLeft(currentList);
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

    public static NavNode FindTopLeft(List<NavNode> all)
    {
        if (all == null || all.Count == 0)
        {
            Debug.Log("null list or 0 count");
            return null;
        }

        List<NavNode> leftest = new List<NavNode>();

        foreach (NavNode node in all)
        {
            if (node.button.name.Equals("NoFocus"))
            {
                continue;
            }

            if (leftest.Count == 0)
            {
                leftest.Add(node);
            }
            else if (leftest[0].button.worldBound.x > node.button.worldBound.x)
            {
                leftest[0] = node;
            }
            else if (leftest[0].button.worldBound.x == node.button.worldBound.x)
            {
                leftest.Add(node);
            }
        }

        NavNode best = leftest[0];

        foreach (NavNode node in leftest)
        {
            if (node == best)
            {
                continue;
            }

            if (best.button.worldBound.y > node.button.worldBound.y)
            {
                best = node;
            }
        }
        return best;
    }

    private void OnBack()
    {
        if (levelSelectRoot.style.display == DisplayStyle.Flex)
        {
            ToMain();
        }
        else if (popup.style.display == DisplayStyle.Flex)
        {
            ToLevel();
        }
        else if (levelRoot.style.display == DisplayStyle.Flex)
        {
            ToLevelSelect();
        }
        else if (settingsRoot.style.display == DisplayStyle.Flex)
        {
            ToMain();
        }
        else if (controlsRoot.style.display == DisplayStyle.Flex)
        {
            ToMain();
        }
        else if (keyboardRebindRoot.style.display == DisplayStyle.Flex)
        {
            ToSettings();
        }
        else if (controllerRebindRoot.style.display == DisplayStyle.Flex)
        {
            ToSettings();
        }
    }

    #endregion

    //<--- Settings --->
    #region Settings
    private void OnSensitivityUp()
    {
        playerStats.IncreaseSensitivity(0.01f);
        sensitivityLabel.text = (playerStats.sensitivity * 100f).ToString("0");
    }

    private void OnSensitivityDown()
    {
        playerStats.DecreaseSensitivity(0.01f);
        sensitivityLabel.text = (playerStats.sensitivity * 100f).ToString("0");
    }

    private void OnVolumeUp()
    {
        playerStats.IncreaseMusicVolume(0.1f);
        volumeLabel.text = (playerStats.musicVolume * 100f).ToString("0");
    }

    private void OnVolumeDown()
    {
        playerStats.DecreaseMusicVolume(0.1f);
        volumeLabel.text = (playerStats.musicVolume * 100f).ToString("0");
    }

    private void ToggleRestartAction()
    {
        Label lbl = controllerRebindRoot.Q<Label>("6");

        if (playerStats.controllerBindingNames[6].Equals("None"))
        {
            playerStats.UpdateControllerBindingName(6, "D-Pad (Any)");
            ActionRebinder.Rebind(InputActions, "Restart", "", "<Gamepad>/dpad/up");
            ActionRebinder.Rebind(InputActions, "Restart", "", "<Gamepad>/dpad/down");
            ActionRebinder.Rebind(InputActions, "Restart", "", "<Gamepad>/dpad/left");
            ActionRebinder.Rebind(InputActions, "Restart", "", "<Gamepad>/dpad/right");
            lbl.text = "D-Pad (Any)";
        }
        else
        {
            playerStats.UpdateControllerBindingName(6, "None");
            ActionRebinder.Rebind(InputActions, "Restart", "<Gamepad>/dpad/up", "");
            ActionRebinder.Rebind(InputActions, "Restart", "<Gamepad>/dpad/down", "");
            ActionRebinder.Rebind(InputActions, "Restart", "<Gamepad>/dpad/left", "");
            ActionRebinder.Rebind(InputActions, "Restart", "<Gamepad>/dpad/right", "");
            lbl.text = "None";
        }
    }

    #endregion

    #region Level
    //<--- Level --->
    private void LevelDescription()
    {
        if (levelInfo.ContainsKey(level))
        {
            title.text = levelInfo[level].name;
            description.text = levelInfo[level].description;
            subtitle.text = levelInfo[level].objective;
        }
    }

    private void IncantationDescription(int spellType)
    {
        if (spellInfo.ContainsKey(spellType))
        {
            subtitle.text = spellInfo[spellType].name;
            subtitle.style.display = DisplayStyle.Flex;
            description.text = spellInfo[spellType].description;
        }
    }

    private void SpellClicked(int spellType)
    {
        int spellIndex = 0;
        Button clickedBtn = levelRoot.Q<Button>(spellType.ToString());
        Label lbl = levelRoot.Q<Label>(spellType.ToString() + "i");

        if (clickedBtn.ClassListContains("selected"))
        {
            playerStats.RemoveSpell((SpellType)spellType);
            clickedBtn.RemoveFromClassList("selected");
            lbl.style.display = DisplayStyle.None;
            return;
        }

        while (spellIndex < 3 && playerStats.spells[spellIndex] != SpellType.None)
        {
            spellIndex++;
        }

        if (spellIndex >= 3 && playerStats.spells[3] != SpellType.None)
        {
            Button btnToRemove = levelRoot.Q<Button>(((int)playerStats.spells[3]).ToString());
            Label lblToRemove = levelRoot.Q<Label>(((int)playerStats.spells[3]).ToString() + "i");
            btnToRemove.RemoveFromClassList("selected");
            lblToRemove.style.display = DisplayStyle.None;
        }

        clickedBtn.AddToClassList("selected");
        lbl.style.display = DisplayStyle.Flex;
        lbl.text = (spellIndex + 1).ToString();
        playerStats.spells[spellIndex] = (SpellType)spellType;
    }

    private void StartLevel()
    {
        bool popupShown = false;

        for (int i = 0; i < playerStats.spells.Length; i++)
        {
            if (playerStats.spells[i] == SpellType.None)
            {
                popupShown = true;
                break;
            }
        }

        if (popupShown)
        {
            popup.style.display = DisplayStyle.Flex;
            currentList = MenuNavigation.Instance.popupNodes;

            if (current != null)
            {
                current = FindTopLeft(MenuNavigation.Instance.popupNodes);
                current.button.Focus();
            }
        }
        else
        {
            LoadLevel();
        }
    }

    private void LoadLevel()
    {
        popup.style.display = DisplayStyle.None;
        SceneLoader.Instance.LoadLevel(level);
    }

    #endregion

    #region Structs
    private struct LevelInfo
    {
        public string name;
        public string description;
        public string objective;

        public LevelInfo(string t, string d, string o)
        {
            name = t;
            description = d;
            objective = o;
        }
    }

    private struct SpellInfo
    {
        public string name;
        public string description;

        public SpellInfo(string n, string d)
        {
            name = n;
            description = d;
        }
    }

    private struct Description
    {
        public string start;
        public string end;

        public Description(string s, string e)
        {
            start = s;
            end = e;
        }
    }

    private bool GamepadActive()
    {
        return Gamepad.current != null
            && Gamepad.current.name != "XInputControllerWindows";
    }


    #endregion
}
