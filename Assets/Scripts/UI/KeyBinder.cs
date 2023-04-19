using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

//[ExecuteAlways]
public class KeyBinder : MonoBehaviour
{
    private Button button;
    private TMP_Text buttonText;
    public KeyCode? binding;
    private bool currentlyBinding = false;
    public UnityAction listener;

    // Start is called before the first frame update
    void Awake()
    {
        button = gameObject.GetComponent<Button>();
        buttonText = gameObject.GetComponentInChildren<TMP_Text>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(beginBinding);
    }

    void OnGUI()
    {
        if (!currentlyBinding) return;

        Event action = Event.current;

        Debug.Log(listener);

        if (action.isKey && action.type == EventType.KeyDown)
        {
            binding = action.keyCode;
            updateLabel();
            currentlyBinding = false;
            listener();
        } else if (Input.GetMouseButtonDown(0))
        {
            binding = null;
            updateLabel();
            currentlyBinding = false;
            listener();
        }
    }

    private void beginBinding()
    {
        currentlyBinding = true;
    }

    public void SetBinding(KeyCode? value)
    {
        binding = value;
        updateLabel();
    }

    private void updateLabel()
    {
        Debug.Log(binding);
        if (binding != null)
        {
            buttonText.text = binding.ToString();
        }
        else
        {
            buttonText.text = "Unbound";
        }
    }
}
