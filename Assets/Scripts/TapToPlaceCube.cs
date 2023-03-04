using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class TapToPlaceCube : MonoBehaviour
{

    public GameObject gameObjectToPlace;


    private GameObject spawnedObject;
    private ARRaycastManager arRaycastManager;
    private Vector2 touchPosition;
    private string path;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    // Start is called before the first frame update

    [SerializeField]
    private Text scanResult;

    void Start()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        path = Application.persistentDataPath + "/temp/cube";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        touchPosition = default;
        return false;
    }
    // Update is called once per frame
    void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }

        if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;

            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(gameObjectToPlace, hitPose.position, hitPose.rotation);


                scanResult.text = spawnedObject.transform.position.ToString();
                StartCoroutine(SaveJson());
                //} else {
                //    spawnedObject.transform.position = hitPose.position;
            }

        }
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
