using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Drawing;
using Unity.XR.CoreUtils;
//using Application = UnityEngine.Application;

public class CaptureImageRunTimeManager : MonoBehaviour
{
    public GameObject ball000;
    public GameObject cube100;
    [SerializeField]
    private Text hintsLog;
    
    [SerializeField]
    private Button captureImageButton;

    [SerializeField]
    private Button placeCubeButton;

    [SerializeField]
    private InputField inputField;

    private int imageCounter = 0;
    private Transform xrCamera;

    // Start is called before the first frame update
    void Start()
    {
        hintsLog.text = "Please first enter a location name! ";
        xrCamera = GameObject.Find("XR Origin").transform.Find("Camera Offset").Find("Main Camera");
        Instantiate(ball000,Vector3.zero, Quaternion.Euler(Vector3.zero));
        Instantiate(cube100, Vector3.right, Quaternion.Euler(Vector3.zero));
        captureImageButton.onClick.AddListener(() => StartCoroutine(CaptureImage()));
        
    }

  

    private IEnumerator CaptureImage()
    {
        yield return new WaitForEndOfFrame();
        imageCounter++;
        // Deactive the input field once the capture button is pressed
        inputField.gameObject.SetActive(false);
        var texture = ScreenCapture.CaptureScreenshotAsTexture();
        //The camera's local position in xr origin space.
        Vector3 imagePosition = xrCamera.position;
        Vector3 imageRotation = xrCamera.eulerAngles;
        //Save the jpeg with name in the form of
        //[LocationName]{ImagePosition}`ImageRotation`UniqueIdentifier
        string newName = "["+inputField.text+"]"+"{" + imagePosition.ToString() + "}" + "`" + imageRotation.ToString() + "`" + Guid.NewGuid().ToString();
        newName = newName.Replace(" ", "");
        //string newName = Guid.NewGuid().ToString();
        //XRReferenceImage newImage = new XRReferenceImage(imageGuid, textureGuid, new Vector2(0.1f, 0.1f), newName, texture2D);
        StartCoroutine(SaveImage(texture, newName));
        

    }


    public IEnumerator SaveImage(Texture2D texture2D, string newName)
    {   
        string path = Application.persistentDataPath + "/temp";
        yield return null;
        byte[] bytes = texture2D.EncodeToJPG();
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        FileStream file = File.Open(path + "/" + newName + ".jpg", FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(bytes);
        file.Close();
    }

     void Update()
    {
        if (imageCounter == 1)
        {
            hintsLog.text = "Take at least 3 pictures.";
        }
        if (imageCounter == 3)
        {
            hintsLog.text = "Enough pictures taken!";
            placeCubeButton.gameObject.SetActive(true);
            
        }
    }

}

