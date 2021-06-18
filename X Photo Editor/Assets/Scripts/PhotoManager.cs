using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System;
using SFB;

public class PhotoManager : MonoBehaviour
{
	public static PhotoManager Instance;

	[SerializeField] private RawImage SampleImage;
	[SerializeField] private GameObject FileTypePanel;

	[SerializeField] private LayerMask ProcessingUILayer;
	[SerializeField] private ImageData CurrentImageData;

	private Texture2D sampleImgTex;

	private string path;

	Color[] currPixels, defaultPixels;

	Vector3 prevPointerPos, deltaPointerPos;
	Vector2 DefaultSampleImageSize;

	bool hideState = false;

	/*
	Vector2 DefaultImageParentSize;
	private AspectRatioFitter aRF;
	[SerializeField] private RectTransform ImageParent;
	*/

	private void Awake()
    {
		Instance = this;
		DefaultSampleImageSize = SampleImage.rectTransform.sizeDelta;

		//DefaultImageParentSize = ImageParent.sizeDelta;
	}

	#region OpenImage
	public void OpenImage()
	{

#if UNITY_EDITOR
		path = EditorUtility.OpenFilePanel("Open Image", "", "jpg,png,jpeg,tif,tiff,bmp");
#else
		ExtensionFilter[] extensions = new[] {new ExtensionFilter("Image Files", "png", "jpg", "jpeg" )};
		string[] paths = StandaloneFileBrowser.OpenFilePanel("Open Image", "", extensions, true);
		path = (paths.Length == 0) ? null : paths[0];
#endif

		if (path != null)
		{
			StartCoroutine(LoadTexture());
		}
	}

	private IEnumerator LoadTexture()
    {
		UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture("file:///" + path);

		yield return webRequest.SendWebRequest();

        if (!webRequest.isNetworkError && !webRequest.isHttpError)
        {
			Texture2D texBuffer = DownloadHandlerTexture.GetContent(webRequest);

			AdjustDestinationAspectRatio(texBuffer);

			SampleImage.texture = texBuffer;
			sampleImgTex = (Texture2D)SampleImage.texture;

			currPixels = sampleImgTex.GetPixels();
			defaultPixels = sampleImgTex.GetPixels();

			CurrentImageData.LoadImageProperties(sampleImgTex);
		}
	}
	#endregion 

	#region SaveImage
    public void SaveAsPNG()
	{
		ExtensionFilter[] extensionList = new[] { new ExtensionFilter("Image Files", "png") };

		SaveImage(sampleImgTex.EncodeToPNG(), extensionList);
	}

	public void SaveAsJPG()
	{
		ExtensionFilter[] extensionList = new[] { new ExtensionFilter("Image Files", "jpg") };

		SaveImage(sampleImgTex.EncodeToJPG(), extensionList);
	}

	private void SaveImage(byte[] encodedBytes, ExtensionFilter[] fileExtensions)
	{
		//string directory = Application.dataPath + "/../../NewImages/";

		string currTimestamp = DateTime.Now.ToString("yyyymmddhhssfff");

		string path = StandaloneFileBrowser.SaveFilePanel("Save Image", "", "NewImage_" + currTimestamp, fileExtensions);

		//if (!Directory.Exists(directory))
		//Directory.CreateDirectory(directory);

		//File.WriteAllBytes(directory + "NewImage_" + currTimestamp + fileExtension, encodedBytes);

		if (path != "")
			File.WriteAllBytes(path, encodedBytes);
	}
#endregion

	private void AdjustDestinationAspectRatio(Texture texBuffer)
    {
		//New and more simplified method is to just calculate the right ratio
		//of the texture to the ratio of the current sizeDelta of the RawImage UI
		//given the current Screen Resolution. NB: Anchor will have to be non stretch

		float newSampleImageWidth = ((float)texBuffer.width / Screen.width) * DefaultSampleImageSize.x;
		float newSampleImageHeight = ((float)texBuffer.height / Screen.height) * DefaultSampleImageSize.y;

		SampleImage.rectTransform.sizeDelta = new Vector2(newSampleImageWidth, newSampleImageHeight);

		/*
		//Old Method of resizing was to have a Parent RectTrans hold the Raw Image
		//Then add an AspectRatioFilter to the Raw Image while doing the below to it
		//...................................................................
		//Resize the Raw Image to fit the texture;
		aRF = SampleImage.GetComponent<AspectRatioFitter>();
		float texBufferAspect = (float)texBuffer.width / (float)texBuffer.height;

		aRF.aspectRatio = texBufferAspect;

		//Resize the Image Parent to fit the Texture
		float newParentWidth = ((float)texBuffer.width / Screen.width) * DefaultImageParentSize.x;
		float newParentHeight = ((float)texBuffer.height / Screen.height) * DefaultImageParentSize.y;

		ImageParent.sizeDelta = new Vector2(newParentWidth, newParentHeight);
		*/
	}

	public void ResetImage()
	{
		sampleImgTex.SetPixels(defaultPixels);
		sampleImgTex.Apply();

		SetPixelsToPixels(defaultPixels, currPixels);
	}

	public void SetTexturePixels(Color[] processedPixels)
    {
		if (processedPixels == null)
			return;

		sampleImgTex.SetPixels(processedPixels);
		sampleImgTex.Apply();
    }

	public Texture2D GetLoadedTexture()
    {
		return sampleImgTex;
    }

	public void Show_Hide_Panel()
    {
		hideState = !hideState;
		FileTypePanel.SetActive(hideState);
    }

	private void ScaleImportedImage(float inputAxis)
    {
		Vector2 scale = SampleImage.rectTransform.localScale;

		scale.x -= inputAxis;
		scale.y -= inputAxis;

        if (scale.x <= 0f)
			scale.x = scale.y = 0;

		SampleImage.rectTransform.localScale = scale;

		//ImageParent.localScale = scale;
	}

	void OnGUI()
	{
		if (Event.current.type == EventType.ScrollWheel)
			ScaleImportedImage(Event.current.delta.y * Time.deltaTime * 7f);
	}

	public void UndoImageProcessing()
    {
		CurrentImageData.SetUndonePixels();
    }

	public void RedoImageProcessing()
    {
		CurrentImageData.SetRedonePixels();
    }

	private void Update()
    {
		PanImageOnScreen();
	}

	private void PanImageOnScreen()
	{
		Rect imageScreenRect = GetScreenCoord(SampleImage.rectTransform);

		if (imageScreenRect.Contains(Input.mousePosition))
        {
			if (Input.GetMouseButtonDown(0))
			{
				prevPointerPos = Input.mousePosition;
			}

			if (Input.GetMouseButton(0) && !CanvasGraphicsRaycaster.Instance.IsPointerOverUI((Vector2)Input.mousePosition))
			{
				deltaPointerPos = prevPointerPos - Input.mousePosition;

				Vector3 temp = SampleImage.rectTransform.position;
				temp.x -= deltaPointerPos.x;
				temp.y -= deltaPointerPos.y;
				SampleImage.rectTransform.position = temp;

				/*
				Vector3 temp = ImageParent.position;
				temp.x -= deltaPointerPos.x;
				temp.y -= deltaPointerPos.y;
				ImageParent.position = temp;
				*/
			}

			prevPointerPos = Input.mousePosition;
		}

		prevPointerPos = Input.mousePosition;
	}

	private Rect GetScreenCoord(RectTransform uiTrans)
	{
		Vector2 screenRectSize = uiTrans.rect.size * uiTrans.lossyScale;

		Vector2 screenPivotPos = screenRectSize * uiTrans.pivot;

		Vector2 screenRectPos = (Vector2)uiTrans.position - screenPivotPos;

		return new Rect(screenRectPos, screenRectSize);
	}

	private void SetPixelsToPixels(Color[] oldPixels, Color[] newPixels)
	{
		for (int i = 0; i < newPixels.Length; i++)
		{
			newPixels[i] = oldPixels[i];
		}
	}

    private void OnApplicationQuit()
	{
		if (SampleImage.texture == null)
			return;

		ResetImage();
	}
}
