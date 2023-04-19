using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameoverMenuController : MenuController
{
    public Button viewButton;
    public Button restartButton;
    public Button quitButton;
    public TextMeshProUGUI title;
    public GameoverDelegate gameoverDelegate;

    public override void SetDelegate(MonoBehaviour value)
    {
        gameoverDelegate = value as GameoverDelegate;
        restartButton.onClick.AddListener(gameoverDelegate.restartButtonPressed);
        quitButton.onClick.AddListener(gameoverDelegate.quitButtonPressed);
        viewButton.onClick.AddListener(gameoverDelegate.viewButtonPressed);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
