using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Settings;

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
        Settings.BindKeyBinder(clearBind, Bindings.CLEAR);

        Settings.BindKeyBinder(flagBind, Bindings.FLAG);

        Settings.BindToggleButton(cleanOpening, Toggles.CLEAN_OPENING);

        Settings.BindToggleButton(clearChord, Toggles.CLEAR_CHORD);

        Settings.BindToggleButton(flagChord, Toggles.FLAG_CHORD);

        Settings.BindToggleButton(mineSweeping, Toggles.MINE_SWEEPING);
    }
    
    public override void SetDelegate(MonoBehaviour value)
    {
        
    }
}
