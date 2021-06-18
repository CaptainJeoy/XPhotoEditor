using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Cartoonize : MonoBehaviour
{
	[SerializeField] private ImageData currentImageData;
	[SerializeField] private Slider cartoonizeSlider;

	List<Color> currPixels = new List<Color>();

	List<Color[]> CurrentlyQuantizedPixels = new List<Color[]>();

	public void Setup()
    {
		//SetPixelsArrayToPixelsList(currentImageData.ReturnProcessedPixels(), currPixels);
	}

	public void QuantizeImage()
	{
		//see log

        if (CurrentlyQuantizedPixels.Count <= 0)
        {
			//Debug.Log("Here?");
			SetPixelsArrayToPixelsList(currentImageData.ReturnProcessedPixels(), currPixels);
        }
        else if (CheckIfColorArraysEqual(CurrentlyQuantizedPixels, currentImageData.ReturnProcessedPixels()))
        {
			SetPixelsArrayToPixelsList(CurrentlyQuantizedPixels[0], currPixels);
		}
        else
        {
			//Debug.Log("Or Here?");
			SetPixelsArrayToPixelsList(currentImageData.ReturnProcessedPixels(), currPixels);
			CurrentlyQuantizedPixels.Clear();
			CurrentlyQuantizedPixels.Add(currPixels.ToArray());
		}

		if (currPixels.Count <= 0)
			return;

		if (cartoonizeSlider.value != 0f)
        {
			for (int i = 0; i < currPixels.Count; i++)
			{
				Color tempPixel = currPixels[i];

				tempPixel.r = QuantizeValue(currPixels[i].r, cartoonizeSlider.value);
				tempPixel.g = QuantizeValue(currPixels[i].g, cartoonizeSlider.value);
				tempPixel.b = QuantizeValue(currPixels[i].b, cartoonizeSlider.value);

				currPixels[i] = tempPixel;
			}
		}

		//SetPixelsListToPixelsArray(currPixels, defaultPixels);

		CurrentlyQuantizedPixels.Add(currPixels.ToArray());

		currentImageData.SetNewProcessedPixels(currPixels.ToArray());
		//currentImageData.UndoProcessedPixelsStack();
	}

	private float QuantizeValue(float x, float quantumNum = 2f)
	{
		if (x % quantumNum != 0f)
		{
			float mod = x % quantumNum;
			return (mod < (quantumNum - 0.5f)) ? x - mod : (x - mod) + quantumNum;
		}

		return x;
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

	private void SetPixelsListToPixelsArray(List<Color> pixelList, Color[] pixelArray)
	{
		if (pixelList == null)
			return;

		pixelArray = new Color[pixelList.Count];

		for (int i = 0; i < pixelArray.Length; i++)
		{
			pixelArray[i] = pixelList[i];
		}
	}

	private bool CheckIfColorArraysEqual(List<Color[]> list_A, Color[] b)
    {
		if (list_A.Count <= 0)
			return false;

		Color[] a = list_A[list_A.Count - 1];

		if (a.Length != b.Length)
			return false;

		bool ret = true;

        for (int i = 0; i < a.Length; i++)
        {
			if(!Color.Equals(a[i], b[i]))
            {
				ret = false;
				break;
            }
        }

		return ret;
    }
}
