using UnityEngine;
using System.Collections;
using System;

public class MaskingWave : MonoBehaviour
{
	private PriorityQueue<TileComparable> toClear;
	public SpriteMask mask;
	public float radius = 1;
	public float expansionRate = 1.1F;

	public MaskingWave(TileController[] toClear, Vector2 position)
	{
		this.toClear = new();
		foreach (TileController tile in toClear)
		{
			this.toClear.Insert(new TileComparable(tile, tile.MaxDistance(position)));
		}
	}

    public void Update()
    {
		radius *= expansionRate;
		mask.transform.localScale = Vector3.one * radius;
        if (toClear.Length() == 0)
        {
			
        }
        while (toClear.Peak().GetDistance() < radius)
		{
			TileController tile = toClear.Pull().GetTile();
		}
    }

    class TileComparable : IComparable
    {
        TileController tile;
        float distance;

		public TileComparable(TileController tile, float distance)
		{
			this.tile = tile;
			this.distance = distance;
		}

        public int CompareTo(object obj)
        {
			if (obj == null) return 1;
			TileComparable other = obj as TileComparable;
			if (other.distance == this.distance) return 0;
			if (other.distance > this.distance) return 1;
			return -1;
        }

		public float GetDistance()
		{
			return distance;
		}

		public TileController GetTile()
		{
			return tile;
		}
    }
}
