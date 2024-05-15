using UnityEngine;
using System.Collections;
using UnityEditor.UIElements;
using Unity.VisualScripting;

[ExecuteInEditMode]
public class FlagAnimator: MonoBehaviour
{
	public SpriteMask mask;
	private Vector3 originalPosition;
	private Vector3 originalScale;
	public float time;
	public float duration;
	private float offset;
	public bool isOpening;
	public bool isActive;

	public IFlagAnimatorDelegate flagDelegate;

	public void BeginAnimation(bool isOpening)
	{
        isActive = true;
        mask.enabled = true;
		this.isOpening = isOpening;
	}

    // Update is called once per frame
    void Update()
	{
		if (isActive)
		{
			time += Time.deltaTime * (isOpening ? 1 : -1);
            mask.transform.localScale = Vector3.Scale(originalScale, new Vector3(1, time / duration, 1));
            offset = mask.bounds.extents.y;
            mask.transform.localPosition = originalPosition + new Vector3(0, offset - originalScale.y / 2, 0);

			if (time > duration || time < 0)
			{
				time = time > duration ? duration : 0;
				mask.enabled = false;
				isActive = false;
				flagDelegate.AnimationCompleted(isOpening);
			}
        }
	}

	public void Resize(Vector3 scale, Vector3 position)
	{
		originalScale = scale;
		originalPosition = position;
	}

	public void ResetAnimation() {
		mask.transform.localScale = Vector3.zero;
		mask.transform.localPosition = originalPosition + 
				new Vector3(0, mask.bounds.extents.y - originalScale.y / 2, 0);
		time = 0;
		mask.enabled = false;
		isActive = false;
	}
}
