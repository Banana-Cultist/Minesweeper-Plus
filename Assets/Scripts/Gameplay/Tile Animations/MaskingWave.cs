using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MaskingWave : MonoBehaviour
{
	private PriorityQueue<TileComparable> toClear;
	public SpriteMask mask;
	public float radius;
	public float expansionRate;

	private MaskingWaveDelegate maskingWaveDelegate;

	public void Initialize(List<TileController> toClear, int maskId, MaskingWaveDelegate maskingWaveDelegate) {
		this.toClear = new();
		this.maskingWaveDelegate = maskingWaveDelegate;

		foreach(TileController tile in toClear) {
			float maxDistance = 0;
			foreach(Vector2 point in tile.points) {
				float distance = Vector2.Distance(point, transform.position);
				if (distance > maxDistance) {
					maxDistance = distance;
				}
			}
			this.toClear.Insert(new TileComparable(tile, maxDistance));

			tile.PrepClearAnimation(maskId);
		}

		mask.frontSortingOrder = maskId + 2;
		mask.backSortingOrder = maskId;
	}

    public void Update()
    {
		if (toClear.Length() == 0)
        {
			maskingWaveDelegate.MaskingWaveCompleted(this);
			return;
        }

		// radius *= expansionRate;
		radius += expansionRate;
		mask.transform.localScale = Vector3.one * radius * 2.4f;
        while (toClear.Length() > 0 && toClear.Peak().GetDistance() < radius)
		{
			TileController tile = toClear.Pull().GetTile();
			tile.FinishClearAnimation();
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
			if (other.distance < this.distance) return 1;
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
