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
    private Text hintsLog;

    [SerializeField]
    private Button captureImageButton;

    [SerializeField]
    private Button goToStartMenuButton;

    [SerializeField]
    private InputField inputField;

    private int imageCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        hintsLog.text = "please enter a name for the location and then take at least 3 pictures for it.";
        captureImageButton.onClick.AddListener(() => StartCoroutine(CaptureImage()));
        
    }

  

    private IEnumerator CaptureImage()
    {
        yield return new WaitForEndOfFrame();

        var texture = ScreenCapture.CaptureScreenshotAsTexture();

        string newName = "["+inputField.text+"]"+Guid.NewGuid().ToString();
        //XRReferenceImage newImage = new XRReferenceImage(imageGuid, textureGuid, new Vector2(0.1f, 0.1f), newName, texture2D);
        StartCoroutine(SaveImage(texture, newName));
        imageCounter++;

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
    }

     void Update()
    {
        Debug.Log(imageCounter);
        if (imageCounter == 3)
        {
            hintsLog.text = "Enough pictures taken!";
            goToStartMenuButton.gameObject.SetActive(true);
        }
    }

}

