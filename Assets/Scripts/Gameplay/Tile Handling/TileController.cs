using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

[ExecuteAlways]
public class TileController : MonoBehaviour
{
    public Vector2[] points;
    public HashSet<TileController> adjacents = new();
    public Color fillColor;
    public Vector3 fillAccent = Vector3.zero;
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

        tileRenderer.SetCoverColor(ApplyAccent(fillColor, fillAccent));

        tileRenderer.SetFillColor(ApplyAccent(fillColor, fillAccent));

        tileRenderer.SetBorderColor(borderColor);
        
        tileRenderer.SetBorderWidth(tileDelegate == null ? 1 : tileDelegate.GetLineWidth());

        // update textbox shape
        image.Resize(points, tileRenderer);
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

        tileRenderer.SetFillColor(ApplyAccent(fillColor, fillAccent));
        tileRenderer.SetCoverColor(ApplyAccent(fillColor, fillAccent));

        tileRenderer.SetHighlight(highlight);
        highlight = Highlight.None;
    }

    void LateUpdate()
    {
        if (tileDelegate == null || tileDelegate.IsPaused()) return;
        if (tileRenderer.Contains(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            if (Input.GetMouseButtonDown(0) || Settings.IsClearBindingPressed())
            {
                // clear
                tileDelegate.Click(this);
            }
            else if (Input.GetMouseButtonDown(1) || Settings.IsFlagBindingPressed())
            {
                // flag
                tileDelegate.Flag(this);
            }
            else
            {
                // highlight
                highlight = Highlight.Selected;
                foreach (TileController neighbor in adjacents)
                {
                    neighbor.highlight = Highlight.Adjacent;
                }
            }
        }
    }

    public void Clear()
    {
        fillAccent = Vector3.one * 0.5f;
        //borderAccent = Vector3.one * 1;
        //borderController.sortingOrder = 1;
        //text.enabled = true;
        image.ShowNumber();
        cleared = true;
        if (isMine)
        {
            image.ShowMine();
        }
    }

    public void Flag()
    {
        //fillAccent = new Vector3(1, -1, -1);
        //borderAccent = Vector3.one * 1;
        //borderController.sortingOrder = 2;
        flagged = true;
        image.ShowFlag();
    }

    public void Unflag()
    {
        //fillAccent = Vector3.zero;
        //borderAccent = Vector3.zero;
        //borderController.sortingOrder = 0;
        flagged = false;
        image.HideFlag();
    }

    public void ResetValues()
    {
        fillAccent = Vector3.zero;
        flagged = false;
        cleared = false;
        adjacentMines = 0;
        isMine = false;
        UpdateLabel();
        image.Hide();
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
                fillAccent = new Vector3(1, -1, -1);
                //text.enabled = true;
            }
            else if (!isMine && !flagged)
            {
                //text.enabled = true;
            }
        }
    }
}
