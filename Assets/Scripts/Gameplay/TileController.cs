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
    public HashSet<TileController> adjacents = new HashSet<TileController>();
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


    public PolygonCollider2D polygonCollider;
    public SpriteShapeRenderer shapeRenderer;
    public SpriteShapeController shapeController;
    public int rounding;
    public LineRenderer borderController;
    public int borderQuality;
    public TextMeshPro text;

    public TileDelegate tileDelegate;

    // Start is called before the first frame update
    void Start()
    {
        text.enabled = false;
    }

    public float round(float value, int decimals)
    {
        return Mathf.Floor(value * Mathf.Pow(10, decimals)) / ((int) Mathf.Pow(10, decimals));
    }

    public void UpdateShape()
    {
        // clear out points that are too close to be rendered
        List<Vector2> rawPoints = new();
        for (int i = 0; i < points.Length; i++)
        {
            bool valid = true;
            for(int j = 0; j < rawPoints.Count; j++)
            {
                if (Vector2.Distance(points[i], rawPoints[j]) < 0.1)
                {
                    valid = false;
                    break;
                }
            }
            if (valid) rawPoints.Add(points[i]);
        }
        points = new Vector2[rawPoints.Count];
        points = rawPoints.ToArray();

        // update collider
        polygonCollider.SetPath(0, points);

        // update fill renderer
        shapeController.spline.Clear();
        for (int i = 0; i < points.Length; i++)
        {
            shapeController.spline.InsertPointAt(i, points[i]);
        }
        shapeRenderer.color = fillColor;

        // update border renderer; add extraneous points around sharp corners to fix line width issues
        List<Vector3> borderPoints = new();
        for (int i = 0; i < points.Length; i++)
        {
            borderPoints.Add(new Vector3(points[i].x, points[i].y, 0));
        }

        //if (borderQuality > 0 && points.Length > 2)
        //{
        //    int offset = 0;
        //    for (int i = 0; i < points.Length; i++)
        //    {
        //        Vector3 pivot = points[(i + 1) % points.Length];
        //        Vector3 current = ((Vector3) points[i]) - pivot;
        //        Vector3 next = ((Vector3) points[(i + 2) % points.Length]) - pivot;
        //        float dot = (current.x * next.x) + (current.y * next.y);
        //        if (dot <= 0) continue;  // angle formed by points isn't very sharp

        //        if (i + 1 == points.Length) offset = 0;

        //        for (int j = 0; j < borderQuality; j++)
        //        {
        //            next /= 2;
        //            current /= 2;
        //            int calculated = (offset + ((i + 1) % points.Length) + j);

        //            //int pivotIndex = borderPoints.IndexOf(pivot);
        //            //if (pivotIndex == -1) Debug.Log(calculated);
        //            //Debug.Log(calculated);
        //            borderPoints.Insert(calculated + 1, pivot + next);
        //            borderPoints.Insert(calculated, pivot + current);
        //        }
        //        offset += borderQuality * 2;
        //    }
        //}

        borderController.positionCount = borderPoints.Count;
        borderController.SetPositions(borderPoints.ToArray());
        borderController.startWidth = 1;
        borderController.endWidth = borderController.startWidth;
        //if (tileDelegate != null)
        //{
        //    borderController.startWidth = tileDelegate.getLineWidth();
        //    borderController.endWidth = borderController.startWidth;
        //}

        // update textbox shape
        int maxMisses = 10;
        float decayConstant = 2.8284f;
        float target = 1E-3f;
        int count = 0;

        Bounds bounds = polygonCollider.bounds;
        Vector2 center = polygonCollider.OverlapPoint(bounds.center) ? bounds.center : points[0];
        float accuracy = float.MaxValue;
        do
        {
            int misses = 0;
            while (misses < maxMisses)
            {
                Vector2 random = new Vector2(
                    UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                    UnityEngine.Random.Range(bounds.min.y, bounds.max.y)
                );
                if (borderDistance(random) > borderDistance(center) &&
                    random != center && polygonCollider.OverlapPoint(random))
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
        Debug.Log(count);

        text.rectTransform.position = (Vector3) center + transform.position;
        text.rectTransform.sizeDelta = borderDistance(center) * 1.5f * Vector2.one;
    }

    public void UpdateLabel()
    {
        if (isMine)
        {
            text.text = "*";
        }
        else
        {
            text.text = adjacentMines != 0 ? adjacentMines.ToString() : "";
        }
    }

    private float borderDistance(Vector2 p)
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

    private void Awake()
    {
        UpdateShape();
    }

    private Color applyAccent(Color color, Vector3 accent)
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

        shapeRenderer.color = applyAccent(fillColor, fillAccent);
        if (highlight != Highlight.None)
        {
            float highlightValue = 0.75f * (highlight == Highlight.Adjacent ? 0.5f : 1);
            highlight = Highlight.None;
            shapeRenderer.color = new Color(
                1 - (1 - shapeRenderer.color.r) * (1 - highlightValue),
                1 - (1 - shapeRenderer.color.g) * (1 - highlightValue),
                1 - (1 - shapeRenderer.color.b) * (1 - highlightValue)
            );
        }

        borderController.startColor = shapeRenderer.color;
        //borderController.startColor = applyAccent(borderColor, borderAccent);
        borderController.endColor = borderController.startColor;
    }

    void LateUpdate()
    {
        if (tileDelegate == null || tileDelegate.isPaused()) return;
        if (polygonCollider.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            if (Input.GetMouseButtonDown(0) ||
                (PlayerPrefs.GetString("Clear", "UNBOUND") != "UNBOUND" ?
                    Input.GetKeyDown((KeyCode) Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Clear", "C"))) :
                    false)
                )
            {
                // clear
                tileDelegate.click(gameObject.GetComponent<TileController>());
            }
            else if (Input.GetMouseButtonDown(1) ||
                (PlayerPrefs.GetString("Flag", "UNBOUND") != "UNBOUND" ?
                    Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Flag", "F"))) :
                    false)
                )
            {
                // flag
                tileDelegate.flag(gameObject.GetComponent<TileController>());
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
        borderAccent = Vector3.one * 1;
        borderController.sortingOrder = 1;
        text.enabled = true;
        cleared = true;
    }

    public void Flag()
    {
        fillAccent = new Vector3(1, -1, -1);
        borderAccent = Vector3.one * 1;
        borderController.sortingOrder = 2;
        flagged = true;
    }

    public void Unflag()
    {
        fillAccent = Vector3.zero;
        borderAccent = Vector3.zero;
        borderController.sortingOrder = 0;
        flagged = false;
    }

    public void ResetValues()
    {
        fillAccent = Vector3.zero;
        borderAccent = Vector3.zero;
        borderController.sortingOrder = 0;
        flagged = false;
        cleared = false;
        text.enabled = false;
        adjacentMines = 0;
        isMine = false;
        UpdateLabel();
    }
}
