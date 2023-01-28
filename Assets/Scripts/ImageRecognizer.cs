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

public class ImageRecognizer : MonoBehaviour
{
    //[SerializeField]
    //private Button recognizeImageButton;

    [SerializeField]
    private GameObject presentObject;

    [SerializeField]
    private XRReferenceImageLibrary runtimeImageLibrary1;

    [SerializeField]
    private int MaxNumberOfMovingImages;


    private ARTrackedImageManager trackImageManager1;

    // Start is called before the first frame update
    void Start()
    {


        //trackImageManager = gameObject.GetComponent<ARTrackedImageManager>();
        trackImageManager1 = gameObject.AddComponent<ARTrackedImageManager>();

        trackImageManager1.referenceLibrary = runtimeImageLibrary1;
        //trackImageManager.referenceLibrary = trackImageManager.CreateRuntimeLibrary(runtimeImageLibrary);
        trackImageManager1.requestedMaxNumberOfMovingImages = MaxNumberOfMovingImages;
        trackImageManager1.trackedImagePrefab = presentObject;


        trackImageManager1.enabled = true;
        trackImageManager1.trackedImagesChanged += OnChanged;

        String imgPath = Application.persistentDataPath + "/temp/";
        if (!Directory.Exists(imgPath))
            Directory.CreateDirectory(imgPath);
        var imgFiles = Directory.GetFiles(imgPath, "*.jpg");
        foreach (string file in imgFiles)
        {
            //Debug.Log(file);
            StartCoroutine(LoadImage(file));
        }

        //capturing images take possibly more than few frames, using startcorotine to handle this job
        //recognizeImageButton.onClick.AddListener(() => StartCoroutine(LoadImage(imgPath)));

    }

    public IEnumerator LoadImage(string imgPath)
    {
        yield return null;
        FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
        int byteLength = (int)fs.Length;
        byte[] imgBytes = new byte[byteLength];
        fs.Read(imgBytes, 0, byteLength);
        fs.Close();
        fs.Dispose();
        //convert to Texture2D
        Texture2D t2d = new Texture2D(4, 4);
        t2d.LoadImage(imgBytes);
        string newName = Guid.NewGuid().ToString();
        StartCoroutine(AddImageJob(t2d, newName));

    }

    void OnDisable()
    {
        trackImageManager1.trackedImagesChanged -= OnChanged;
    }


    public IEnumerator AddImageJob(Texture2D texture2D, string newName)
    {
        yield return null;

        //imageGuid refers to XRReferenceImage, textureGuid refers to texture in the AssetsDatabase 
        //var imageGuid = new SerializableGuid(0, 0);
        //var textureGuid = new SerializableGuid(0, 0);


        try
        {
            MutableRuntimeReferenceImageLibrary mutableRuntimeReferenceImageLibrary = trackImageManager1.referenceLibrary as MutableRuntimeReferenceImageLibrary;

            //String path = Application.persistentDataPath + "/temp";
            //var files = Directory.GetFiles(path);
            //foreach (Texture2D pics in files) 
            //{
            //    mutableRuntimeReferenceImageLibrary.ScheduleAddImageWithValidationJob(texture2D, Guid.NewGuid().ToString(), 0.1f);
            //}

            var jobHandle = mutableRuntimeReferenceImageLibrary.ScheduleAddImageWithValidationJob(texture2D, newName, 0.1f);

        }
        catch (Exception e)
        {
            if (texture2D == null)
            {
                Debug.Log("texture is null");
            }
            Debug.Log("Error:" + e.ToString());
        }

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

