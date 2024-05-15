using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface MaskingWaveDelegate {
	public void MaskingWaveCompleted(MaskingWave wave);
}

public class TileAnimationController : MonoBehaviour, MaskingWaveDelegate
{
	private List<MaskingWave> clearingWaves;
	private List<int> availableLayers;
	private List<int> usedLayers;
	public GameObject clearingWave;
	
	public void ClearingAnimation(List<TileController> clearingGroup, Vector2 pos)
	{
		MaskingWave wave = Instantiate(
			clearingWave, 
			transform
		).GetComponent<MaskingWave>();
		wave.transform.position = pos;

		wave.Initialize(clearingGroup, FetchID(), this);

		clearingWaves.Add(wave);
	}
	
	// Use this for initialization
	void Awake()
	{
		clearingWaves = new();
		availableLayers = new();
		usedLayers = new();

	}

	// Update is called once per frame
	void Update()
	{
			
	}

	private int FetchID() {
		int id;
		if (availableLayers.Count > 0) {
			id = availableLayers[0];
			availableLayers.RemoveAt(0);
		} else {
			id = (availableLayers.Count + usedLayers.Count + 1) * 2;
		}
		usedLayers.Add(id);
		return id;
	}

    public void MaskingWaveCompleted(MaskingWave wave)
    {
        clearingWaves.Remove(wave);
		Destroy(wave.gameObject);
    }

	public void Reset() {
		foreach(MaskingWave wave in clearingWaves) {
			Destroy(wave.gameObject);
		}

		clearingWaves.Clear();
		availableLayers.Clear();
		usedLayers.Clear();
	}
}

