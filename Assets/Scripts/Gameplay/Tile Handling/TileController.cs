using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

[ExecuteInEditMode]
public class TileController : MonoBehaviour
{
    public Vector2[] points;
    public HashSet<TileController> adjacents = new();
    public Color fillColor;
    public Color borderColor;
    public Vector3 borderAccent = Vector3.zero;
    public bool cleared = false;
    public bool flagged = false;
    public bool isMine = false;
    public enum Highlight {Selected, Adjacent, None};
    public Highlight highlight = Highlight.None;
    public int adjacentMines = 0;

    public int rounding;
    //public TextMeshPro text;
    public TileIcon image;
    public TileRenderer tileRenderer;
    public ITileDelegate tileDelegate;

    // Start is called before the first frame update
    void Start()
    {
        image.Hide();
    }

    public float Round(float value, int decimals)
    {
        return Mathf.Floor(value * Mathf.Pow(10, decimals)) / ((int) Mathf.Pow(10, decimals));
    }

    public void UpdateShape()
    {
        // clear out points that are too close to be rendered
        RemoveClosePoints(0.1);

        tileRenderer.SetPoints(points);
        // tileRenderer.SetCoverColor(fillColor);
        // tileRenderer.SetFillColor(ApplyAccent(fillColor, Vector3.one * 0.5f));
        // tileRenderer.SetBorderColor(borderColor);
        tileRenderer.SetBorderWidth(tileDelegate == null ? 1 : tileDelegate.GetLineWidth());

        // update textbox shape
        image.Resize(points, GetBounds());
    }

    public void UpdateColor()
    {
        tileRenderer.SetCoverColor(fillColor);
        tileRenderer.SetFillColor(ApplyAccent(fillColor, Vector3.one * 0.5f));
        tileRenderer.SetBorderColor(borderColor);
    }

    private void RemoveClosePoints(double accuracy)
    {
        List<Vector2> rawPoints = new();
        for (int i = 0; i < points.Length; i++)
        {
            bool valid = true;
            for (int j = 0; j < rawPoints.Count; j++)
            {
                if (Vector2.Distance(points[i], rawPoints[j]) < accuracy)
                {
                    valid = false;
                    break;
                }
            }
            if (valid) rawPoints.Add(points[i]);
        }
        points = new Vector2[rawPoints.Count];
        points = rawPoints.ToArray();
    }

    public void UpdateLabel()
    {
        image.SetNumber(adjacentMines);
    }

    private void Awake()
    {
        UpdateShape();
    }

    private Color ApplyAccent(Color color, Vector3 accent)
    {
        return new Color(
            Mathf.Clamp01(color.r + accent.x),
            Mathf.Clamp01(color.g + accent.y),
            Mathf.Clamp01(color.b + accent.y),
        color.a);
    }

    // Update is called once per frame
    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UpdateShape();
        }
#endif

        tileRenderer.SetHighlight(highlight);
        highlight = Highlight.None;
    }

    void LateUpdate()
    {
        if (tileDelegate == null || tileDelegate.IsPaused()) return;
        Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Contains(points, clickPos))
        {
            if (Input.GetMouseButtonDown(0) || Settings.IsClearBindingPressed())
            {
                // clear
                tileDelegate.Click(this);
                tileDelegate.SetClickPosition(clickPos);
            }
            else if (Input.GetMouseButtonDown(1) || Settings.IsFlagBindingPressed())
            {
                // flag
                tileDelegate.Flag(this);
            }
            // highlight
            highlight = Highlight.Selected;
            foreach (TileController neighbor in adjacents)
            {
                neighbor.highlight = Highlight.Adjacent;
            }
        }
    }

    public Bounds GetBounds()
    {
        float minX = points[0].x;
        float maxX = minX;
        float minY = points[0].y;
        float maxY = minY;

        for (int i = 1; i < points.Length; i++)
        {
            float x = points[i].x;
            maxX = x > maxX ? x : maxX;
            minX = x < minX ? x : minX;
            float y = points[i].y;
            maxY = y > maxY ? y : maxY;
            minY = y < minY ? y : minY;
        }

        Vector3 center = new((minX + maxX) / 2, (minY + maxY) / 2);
        Vector3 size = new(maxX - minX, maxY - minY);
        return new Bounds(center, size);
    }

    public static bool Contains(Vector2[] points, Vector2 p)
    {
        bool contained = false;
        Vector2 p1 = points[0];
        Vector2 p2;
        for (int i = 1; i <= points.Length; i++)
        {
            p2 = points[i % points.Length];
            if (p.y > Math.Min(p1.y, p2.y) &&
                p.y <= Math.Max(p1.y, p2.y) &&
                p.x <= Math.Max(p1.x, p2.x))
            {
                float intersection = (p.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) + p1.x;
                if (p1.x == p2.x || p.x <= intersection)
                {
                    contained = !contained;
                }
            }
            p1 = p2;
        }
        return contained;
    }

    public bool Contains(Vector2 p)
    {
        return Contains(points, p);
    }

    public float MaxDistance(Vector2 p)
    {
        float max = float.MinValue;
        for (int i = 0; i < points.Length; i++)
        {
            float dist2 = (points[i] - p).magnitude;
            if (dist2 > max)
            {
                max = dist2;
            }
        }
        return max;
    }

    public void Flag()
    {
        flagged = true;
        image.ShowFlag();
    }

    public void Unflag()
    {
        flagged = false;
        image.HideFlag();
    }

    public void Reset()
    {
        flagged = false;
        cleared = false;
        adjacentMines = 0;
        isMine = false;
        UpdateLabel();
        image.Hide();
        tileRenderer.Reset();
    }

    public void Reveal()
    {
        if (!cleared)
        {
            if (isMine && !flagged)
            {
                image.ShowMine();
            }
            else if (!isMine && flagged)
            {
                image.Hide();
                //text.enabled = true;
            }
            else if (!isMine && !flagged)
            {
                //text.enabled = true;
            }
        }
    }

    public void PrepClearAnimation(int maskId)
    {
        tileRenderer.PrepClearAnimation(maskId);
        cleared = true;
        if (isMine)
        {
            image.ShowMine();
        }
        else
        {
            image.ShowNumber();
        }
        
        // image.flagSprite.enabled = true;

        flagged = false;
        image.PrepClearAnimation(maskId);
    }

    public void FinishClearAnimation()
    {
        tileRenderer.FinishClearAnimation();
        image.CompleteClearAnimation();
    }
}
