using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MenuController
{
    public ScrollRect scrollView;
    public Button[] sideBar;
    public TMP_Text[] contentTitles;
    public KeyBinder clearBind;
    public KeyBinder flagBind;
    public ButtonToggle cleanOpening;
    public ButtonToggle clearChord;
    public ButtonToggle flagChord;
    public ButtonToggle mineSweeping;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("Clear"))
        {
            clearBind.SetBinding(null);
        }
        else
        {
            clearBind.SetBinding((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Clear", null)));
        }
        clearBind.listener = () =>
        {
            PlayerPrefs.SetString("Clear", clearBind.binding != null ? clearBind.binding.ToString() : "UNBOUND");
        };

        if (!PlayerPrefs.HasKey("Flag"))
        {
            flagBind.SetBinding(null);
        }
        else
        {
            flagBind.SetBinding((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Flag", null)));
        }
        flagBind.listener = () =>
        {
            PlayerPrefs.SetString("Flag", flagBind.binding != null ? flagBind.binding.ToString() : "UNBOUND");
        };

        cleanOpening.SetState(PlayerPrefs.GetInt("CleanOpening", 0) == 1);
        cleanOpening.listener = () =>
        {
            PlayerPrefs.SetInt("CleanOpening", cleanOpening.state ? 1 : 0);
        };

        clearChord.SetState(PlayerPrefs.GetInt("ClearChord", 0) == 1);
        clearChord.listener = () =>
        {
            PlayerPrefs.SetInt("ClearChord", clearChord.state ? 1 : 0);
        };

        flagChord.SetState(PlayerPrefs.GetInt("FlagChord", 0) == 1);
        flagChord.listener = () =>
        {
            PlayerPrefs.SetInt("FlagChord", flagChord.state ? 1 : 0);
        };

        mineSweeping.SetState(PlayerPrefs.GetInt("MineSweeping", 0) == 1);
        mineSweeping.listener = () =>
        {
            PlayerPrefs.SetInt("MineSweeping", mineSweeping.state ? 1 : 0);
        };


    }
    
    public override void SetDelegate(MonoBehaviour value)
    {
        
    }
}
