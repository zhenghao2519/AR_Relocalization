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
using System.Text;
//using Application = UnityEngine.Application;

public class CaptureImageRunTimeManager : MonoBehaviour
{
    public GameObject gameObjectToPlace;
    private GameObject spawnedObject;
    private string path;
    private Vector3 cubePosition;
    private Vector3 cubeRotation;
    //public GameObject ball000;
    //public GameObject cube100;
    [SerializeField]
    private Text hintsLog;
    
    [SerializeField]
    private Button captureImageButton;

    [SerializeField]
    //private Button placeCubeButton;
    private Button goToMenuButton;

    [SerializeField]
    private InputField inputField;

    private int imageCounter = 0;
    private Transform xrCamera;

    // Start is called before the first frame update
    void Start()
    {
        hintsLog.text = "Please first enter a location name! ";
        xrCamera = GameObject.Find("XR Origin").transform.Find("Camera Offset").Find("Main Camera");
        //Instantiate(ball000,Vector3.zero, Quaternion.Euler(Vector3.zero));
        //Instantiate(cube100, Vector3.right, Quaternion.Euler(Vector3.zero));
        path = Application.persistentDataPath + "/temp/cube";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
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
        cubePosition = (cubePosition + imagePosition) / 2;
        cubeRotation = (cubeRotation + imageRotation)   / 2;    
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
            hintsLog.text = "Take at least 2 more pictures.";
        }
        if (imageCounter == 2)
        {
            hintsLog.text = "Take at least 1 more picture.";
        }
        if (imageCounter == 3)
        {
            hintsLog.text = "Enough pictures taken!";
            //placeCubeButton.gameObject.SetActive(true);
            spawnedObject = Instantiate(gameObjectToPlace, cubePosition, Quaternion.Euler(cubeRotation));
            StartCoroutine(SaveJson());
            goToMenuButton.gameObject.SetActive(true);
            
        }
    }


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
    public IEnumerator SaveJson()
    {
        yield return null;
        //string json = JsonUtility.ToJson(spawnedObject.transform);
        string json = "{" + spawnedObject.transform.position.ToString() + "}" + "`" + spawnedObject.transform.eulerAngles.ToString() + "`";
        json = json.Replace(" ", "");
        //File.Create(path + "/cube.txt");
        //File.WriteAllText(path + "/cube.txt", json, System.Text.Encoding.UTF8);
        FileStream fs = new FileStream(path + "/cube.json", FileMode.Create);
        byte[] bytes = new UTF8Encoding().GetBytes(json.ToString());
        fs.Write(bytes, 0, bytes.Length);
        fs.Close();
        //String path = Application.persistentDataPath + "/temp";
        //yield return null;
        //if (!File.Exists(path + "/cubeJson.txt"))
        //    File.Create(path + "/cubeJson.txt");
        //FileStream file = File.Open(path + "/cubeJson.txt", FileMode.Open);
        //StreamWriter writer = new StreamWriter(file);
        //writer.Write(json);
        //file.Close();
    }

}

