using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageTracking : MonoBehaviour
{
    private ARTrackedImageManager imageManager;

    private void Awake()
    {
        imageManager = FindObjectOfType<ARTrackedImageManager>();
    }

    public void OnEnable()
    {
        imageManager.trackedImagesChanged += OnChanged;
    }

    public void OnDisable()
    {
        imageManager.trackedImagesChanged -= OnChanged;
    }

    public void OnChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.added){
            Debug.Log(trackedImage.name);
        }
    }
}
