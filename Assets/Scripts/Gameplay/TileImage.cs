using System;
using UnityEngine;

public class TileImage : MonoBehaviour
{
	public SpriteRenderer mineSprite;
	public SpriteRenderer flagSprite;

    public void Awake()
    {
		mineSprite.enabled = false;
		flagSprite.enabled = false;
    }

    public void Resize(Vector2 sizeDelta, Vector3 position)
	{
		mineSprite.transform.localScale = sizeDelta;
		mineSprite.transform.position = position;

        flagSprite.transform.localScale = sizeDelta;
        flagSprite.transform.position = position;
	}

    public void DisplayMine()
	{
		mineSprite.enabled = true;
		flagSprite.enabled = false;
	}

	public void DisplayFlag()
	{
        mineSprite.enabled = false;
        flagSprite.enabled = true;
    }

	public void Hide()
	{
        mineSprite.enabled = false;
        flagSprite.enabled = false;
    }
}

