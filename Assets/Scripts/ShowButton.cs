using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowButton : MonoBehaviour
{
    [SerializeField]
    private Button button;
    public void ShowTheButton()
    {
        button.gameObject.SetActive(true);
    }

}
