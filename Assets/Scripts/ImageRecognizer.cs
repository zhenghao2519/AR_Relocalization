using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using Unity.VisualScripting;
using static System.Net.WebRequestMethods;
//using System.Text.RegularExpressions;
//using Application = UnityEngine.Application;

public class ImageRecognizer : MonoBehaviour
{
    //[SerializeField]

    public GameObject ball_cood_origin;

    public GameObject gameobjectToPlace;
    [SerializeField]
    private Text scanResult;

    [SerializeField]
    private GameObject presentObject;

    [SerializeField]
    private XRReferenceImageLibrary runtimeImageLibrary;

    [SerializeField]
    private int MaxNumberOfMovingImages;

    //public GameObject gameObjectToPlace;


    private ARTrackedImageManager trackImageManager;
    private string locationName = null;
    private int imageCounter = 0;
    private Vector3 deltaPostion;
    private Vector3 deltaRotation;
    private Vector3 newCubePosition;
    private Vector3 newCubeRotation;
    private string cubePosition;
    private string cubeRotation;
    private Vector3 cubePositionVector;
    private Vector3 cubeRotationVector;
    private GameObject showedObject = null;

    //private Transform xrCamera;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(ball_cood_origin, Vector3.zero, Quaternion.Euler(Vector3.zero));

        trackImageManager = gameObject.AddComponent<ARTrackedImageManager>();

        trackImageManager.referenceLibrary = runtimeImageLibrary;

        trackImageManager.requestedMaxNumberOfMovingImages = MaxNumberOfMovingImages;
        trackImageManager.trackedImagePrefab = presentObject;


        trackImageManager.enabled = true;

        //this read the the local json file of the placed cube
        //StartCoroutine(LoadCube());

        trackImageManager.trackedImagesChanged += OnChanged;

        //recognizing images take possibly more than few frames, using startcorotine to handle this job
        String imgPath = Application.persistentDataPath + "/temp/";
        
        StartCoroutine(LoadImage(imgPath));

        // in our demo, we demonstrate a cube placed in capture mode with position (1,0,0) and rotation (0,0,0)
        cubePositionVector = Vector3.right;
        cubeRotationVector = Vector3.zero;

        scanResult.text = "No picture is detected";



    }


    void OnDisable()
    {
        trackImageManager.trackedImagesChanged -= OnChanged;
    }
    //load the position and rotation(string) of the cube placed in image capture mode, and parse them into vector
    //public IEnumerator LoadCube()
    //{
    //    yield return null;
    //    scanResult.text = "start read";
    //    string cubeLocation = LoadJsontoString();
    //    scanResult.text = "getJson" + "|" + cubeLocation;
    //    cubePosition = Regex.Match(cubeLocation, @"\{\S*\}").Value;
    //    cubePosition = cubePosition.Substring(2, cubePosition.Length - 4);
    //    cubeRotation = Regex.Match(cubeLocation, @"\`\S*\`").Value;
    //    cubeRotation = cubeRotation.Substring(2, cubeRotation.Length - 4);
    //    scanResult.text = cubeLocation + "||" + cubePosition + "|" + cubeRotation + "Cube loaded";
    //    cubePositionVector = ParseVector3(cubePosition);
    //    cubeRotationVector = ParseVector3(cubeRotation);

    //}


    // After this coroutine is called in start:
    // In the first frame it find a jpeg file in imgPath, start new coroutine AddImageJob to load this file, yield and wait for the next frame
    // In the following frame, we have only two coroutines to execute: the original LoadImage coroutine running a foreach loop and an AddImageJob from the LoadImage in the last frame
    public IEnumerator LoadImage(string imgPath)
    {
        yield return null;
        //if the directory ex
        if (Directory.Exists(imgPath)) {
            var imgFiles = Directory.GetFiles(imgPath, "*.jpg");
            foreach (string file in imgFiles) {
                FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                int byteLength = (int)fs.Length;
                byte[] imgBytes = new byte[byteLength];
                fs.Read(imgBytes, 0, byteLength);
                fs.Close();
                fs.Dispose();
                //convert to Texture2D
                Texture2D t2d = new Texture2D(4, 4);
                t2d.LoadImage(imgBytes);
                StartCoroutine(AddImageJob(t2d, file));
                yield return null;
            }
        }
    }




    public IEnumerator AddImageJob(Texture2D texture2D, string newName)
    {
        yield return null;


        try
        {
            MutableRuntimeReferenceImageLibrary mutableRuntimeReferenceImageLibrary = trackImageManager.referenceLibrary as MutableRuntimeReferenceImageLibrary;

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

    //transform string into vector3
    public static Vector3 ParseVector3(string str)
    {
        string[] strs = str.Split(',');
        float x = float.Parse(strs[0]);
        float y = float.Parse(strs[1]);
        float z = float.Parse(strs[2]);
        return new Vector3(x, y, z);
    }

    //public string LoadJsontoString()
    //{
    //    string path = Application.persistentDataPath + "/temp/cube/cube.json";
    //    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
    //    int byteLength = (int)fs.Length;
    //    byte[] bytes = new byte[byteLength];
    //    fs.Read(bytes, 0, byteLength);
    //    fs.Close();
    //    fs.Dispose();
    //    string s = new UTF8Encoding().GetString(bytes);
    //    return s;
    //}

    private IEnumerator UpdateOffset(ARTrackedImage trackedImage)
    {
        // wait the tracked image manager to set right position and rotation
        yield return new WaitForSeconds(1);

        //read positions and rotations from file name
        string detectedLocation = Regex.Match(trackedImage.referenceImage.name, @"\[\S*\]").Value;
        string position = Regex.Match(trackedImage.referenceImage.name, @"\{\S*\}").Value;
        //from {(xx,yy,zz)} to xx,yy,zz
        position = position.Substring(2, position.Length - 4);
        string rotation = Regex.Match(trackedImage.referenceImage.name, @"\`\S*\`").Value;
        //from '(xx,yy,zz)' to xx,yy,zz
        rotation = rotation.Substring(2, rotation.Length - 4);

        //if tracked image is the first one being detected in this location
        if (detectedLocation != locationName)
        {
            locationName = detectedLocation;
            imageCounter = 1;

            deltaPostion = trackedImage.transform.position - ParseVector3(position);
            deltaRotation = trackedImage.transform.eulerAngles - ParseVector3(rotation);

            newCubePosition = cubePositionVector + deltaPostion;
            newCubeRotation = cubeRotationVector + deltaRotation;

        }
        //if tracked image belongs to the same detected location of the previous detected image,update cube position and rotation
        else
        {
            imageCounter++;
            Vector3 currentDeltaPosition = trackedImage.transform.position - ParseVector3(position);
            Vector3 currentDeltaRotation = trackedImage.transform.eulerAngles - ParseVector3(rotation);

            //if more image detected, recalculate the offset (deltaPosition, deltaRotation) by choosing the average value
            deltaPostion = (deltaPostion * (imageCounter - 1) + currentDeltaPosition) / imageCounter;
            deltaRotation = (deltaRotation * (imageCounter - 1) + currentDeltaRotation) / imageCounter;
            newCubePosition = cubePositionVector + deltaPostion;
            newCubeRotation = cubeRotationVector + deltaRotation;

        }

        if (imageCounter >= 1)
        {
            if (showedObject == null)
            {

                showedObject = Instantiate(gameobjectToPlace, newCubePosition, Quaternion.Euler(newCubeRotation));
            }
            else
            {
                showedObject.transform.position = newCubePosition;
                showedObject.transform.rotation = Quaternion.Euler(newCubeRotation);
            }


            scanResult.text = imageCounter + " pictures of " + locationName + "detected.";
        }
    }
    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {

        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            StartCoroutine(UpdateOffset(trackedImage));
        }
    }
}

