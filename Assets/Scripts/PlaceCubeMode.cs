using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceCubeModeButton : MonoBehaviour {
    [SerializeField]
    private Button captureImageButton;

    public void PlaceCubeMode() {
        captureImageButton.gameObject.SetActive(false);
        //this.gameObject.SetActive(false);
    }
}