using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Drawing;
//using Application = UnityEngine.Application;

public class CaptureImageRunTimeManager : MonoBehaviour
{
    [SerializeField]
    private Button captureImageButton;

    [SerializeField]
    private GameObject presentObject;

    [SerializeField]
    private XRReferenceImageLibrary runtimeImageLibrary;

    [SerializeField]
    private int MaxNumberOfMovingImages;

    
    private ARTrackedImageManager trackImageManager;

    // Start is called before the first frame update
    void Start()
    {
        

        //trackImageManager = gameObject.GetComponent<ARTrackedImageManager>();
        trackImageManager =  gameObject.AddComponent<ARTrackedImageManager>();
        
        trackImageManager.referenceLibrary = runtimeImageLibrary;
        //trackImageManager.referenceLibrary = trackImageManager.CreateRuntimeLibrary(runtimeImageLibrary);
        trackImageManager.requestedMaxNumberOfMovingImages = MaxNumberOfMovingImages;
        trackImageManager.trackedImagePrefab = presentObject;


        trackImageManager.enabled = true;
        trackImageManager.trackedImagesChanged += OnChanged;
        //capturing images take possibly more than few frames, using startcorotine to handle this job
        captureImageButton.onClick.AddListener(() => StartCoroutine(CaptureImage()));
        
    }

   
    
    void OnDisable()
    {
        trackImageManager.trackedImagesChanged -= OnChanged;
    }

    private IEnumerator CaptureImage()
    {
        yield return new WaitForEndOfFrame();

        var texture = ScreenCapture.CaptureScreenshotAsTexture();

        string newName = Guid.NewGuid().ToString();
        //XRReferenceImage newImage = new XRReferenceImage(imageGuid, textureGuid, new Vector2(0.1f, 0.1f), newName, texture2D);
        StartCoroutine(SaveImage(texture, newName));
    }

    
    public IEnumerator SaveImage(Texture2D texture2D, string newName)
    {
        String path = Application.persistentDataPath + "/temp";
        yield return null;
        byte[] bytes = texture2D.EncodeToJPG();
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        FileStream file = File.Open(path + "/" + newName + ".jpg", FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(bytes);
        file.Close();

        Debug.Log(path);
    }

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            // Display the name of the tracked image in the canvas
            //currentImageText.text = trackedImage.referenceImage.name;
            //trackedImage.transform.Rotate(Vector3.up, 180);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            // Display the name of the tracked image in the canvas
            //currentImageText.text = trackedImage.referenceImage.name;
            trackedImage.transform.Rotate(Vector3.up, 180);
        }
    }
}

