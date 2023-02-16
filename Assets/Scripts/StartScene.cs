using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    // Start is called before the first frame update
    [RuntimeInitializeOnLoadMethod]
    static void Intialize() {
        if (SceneManager.GetActiveScene().name == "StartScene") {
            return;
        }
        SceneManager.LoadScene("StartScene");
    }

}
