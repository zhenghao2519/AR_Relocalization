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


public class CaptureImageRunTimeManager : MonoBehaviour
{
    public GameObject gameObjectToPlace;
    private GameObject cube100;
    private string path;
    private Vector3 cubePosition;
    private Vector3 cubeRotation;
    public GameObject ball_cood_origin;

    [SerializeField]
    private Text hintsLog;

    [SerializeField]
    private Button captureImageButton;

    [SerializeField]

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
        Instantiate(ball_cood_origin, Vector3.zero, Quaternion.Euler(Vector3.zero));
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

        cubePosition = Vector3.right;
        cubeRotation = Vector3.zero;


        //Save the jpeg with name in the form of
        //[LocationName]{ImagePosition}`ImageRotation`UniqueIdentifier
        string newName = "[" + inputField.text + "]" + "{" + imagePosition.ToString() + "}" + "`" + imageRotation.ToString() + "`" + Guid.NewGuid().ToString();
        newName = newName.Replace(" ", "");

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
            hintsLog.text = "Take at least 4 more pictures.";
        }
        if (imageCounter == 2)
        {
            hintsLog.text = "Take at least 3 more picture.";
        }
        if (imageCounter == 3)
        {
            hintsLog.text = "Take at least 2 more picture.";
        }
        if (imageCounter == 4)
        {
            hintsLog.text = "Take at least 1 more picture.";
        }
        if (imageCounter == 5)
        {
            hintsLog.text = "Enough pictures taken!";

            cube100 = Instantiate(gameObjectToPlace, cubePosition, Quaternion.Euler(new Vector3(cubeRotation.x, 0, 0)));
            //StartCoroutine(SaveJson());
            goToMenuButton.gameObject.SetActive(true);
        }
    }


    //public static Vector3 ParseVector3(string str)
    //{
    //    string[] strs = str.Split(',');
    //    float x = float.Parse(strs[0]);
    //    float y = float.Parse(strs[1]);
    //    float z = float.Parse(strs[2]);
    //    return new Vector3(x, y, z);
    //}

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
    //public IEnumerator SaveJson()
    //{
    //    yield return null;

    //    string json = "{" + spawnedObject.transform.position.ToString() + "}" + "`" + spawnedObject.transform.eulerAngles.ToString() + "`";
    //    json = json.Replace(" ", "");

    //    FileStream fs = new FileStream(path + "/cube.json", FileMode.Create);
    //    byte[] bytes = new UTF8Encoding().GetBytes(json.ToString());
    //    fs.Write(bytes, 0, bytes.Length);
    //    fs.Close();

    //}

}

