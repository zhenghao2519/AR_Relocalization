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
//using System.Text.RegularExpressions;
//using Application = UnityEngine.Application;

public class ImageRecognizer : MonoBehaviour
{
    //[SerializeField]
    //private Button recognizeImageButton;
    public GameObject ball000;
    public GameObject cube100;
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
        Instantiate(ball000, Vector3.zero, Quaternion.Euler(Vector3.zero));

        trackImageManager = gameObject.AddComponent<ARTrackedImageManager>();

        trackImageManager.referenceLibrary = runtimeImageLibrary;
        //trackImageManager.referenceLibrary = trackImageManager.CreateRuntimeLibrary(runtimeImageLibrary);
        trackImageManager.requestedMaxNumberOfMovingImages = MaxNumberOfMovingImages;
        trackImageManager.trackedImagePrefab = presentObject;


        trackImageManager.enabled = true;

        //this read the the local json file of the placed cube
        StartCoroutine(LoadCube());

        trackImageManager.trackedImagesChanged += OnChanged;

        //recognizing images take possibly more than few frames, using startcorotine to handle this job
        String imgPath = Application.persistentDataPath + "/temp/";
        if (Directory.Exists(imgPath))
        {
            var imgFiles = Directory.GetFiles(imgPath, "*.jpg");
            foreach (string file in imgFiles)
            {
                //Debug.Log(file);
                StartCoroutine(LoadImage(file));
            }
        }
        

        scanResult.text = "No picture is detected" + cubePosition + cubeRotation;

        //recognizeImageButton.onClick.AddListener(() => StartCoroutine(LoadImage(imgPath)));

    }


    void OnDisable()
    {
        trackImageManager.trackedImagesChanged -= OnChanged;
    }
    //load the position and rotation(string) of the cube placed in image capture mode, and parse them into vector
    public IEnumerator LoadCube()
    {
        yield return null;
        scanResult.text = "start read";
        string cubeLocation = LoadJsontoString();
        scanResult.text = "getJson" + "|" + cubeLocation;
        cubePosition = Regex.Match(cubeLocation, @"\{\S*\}").Value;
        cubePosition = cubePosition.Substring(2, cubePosition.Length - 4);
        cubeRotation = Regex.Match(cubeLocation, @"\`\S*\`").Value;
        cubeRotation = cubeRotation.Substring(2, cubeRotation.Length - 4);
        scanResult.text = cubeLocation + "||" + cubePosition + "|" + cubeRotation + "Cube loaded";
        cubePositionVector = ParseVector3(cubePosition);
        cubeRotationVector = ParseVector3(cubeRotation);

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

        StartCoroutine(AddImageJob(t2d, imgPath));

    }




    public IEnumerator AddImageJob(Texture2D texture2D, string newName)
    {
        yield return null;

        //imageGuid refers to XRReferenceImage, textureGuid refers to texture in the AssetsDatabase 
        //var imageGuid = new SerializableGuid(0, 0);
        //var textureGuid = new SerializableGuid(0, 0);


        try
        {
            MutableRuntimeReferenceImageLibrary mutableRuntimeReferenceImageLibrary = trackImageManager.referenceLibrary as MutableRuntimeReferenceImageLibrary;
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

    //transform string into vector3
    public static Vector3 ParseVector3(string str)
    {
        string[] strs = str.Split(',');
        float x = float.Parse(strs[0]);
        float y = float.Parse(strs[1]);
        float z = float.Parse(strs[2]);
        return new Vector3(x, y, z);
    }

    public string LoadJsontoString()
    {
        string path = Application.persistentDataPath + "/temp/cube/cube.json";
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        int byteLength = (int)fs.Length;
        byte[] bytes = new byte[byteLength];
        fs.Read(bytes, 0, byteLength);
        fs.Close();
        fs.Dispose();
        string s = new UTF8Encoding().GetString(bytes);
        return s;
    }

    private IEnumerator UpdateOffset(ARTrackedImage trackedImage, string detectedLocation, string position, string rotation)
    //private IEnumerator UpdateOffset(string detectedLocation, string position, string rotation)
    {
        yield return new WaitForSeconds(1);
        //if tracked image is the first one being detected in this location
        if (detectedLocation != locationName)
        {
            locationName = detectedLocation;
            imageCounter = 1;
            deltaPostion = trackedImage.transform.position - ParseVector3(position);
            deltaRotation = trackedImage.transform.eulerAngles - ParseVector3(rotation);
            //deltaPostion = xrCamera.transform.position - ParseVector3(position);
            //deltaRotation = xrCamera.transform.eulerAngles - ParseVector3(rotation);
            scanResult.text = ParseVector3(position).ToString();
            newCubePosition = cubePositionVector + deltaPostion;
            newCubeRotation = cubeRotationVector + deltaRotation;

        }
        else
        //if tracked image belongs to the same detected location of the previous detected image,update cube position and rotation
        {
            imageCounter++;
            Vector3 currentDeltaPosition = trackedImage.transform.position - ParseVector3(position);
            Vector3 currentDeltaRotation = trackedImage.transform.eulerAngles - ParseVector3(rotation);
            //Vector3 currentDeltaPosition = xrCamera.transform.position - ParseVector3(position);
            //Vector3 currentDeltaRotation = xrCamera.transform.eulerAngles - ParseVector3(rotation);

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
                //showedObject = Instantiate(gameObjectToPlace, ParseVector3(cubePosition) + deltaPostion, Quaternion.Euler(ParseVector3(cubeRotation) + deltaRotation));
                //showedObject = Instantiate(cube100, Vector3.right + deltaPostion, Quaternion.Euler(Vector3.zero + deltaRotation));

                showedObject = Instantiate(cube100,newCubePosition,Quaternion.Euler(new Vector3(newCubeRotation.x, 0,0)));
                
            }
            else
            {
                //showedObject.transform.position = ParseVector3(cubePosition) + deltaPostion;
                //showedObject.transform.rotation = Quaternion.Euler(ParseVector3(cubeRotation) + deltaRotation);
                showedObject.transform.position = newCubePosition;
                showedObject.transform.rotation = Quaternion.Euler(new Vector3(newCubeRotation.x, 0, 0));
            }
            //scanResult.text = imageCounter + " pictures of " + locationName + "detected. Current offset is" + deltaPostion + deltaRotation;

            
            scanResult.text += imageCounter + " pictures of " + locationName + "detected. Current offset is" + newCubePosition + newCubeRotation;
        }
    }
    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
         
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            
            string detectedLocation = Regex.Match(trackedImage.referenceImage.name, @"\[\S*\]").Value;
            string position = Regex.Match(trackedImage.referenceImage.name, @"\{\S*\}").Value;
            //from {(xx,yy,zz)} to xx,yy,zz
            position = position.Substring(2, position.Length - 4);
            string rotation = Regex.Match(trackedImage.referenceImage.name, @"\`\S*\`").Value;
            //from '(xx,yy,zz)' to xx,yy,zz
            rotation = rotation.Substring(2, rotation.Length - 4);
            StartCoroutine(UpdateOffset(trackedImage, detectedLocation, position, rotation));
            
            

            // Display the name of the tracked image in the canvas
            //currentImageText.text = trackedImage.referenceImage.name;
            //trackedImage.transform.Rotate(Vector3.up, 180);
        }

        //foreach (ARTrackedImage trackedImage in eventArgs.updated)
        //{
        //}
    }
}

