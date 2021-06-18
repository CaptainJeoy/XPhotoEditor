using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformPixels : MonoBehaviour
{
	/*
     * contains funtions for Cropping, Rotating and Flippling
     */

	[SerializeField] private ImageData currentImageData;

	List<Color> currPixels = new List<Color>();
	List<Color> TransPixels = new List<Color>();

	int width, height, counter = 0;

	Color[,] currentPixelArray;

	public void RotateImage()
	{
		//To properly rotate, reset also the width and the size of the texture;

		SetPixelsArrayToPixelsList(currentImageData.ReturnProcessedPixels(), currPixels);
		currentImageData.EvaluateCurrentImageDimension(out width, out height);

		currentPixelArray = new Color[width, height];

		if (currPixels.Count <= 0)
			return;

        while (counter < currPixels.Count)
        {
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					currentPixelArray[i, j] = currPixels[width + height];
					counter++;
				}
			}
		}

        /*
		currPixels.Clear();

		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				currPixels.Add(currentPixelArray[i, j]);
			}
		}*/

        for (int i = currPixels.Count - 1; i >= 0; i--)
        {
			TransPixels.Add(currPixels[i]);
		}

		currentImageData.SetNewProcessedPixels(TransPixels.ToArray());
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
