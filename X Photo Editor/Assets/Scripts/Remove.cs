using System.Collections.Generic;
using UnityEngine;

public class Remove : MonoBehaviour
{
	[SerializeField] private ImageData currentImageData;

	List<Color> currPixels = new List<Color>();

	public void RemoveBlackBackGround()
	{
		SetPixelsArrayToPixelsList(currentImageData.ReturnProcessedPixels(), currPixels);

		if (currPixels.Count <= 0)
			return;

		for (int i = 0; i < currPixels.Count; i++)
		{
			Color tempPixel = currPixels[i];

			if (tempPixel.r < 0.5f && tempPixel.g < 0.5f && tempPixel.b < 0.5f)
            {
				tempPixel.a = 0;
			}

			currPixels[i] = tempPixel;
		}

		currentImageData.SetNewProcessedPixels(currPixels.ToArray());
	}

	private void SetPixelsArrayToPixelsList(Color[] pixelArray, List<Color> pixelList)
	{
		if (pixelArray == null)
			return;

		pixelList.Clear();

		for (int i = 0; i < pixelArray.Length; i++)
		{
			pixelList.Add(pixelArray[i]);
		}
	}
}
