using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ChangeScene : MonoBehaviour
{
    public void MoveToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        //Scene newScene = SceneManager.GetSceneByName(sceneName);
        //SceneManager.SetActiveScene(newScene);
    }
   
}
