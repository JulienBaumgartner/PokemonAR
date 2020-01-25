#if UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;

public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject objectToPlace;
    public GameObject placementIndicator;
    private ARSessionOrigin arOrigin;
    private Pose placementPose;
    private bool isPlacementValid = false;
    private bool isActive = true;

    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
    }


    void Update()
    {
        if (!isActive)
        {
            placementIndicator.SetActive(false);
            return;
        }

        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (isPlacementValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }
    }

    private void PlaceObject()
    {
        GameObject plane = Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        isActive = false;
        GameObject.FindObjectOfType<GameManager>().PlanePlaced(plane);
    }

    private void UpdatePlacementIndicator()
    {
        if (isPlacementValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f,0.5f));
        var hits = new List<ARRaycastHit>();
        arOrigin.GetComponent<ARRaycastManager>().Raycast(screenCenter, hits, TrackableType.All);

        isPlacementValid = hits.Count > 0;

        if (isPlacementValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;

            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}
#endif
