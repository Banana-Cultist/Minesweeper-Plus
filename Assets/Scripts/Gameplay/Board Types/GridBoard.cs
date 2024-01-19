using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class GridBoard
{
	public static List<TileController> initialize(IBoardTypeDelegate board, int columns, int rows, RectTransform bounds, float offsetAngle, int offsetTile) {
		offsetAngle = Mathf.Clamp(offsetAngle, 0, 180);
		List<TileController> tiles = new List<TileController>();
        Hashtable tilePoints = new Hashtable();

        float dx = bounds.rect.width / columns;
		float dy = bounds.rect.height / rows;
		float dw = -Mathf.Tan(Mathf.Deg2Rad * (offsetAngle + 90)) * dy;
		Vector2 cursor = Vector2.zero;
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < columns; j++)
			{
				cursor.x += dx;
                TileController tile = board.CreateTile();

                tile.points = new Vector2[4] {
					bounds.rect.min + new Vector2(cursor.x, cursor.y),
                    bounds.rect.min + new Vector2(cursor.x + dx, cursor.y),
                    bounds.rect.min + new Vector2(cursor.x + dx + dw, cursor.y + dy),
                    bounds.rect.min + new Vector2(cursor.x + dw, cursor.y + dy)
                };
                tile.fillColor = Color.HSVToRGB(0, 0, 0.25f);
                //Color.HSVToRGB(Random.Range(0f, 1f), 1, 0.3f);
                tile.borderColor = Color.black;
                tiles.Add(tile);

                tile.UpdateShape();
                foreach (Vector2 point in tile.points) {
                    if (tilePoints.Contains(point))
                    {
                        (tilePoints[point] as List<TileController>).Add(tile);
                    }
                    else
                    {
                        tilePoints.Add(point, new List<TileController> { tile });
                    }
                }
			}
			cursor.x -= dx * (columns - offsetTile) - dw;
			cursor.y += dy;
        }

        foreach (TileController tile in tiles)
        {
            foreach (Vector2 point in tile.points)
            {
                foreach (TileController neighbor in (tilePoints[point] as List<TileController>))
                {
                    if (!neighbor.Equals(tile))
                    {
                        tile.adjacents.Add(neighbor);
                    }
                }
            }
        }

        return tiles;
	}
}
