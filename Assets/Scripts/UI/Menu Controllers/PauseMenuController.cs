using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MenuController
{
    public Button resumeButton;
    public Button settingsButton;
    public Button quitButton;
    public PauseDelegate pauseDelegate;

    // Start is called before the first frame update

    public override void SetDelegate(MonoBehaviour value)
    {
        pauseDelegate = value as PauseDelegate;
        resumeButton.onClick.AddListener(pauseDelegate.ResumeButtonPressed);
        settingsButton.onClick.AddListener(pauseDelegate.SettingsButtonPressed);
        quitButton.onClick.AddListener(pauseDelegate.QuitButtonPressed);
    }
}
