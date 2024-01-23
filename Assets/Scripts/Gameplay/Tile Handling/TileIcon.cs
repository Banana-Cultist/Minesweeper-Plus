using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TileIcon : MonoBehaviour
{
	public SpriteRenderer mineSprite;
	public SpriteRenderer flagSprite;
    public TextMeshPro numberText;

    public void Awake()
    {
		mineSprite.enabled = false;
		flagSprite.enabled = false;
    }


	public void Resize(Vector2[] points, TileRenderer tileRenderer)
	{
        int maxMisses = 10;
        float decayConstant = 2.8284f;
        float target = 1E-3f;
        int count = 0;

        Bounds bounds = tileRenderer.GetBounds();
        Vector2 center = tileRenderer.Contains(bounds.center) ? bounds.center : points[0];
        float accuracy = float.MaxValue;
        do
        {
            int misses = 0;
            while (misses < maxMisses)
            {
                Vector2 random = new(
                    UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                    UnityEngine.Random.Range(bounds.min.y, bounds.max.y)
                );
                if (BorderDistance(points, random) > BorderDistance(points, center) &&
                    random != center && tileRenderer.Contains(random))
                {
                    center = random;
                    misses = 0;
                }
                else
                {
                    misses++;
                }
            }
            bounds.SetMinMax(
                center - (Vector2)bounds.size / decayConstant,
                center + (Vector2)bounds.size / decayConstant
            );
            // if accuracy is no longer changing, break
            if (accuracy == Mathf.Min(bounds.size.x, bounds.size.y)) break;
            accuracy = Mathf.Min(bounds.size.x, bounds.size.y);
            count++;
        } while (accuracy > target);

        Vector2 newSizeDelta = BorderDistance(points, center) * 1.5f * Vector2.one;
        Vector3 newPosition = (Vector3) center + transform.position;

        mineSprite.transform.localScale = newSizeDelta;
        mineSprite.transform.localPosition = newPosition;

        flagSprite.transform.localScale = newSizeDelta;
        flagSprite.transform.localPosition = newPosition;

        numberText.rectTransform.sizeDelta = newSizeDelta;
        numberText.rectTransform.localPosition = newPosition;
    }

    private float BorderDistance(Vector2[] points, Vector2 p)
    {
        float minimum = float.MaxValue;
        for (int i = 0; i < points.Length; i++)
        {
            Vector2 s = points[i];
            Vector2 e = points[(i + 1) % points.Length];
            float numerator = (p.x - s.x) * (e.x - s.x) + (p.y - s.y) * (e.y - s.y);
            float denominator = (e.x - s.x) * (e.x - s.x) + (e.y - s.y) * (e.y - s.y);
            float t = Mathf.Clamp01(numerator / denominator);
            float distance = Vector2.Distance(p, s + t * (e - s));
            minimum = Mathf.Min(minimum, distance);
        }
        return minimum;
    }

    public void SetNumber(int number)
    {
        numberText.text = number != 0 ? number.ToString() : "";
    }

    public void ShowNumber()
    {
        numberText.enabled = true;
    }

    public void ShowMine()
	{
		mineSprite.enabled = true;
		flagSprite.enabled = false;
        numberText.enabled = false;
	}

	public void ShowFlag()
	{
        mineSprite.enabled = false;
        flagSprite.enabled = true;
    }

	public void HideFlag()
	{
        flagSprite.enabled = false;
    }

    public void Hide()
    {
        mineSprite.enabled = false;
        flagSprite.enabled = false;
        numberText.enabled = false;
    }
}

