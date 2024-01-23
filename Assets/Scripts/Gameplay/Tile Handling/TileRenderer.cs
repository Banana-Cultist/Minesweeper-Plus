using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using static TileController;

public class TileRenderer : MonoBehaviour {    
    public PolygonCollider2D polygonCollider;

    public SpriteShapeRenderer coverRenderer;
    public SpriteShapeController coverController;

    public SpriteShapeRenderer fillRenderer;
    public SpriteShapeController fillController;

    public LineRenderer borderController;
    public int borderQuality;

    private Color coverColor;
    private Color fillColor;
    private Color borderColor;
    
    public void SetPoints(Vector2[] points)
    {
        // Adjust collider
        polygonCollider.SetPath(0, points);

        // Adjust sprite shape controllers
        coverController.spline.Clear();
        fillController.spline.Clear();
        for (int i = 0; i < points.Length; i++)
        {
            coverController.spline.InsertPointAt(i, points[i]);
            fillController.spline.InsertPointAt(i, points[i]);
        }

        // Adjust border
        List<Vector3> borderPoints = GetBorderPoints(points, borderQuality);
        borderController.positionCount = borderPoints.Count;
        borderController.SetPositions(borderPoints.ToArray());
    }

    // made to fix an issue with border width
    private List<Vector3> GetBorderPoints(Vector2[] points, int borderQuality)
    {
        List<Vector3> borderPoints = new();
        for (int i = 0; i < points.Length; i++)
        {
            borderPoints.Add(new Vector3(points[i].x, points[i].y, 0));
        }

        if (borderQuality <= 0 || points.Length <= 2)
        {
            return borderPoints;
        }

        int offset = 0;
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 pivot = points[(i + 1) % points.Length];
            Vector3 current = ((Vector3)points[i]) - pivot;
            Vector3 next = ((Vector3)points[(i + 2) % points.Length]) - pivot;
            float dot = (current.x * next.x) + (current.y * next.y);
            if (dot <= 0) continue;  // angle formed by points isn't very sharp

            if (i + 1 == points.Length) offset = 0;

            int insertion = offset + ((i + 1) % points.Length);
            for (int j = 0; j < borderQuality; j++)
            {
                next /= 2;
                current /= 2;
                
                //int pivotIndex = borderPoints.IndexOf(pivot);
                //if (pivotIndex == -1) Debug.Log(calculated);
                //Debug.Log(calculated);
                borderPoints.Insert(insertion + 1, pivot + next);
                borderPoints.Insert(insertion, pivot + current);
                insertion++;
            }
            offset += borderQuality * 2;
        }
        

        return borderPoints;
    }

    public void SetCoverColor(Color color)
    {
        coverColor = color;
        coverRenderer.color = coverColor;
    }

    public void SetFillColor(Color color)
    {
        fillColor = color;
        fillRenderer.color = fillColor;
    }

    public void SetBorderColor(Color color)
    {
        borderColor = color;
        borderController.startColor = borderColor;
        borderController.endColor = borderColor;
    }

    public void SetHighlight(TileController.Highlight highlight)
    {
        switch(highlight)
        {
            case TileController.Highlight.Selected:
            case TileController.Highlight.Adjacent:
                float highlightValue = 0.75f * (highlight == Highlight.Adjacent ? 0.5f : 1);
                fillRenderer.color = GetHighlightColor(fillColor, highlightValue);
                coverRenderer.color = GetHighlightColor(coverColor, highlightValue);
                borderController.startColor = Color.white;
                borderController.endColor = borderController.startColor;
                borderController.sortingOrder = 1;
                break;
            case TileController.Highlight.None:
                fillRenderer.color = fillColor;
                coverRenderer.color = coverColor;
                borderController.startColor = borderColor;
                borderController.endColor = borderController.startColor;
                borderController.sortingOrder = 0;
                break;
        }
    }

    private Color GetHighlightColor(Color color, float highlightValue)
    {
        return new Color(
            1 - (1 - color.r) * (1 - highlightValue),
            1 - (1 - color.g) * (1 - highlightValue),
            1 - (1 - color.b) * (1 - highlightValue)
        );
    }

    public void SetBorderWidth(float width)
    {
        borderController.startWidth = width;
        borderController.endWidth = width;
    }

    public Bounds GetBounds()
    {
        return polygonCollider.bounds;
    }

    public bool Contains(Vector2 point)
    {
        return polygonCollider.OverlapPoint(point);
    }
}
