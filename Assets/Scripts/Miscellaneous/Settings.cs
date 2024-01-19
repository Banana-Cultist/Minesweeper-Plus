using System;
using UnityEngine;

public static class Settings
{

	public static bool GetToggle(Toggles toggle)
	{
		return PlayerPrefs.GetInt(toggle.ToString(), 0) == 1;
	}

	public static void BindToggleButton(ButtonToggle button, Toggles toggle)
	{
        button.SetState(PlayerPrefs.GetInt(toggle.ToString(), 0) == 1);
        button.listener = () =>
        {
            PlayerPrefs.SetInt(toggle.ToString(), button.state ? 1 : 0);
        };
    }

	public static bool IsClearBindingPressed()
	{
        var clear = Bindings.CLEAR.ToString();
        return
			PlayerPrefs.GetString(clear, "null") != "null" &&
			Input.GetKeyDown((KeyCode) Enum.Parse(
				typeof(KeyCode),
				PlayerPrefs.GetString(clear, "")
			)
		);
    }

    public static bool IsFlagBindingPressed()
    {
        var flag = Bindings.FLAG.ToString();
        return
            PlayerPrefs.GetString(flag, "null") != "null" &&
            Input.GetKeyDown((KeyCode) Enum.Parse(
                typeof(KeyCode),
                PlayerPrefs.GetString(flag, "")
            )
        );
    }

    public static void BindKeyBinder(KeyBinder toggle, Bindings binding)
	{
        if (PlayerPrefs.HasKey(binding.ToString()))
        {
            toggle.SetBinding(
                (KeyCode) Enum.Parse(
                    typeof(KeyCode),
                    PlayerPrefs.GetString("Flag", null)
                )
            );
        }
        else
        {
            toggle.SetBinding(null);
        }

        toggle.listener = () =>
        {
            PlayerPrefs.SetString(
                binding.ToString(),
                toggle.binding != null ? toggle.binding.ToString() : "UNBOUND"
            );
        };
    }
}

public enum Toggles
{
    CLEAN_OPENING,
    CLEAR_CHORD,
    FLAG_CHORD,
    MINE_SWEEPING
}

public enum Bindings
{
    CLEAR,
    FLAG
}
