using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class AR_PutObject : MonoBehaviour
{
	public ARRaycastManager arRaycastManager;
    public ARPlaneManager planeManager;
	public GameObject prefabObjectSpawn;

    public void PutObject(){
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
		CreateObjectInPosition(GetRayPosition(ray));
	}

	public void PutObjectByScreenPoint(Vector3 screenCoordinates){
		Ray ray = Camera.main.ScreenPointToRay(screenCoordinates);
		CreateObjectInPosition(GetRayPosition(ray));
	}

	void CreateObjectInPosition(Vector3 worldPosition){
		Vector3 cameraForward = Camera.main.transform.forward;
		cameraForward.y = 0;
		cameraForward.Normalize();
		Instantiate(prefabObjectSpawn, worldPosition, Quaternion.LookRotation(cameraForward));
	}

	public Vector3 GetRayPosition(Ray ray){
		List<ARRaycastHit> hits = new List<ARRaycastHit>();
		if (arRaycastManager.Raycast(ray,hits,TrackableType.Planes)){
			return ray.GetPoint(hits[0].distance);
		}
		else
			return Vector3.zero;
	}

	private void Update()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay(ray.origin,ray.direction);
	}

}
