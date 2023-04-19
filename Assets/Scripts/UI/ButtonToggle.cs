using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.U2D;
using UnityEngine.Events;

[ExecuteAlways]
public class ButtonToggle : MonoBehaviour
{
    private Button button;
    private TMP_Text buttonText;
    public bool state;
    public UnityAction listener;

    // Start is called before the first frame update
    void Awake()
    {
        button = gameObject.GetComponent<Button>();
        buttonText = gameObject.GetComponentInChildren<TMP_Text>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(Toggle);
    }

    private void Toggle()
    {
        state = !state;
        buttonText.text = state ? "Enabled" : "Disabled";

        Debug.Log(listener);
        listener();
    }

    public void SetState(bool value)
    {
        state = value;
        updateLabel();
    }

    private void updateLabel()
    {
        buttonText.text = state ? "Enabled" : "Disabled";
    }

#if UNITY_EDITOR
    // Update is called once per frame
    void Update()
    {
        buttonText.text = state ? "Enabled" : "Disabled";
    }
#endif

}
