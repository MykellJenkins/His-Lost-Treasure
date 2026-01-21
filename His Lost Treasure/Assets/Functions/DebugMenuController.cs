using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;

public class DebugMenuController : MonoBehaviour
{
    bool showConsole;
    bool showHelp;

    string input;

    public static DebugCommand KILL_ALL_ENEMIES;
    public static DebugCommand RESPAWN_PLAYER;
    public static DebugCommand GIVE_LIFE;
    public static DebugCommand<int> SET_MONEY;
    public static DebugCommand HELP;

    public List<object> commandList;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))  
        {
            OnToggleDebug();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnReturn();
        }
         
    }

    private void Awake()
    {
        KILL_ALL_ENEMIES = new DebugCommand("kill_all_enemies", "Removes all enemies from the scene.", "kill_all_enemies", () =>
        {
            // need to make a function to remove all enemies in game manager
            //GameManager.instance.KillAllEnemies();
        });

        RESPAWN_PLAYER = new DebugCommand("respawn_player", "Respawns the player at the last checkpoint.", "respawn_player", () =>
        {
            GameManager.Instance.rmInstance.RespawnPlayer(GameManager.Instance.playerScript);
        });

        GIVE_LIFE = new DebugCommand("give_life", "Gives full lives to the player.", "give_life", () =>
        {
            GameManager.Instance.playerScript.maxLives = 3;
        });

        SET_MONEY = new DebugCommand<int>("set_money", "Gives a specfiied amount of money.", "set_money <moneyAmount>", (x) =>
        {
            // need money implemented
            //GameManager.instance.money = x; 
        });

        HELP = new DebugCommand("help", "Shows the commands to help.", "help", () =>
        {
            showHelp = true;
        });

        commandList = new List<object>
        {
            KILL_ALL_ENEMIES, 
            RESPAWN_PLAYER, 
            GIVE_LIFE,
            SET_MONEY,
            HELP
        };
    }

    public void OnToggleDebug()
    {
        showConsole = !showConsole;

        if (!GameManager.Instance.isPaused)
        {
            GameManager.Instance.StatePause();
        }
        else
        {
            GameManager.Instance.StateUnpause();
        }
    }

    Vector2 scroll;

    private void OnGUI()
    {
        if (!showConsole)
        {
            return;
        }

        float y = 0f;

        if (showHelp)
        {
            GUI.Box(new Rect(0, y, Screen.width, 100), "");

            Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * commandList.Count);

            scroll = GUI.BeginScrollView(new Rect(0, y + 5f, Screen.width, 90), scroll, viewport);

            for (int i = 0; i < commandList.Count; i++)
            {
                DebugCommands command = commandList[i] as DebugCommands;

                string label = $"{command.commandFormat} - {command.commandDescription}";

                Rect labelrect = new Rect(5, 20 * i, viewport.width - 100, 20);

                GUI.Label(labelrect, label);
            }
            GUI.EndScrollView();
            y += 100;
        }

        GUI.Box(new Rect(0, y, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);
    }

    public void OnReturn()
    {
        if (showConsole)
        {
            HandleInput();
            input = "";
        }
    }

    private void HandleInput()
    {
        string[] properties = input.Split(' ');
        string commandId = properties[0].ToLower();

        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommands commandBase = commandList[i] as DebugCommands;

            if (commandBase.commandId == commandId)
            {
                if (commandList[i] as DebugCommand != null)
                {
                    (commandList[i] as DebugCommand).Invoke();
                    return;
                }
                else if (commandList[i] as DebugCommand<int> != null)
                {
                    if (properties.Length > 1 && int.TryParse(properties[1], out int value))
                    {
                        (commandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[1]));
                        input = "";
                        return;
                    }
                    else
                    {
                        Debug.LogWarning("Missing or invalid argument for " + commandId);
                    }
                }
                return;
            }
            Debug.Log("Unknown command: " + commandId);
            input = "";
        }    
    }
}
