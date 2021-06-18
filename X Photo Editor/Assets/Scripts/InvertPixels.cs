using System.Collections.Generic;
using UnityEngine;

public class InvertPixels : MonoBehaviour
{
	[SerializeField] private ImageData currentImageData;

	List<Color> currPixels = new List<Color>();

	public void InvertImage()
	{
		SetPixelsArrayToPixelsList(currentImageData.ReturnProcessedPixels(), currPixels);

		if (currPixels.Count <= 0)
			return;

		for (int i = 0; i < currPixels.Count; i++)
		{
			Color tempPixel = currPixels[i];

			tempPixel.r = 1 - currPixels[i].r;
			tempPixel.g = 1 - currPixels[i].g;
			tempPixel.b = 1 - currPixels[i].b;

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