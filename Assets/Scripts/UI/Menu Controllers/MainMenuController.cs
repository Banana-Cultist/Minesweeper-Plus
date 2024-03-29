using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface PauseDelegate {
    void ResumeButtonPressed();
    void SettingsButtonPressed();
    void QuitButtonPressed();
}

public interface GameoverDelegate
{
    void RestartButtonPressed();
    void QuitButtonPressed();
    void ViewButtonPressed();
}

public interface BoardDelegate
{
    void GameCompleted(bool status);
}

public class MainMenuController : MonoBehaviour, PauseDelegate, BoardDelegate, GameoverDelegate
{
    public MenuController[] menuControllers;
    public enum MenuState { PLAYING, PAUSE, SETTINGS, GAMEOVER, POSTVIEW }
    public MenuState state = MenuState.PLAYING;
    public BoardController board;
    public CameraController cameraController;

    // Start is called before the first frame update
    void Start()
    {
        foreach (MenuController controller in menuControllers)
        {
            controller.SetDelegate(this);
        }
        board.menuDelegate = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == MenuState.PLAYING && Input.GetKeyDown(KeyCode.Escape))
        {
            pause();
        }
        else if (state == MenuState.PAUSE && Input.GetKeyDown(KeyCode.Escape))
        {
            ResumeButtonPressed();
        }
        else if (state == MenuState.SETTINGS && Input.GetKeyDown(KeyCode.Escape))
        {
            ResumeButtonPressed();
        }
        else if (state == MenuState.GAMEOVER && Input.GetKeyDown(KeyCode.Escape))
        {
            ViewButtonPressed();
        }
        else if (state == MenuState.POSTVIEW && Input.GetKeyDown(KeyCode.Escape))
        {
            state = MenuState.GAMEOVER;
            cameraController.movable = false;
            menuControllers[2].SetActive(true);
        }
    }

    private void pause()
    {
        state = MenuState.PAUSE;
        board.paused = true;
        cameraController.movable = false;
        SetAllFalse();
        menuControllers[0].SetActive(true);
    }

    private void SetAllFalse()
    {
        foreach (MenuController controller in menuControllers)
        {
            controller.SetActive(false);
        }
    }

    public void GameCompleted(bool status)
    {
        state = MenuState.GAMEOVER;
        menuControllers[2].SetActive(true);
        board.paused = true;
        cameraController.movable = false;

        if (!status)
        {
            board.Reveal();
        }
    }

    public void QuitButtonPressed()
    {
        Debug.Log("Quit Button Pressed");
    }

    public void ResumeButtonPressed()
    {
        state = MenuState.PLAYING;
        board.paused = false;
        cameraController.movable = true;
        SetAllFalse();
    }

    public void SettingsButtonPressed()
    {
        Debug.Log("Settings Button Pressed");
        state = MenuState.SETTINGS;
        SetAllFalse();
        menuControllers[1].SetActive(true);
    }

    public void RestartButtonPressed()
    {
        state = MenuState.PLAYING;
        board.paused = false;
        cameraController.movable = true;

        SetAllFalse();
        board.ResetBoard();
    }

    public void ViewButtonPressed()
    {
        state = MenuState.POSTVIEW;
        cameraController.movable = true;
        SetAllFalse();
    }
}
