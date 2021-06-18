using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vignette : MonoBehaviour
{
    [SerializeField] private ImageData imageData;
    [SerializeField] private Slider adjustSlider;

    Color[,] currPixelMatrix;

    List<Color> currPixelList = new List<Color>();
    List<Color[,]> CurrentlyVignettedPixels = new List<Color[,]>();

    int imageWidth, imageHeight, counter = 0;

    private float interpolateValue(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    private float inverseInterpolateValue(float v, float vMin, float vMax)
    {
        return (v - vMin) / (vMax - vMin);
    }

    private float MapToValues(float v, float vMin, float vMax, float outMin, float outMax)
    {
        return interpolateValue(outMin, outMax, inverseInterpolateValue(v, vMin, vMax));
    }

    public void ApplyVignette()
    {
        imageData.EvaluateCurrentImageDimension(out imageWidth, out imageHeight);

        currPixelMatrix = imageData.PixelListToMatrix();

        for (int i = 0; i < imageWidth; i++)
        {
            for (int j = 0; j < imageHeight; j++)
            {
                Vector2 centre = new Vector2(imageWidth / 2f, imageHeight / 2f);
                Vector2 currPixelVec = new Vector2(i, j);

                float distance = Vector2.Distance(currPixelVec, centre);

                float scale = MapToValues(distance, 0f, adjustSlider.value, 1f, 0f);

                currPixelMatrix[i, j].r *= scale;
                currPixelMatrix[i, j].g *= scale;
                currPixelMatrix[i, j].b *= scale;
            }
        }

        //CurrentlyVignettedPixels.Add(currPixelMatrix);

        SetPixelMatrixToList(currPixelMatrix, imageWidth, imageHeight, currPixelList);

        imageData.SetNewProcessedPixels(currPixelList.ToArray());
    }

    private void SetPixelMatrixToAnotherPixelMatrix(Color[,] a, Color[,] b, int width, int height)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                b[i, j] = a[i, j];
            }
        }
    }

    private void SetPixelMatrixToList(Color[,] pixMat, int width, int height, List<Color> pixList)
    {
        pixList.Clear();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                pixList.Add(pixMat[i, j]);
            }
        }
    }

    private void SetPixelsArrayToPixelMatrix(Color[] pixelArray, Color[,] pixMat, int width, int height)
    {
        if (pixelArray == null)
            return;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                pixMat[i, j] = pixelArray[counter];

                counter++;
            }
        }

        counter = 0;
    }

    private bool CheckIfColorArraysEqual(List<Color[,]> list_A, Color[] b, int width, int height)
    {
        if (list_A.Count <= 0)
            return false;

        Color[,] a = list_A[list_A.Count - 1];

        if (a.Length != b.Length)
            return false;

        bool ret = true;

        List<Color> c = new List<Color>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                c.Add(a[i, j]);
            }
        }

        for (int i = 0; i < b.Length; i++)
        {
            if (!Color.Equals(c[i], b[i]))
            {
                ret = false;
                break;
            }
        }

        return ret;
    }

    /*
       currPixelMatrix = new Color[imageWidth, imageHeight];

       if (CurrentlyVignettedPixels.Count <= 0)
       {
           Debug.Log("Here!");
           currPixelMatrix = new Color[imageWidth, imageHeight];
           SetPixelsArrayToPixelMatrix(imageData.ReturnProcessedPixels(), currPixelMatrix, imageWidth, imageHeight);
       }
       else if (CheckIfColorArraysEqual(CurrentlyVignettedPixels, imageData.ReturnProcessedPixels(), imageWidth, imageHeight))
       {
           Debug.Log("Or Here!");
           currPixelMatrix = new Color[imageWidth, imageHeight];
           SetPixelMatrixToAnotherPixelMatrix(CurrentlyVignettedPixels[0], currPixelMatrix, imageWidth, imageHeight);
       }
       else
       {
           Debug.Log("Or Or Here!");
           currPixelMatrix = new Color[imageWidth, imageHeight];
           SetPixelsArrayToPixelMatrix(imageData.ReturnProcessedPixels(), currPixelMatrix, imageWidth, imageHeight);
           CurrentlyVignettedPixels.Clear();
           CurrentlyVignettedPixels.Add(currPixelMatrix);
       }

       if (currPixelMatrix.Length <= 0)
           return;
       */
}
