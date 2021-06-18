using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct Dimension
{
    public int Height;
    public int Width;

    public float AspectRatio()
    {
        return (float) Width / (float) Height;
    }
}

[CreateAssetMenu(fileName = "ImageData")]
public class ImageData : ScriptableObject
{
    [HideInInspector] public Color[] LoadedPixels;

    public Dimension ImageDimension;

    private Stack<Color[]> ProcessedPixelsStack = new Stack<Color[]>();
    private Stack<Color[]> UndoneProcessedPixelsStack = new Stack<Color[]>();

    private Color[,] savedMatrix;

    private int keeper = 0;

    public void LoadImageProperties(Texture2D loadedImage)
    {
        LoadedPixels = loadedImage.GetPixels();

        ImageDimension.Height = loadedImage.height;
        ImageDimension.Width = loadedImage.width;

        ProcessedPixelsStack.Push(LoadedPixels);
    }

    public void EvaluateCurrentImageDimension(out int width, out int height)
    {
        Texture2D currTex = PhotoManager.Instance.GetLoadedTexture();

        width = ImageDimension.Width = currTex.width;
        height = ImageDimension.Height = currTex.height;
    }

    public Color[,] PixelListToMatrix()
    {
        if (ProcessedPixelsStack.Count <= 0)
            return null;

        //if (keeper <= 0)
        //{
            Texture2D currTex = PhotoManager.Instance.GetLoadedTexture();

            Color[] currPixel = currTex.GetPixels();

            int width = currTex.width;
            int height = currTex.height;

            int index = 0;

            Color[,] pixelMatrix = new Color[width, height];

            //savedMatrix = new Color[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    pixelMatrix[i, j] = currPixel[index];
                    //savedMatrix[i, j] = currPixel[index];
                    index++;
                }
            }

            keeper++;

            return pixelMatrix;
        //}
        //else
        //{
           // return savedMatrix;
       // }
    }

    public Color[] ReturnProcessedPixels()
    {
        return (ProcessedPixelsStack.Count > 0) ? ProcessedPixelsStack.Peek() : null;
    }

    public void SetNewProcessedPixels(Color[] pixels)
    {
        ProcessedPixelsStack.Push(pixels);

        PhotoManager.Instance.SetTexturePixels(ReturnProcessedPixels());
    }

    public void UndoProcessedPixelsStack()
    {
        if (ProcessedPixelsStack.Count <= 1)
            return;

        UndoneProcessedPixelsStack.Push(ProcessedPixelsStack.Pop());
    }

    public void RedoProcessedPixelsStack()
    {
        if (UndoneProcessedPixelsStack.Count <= 0)
            return;

        ProcessedPixelsStack.Push(UndoneProcessedPixelsStack.Pop());
    }

    public void SetUndonePixels()
    {
        UndoProcessedPixelsStack();

        PhotoManager.Instance.SetTexturePixels(ReturnProcessedPixels());
    }

    public void SetRedonePixels()
    {
        RedoProcessedPixelsStack();

        PhotoManager.Instance.SetTexturePixels(ReturnProcessedPixels());
    }
}
