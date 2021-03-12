using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ARRuntimeImageDetection : MonoBehaviour
{
    public ARTrackedImageManager arTrackingImageManager;

    public ObjectToImage[] images;

    Dictionary<string, GameObject> goToImage = new Dictionary<string, GameObject>();

    Dictionary<string, GameObject> referenceImagesToGameObject = new Dictionary<string, GameObject>();
    // Dictionary<ARTrackedImage, GameObject> goToImage=new Dictionary<ARTrackedImage, GameObject>();

    [System.Serializable]
    public struct ObjectToImage {
        public Texture2D image;
        public GameObject @object;
    }

	private void OnValidate()
	{
        if (arTrackingImageManager == null)
        {
            arTrackingImageManager = GetComponent<ARTrackedImageManager>();
            arTrackingImageManager.enabled = false;
        }
    }


	private void Start()
	{
        OnValidate();

    }

	public void InitializeImagesLibrary()
    {
        Debug.Log(arTrackingImageManager+":"+ arTrackingImageManager.descriptor);
        if (arTrackingImageManager)
        {
            goToImage.Clear();
            MutableRuntimeReferenceImageLibrary runtimeImageLibrary = arTrackingImageManager.CreateRuntimeLibrary() as MutableRuntimeReferenceImageLibrary;

            if (runtimeImageLibrary == null)
            {
                Debug.LogError(gameObject.name + ":Library is nullable");
                return;
            }

            foreach (ObjectToImage item in images)
            {
                Debug.Log(item.image.name);
                goToImage.Add(item.image.name, item.@object);
                runtimeImageLibrary.ScheduleAddImageJob(item.image, item.image.name, item.image.width / 1000f);// Первая ошибка, размеры текстуры я делил на цельный int от чего оно округлялось до ближайшего цельного числа, к примеру "201f/1000 = 0" а вот "201f/1000f = 0.201" не забывайте про логику хранения чисел в памяти компьютера
            }

            arTrackingImageManager.referenceLibrary = runtimeImageLibrary;
            arTrackingImageManager.maxNumberOfMovingImages = 2;
            arTrackingImageManager.trackedImagesChanged += ArTrackingImageManager_trackedImagesChanged;
            arTrackingImageManager.enabled = true;
        }
        else
        {
            Debug.LogError("Not Supports Mutable Library");
        }
    }

    private void ArTrackingImageManager_trackedImagesChanged(ARTrackedImagesChangedEventArgs objList)
	{
        if (objList.added.Count>0){
            foreach(ARTrackedImage item in objList.added){
                referenceImagesToGameObject.Add(item.referenceImage.name, Instantiate(goToImage[item.referenceImage.name], item.transform));
            }
        }
        if (objList.removed.Count > 0)
        {
            GameObject goOut;
            foreach (ARTrackedImage item in objList.removed)
            {
                if (referenceImagesToGameObject.TryGetValue(item.referenceImage.name, out goOut))
                {
                    Destroy(goOut);
                }
            }
        }
        /*if (objList.updated.Count > 0)
        {
            GameObject goOut;
            foreach (ARTrackedImage item in objList.removed)
            {
                if (referenceImagesToGameObject.TryGetValue(item.referenceImage.name, out goOut))
                {
                    goOut.transform.position = item.transform.position;
                    goOut.transform.rotation = item.transform.rotation;
                }
            }
        }*/
    }
}
