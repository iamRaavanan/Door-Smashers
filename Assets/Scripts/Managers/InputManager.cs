using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace Raavanan
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;
        public Transform stepOverlayUI;
        public UIOverlay uiOverlay;

        private GameObject mCurrentInteractionObj;
        private Vector3 mTargetUIOverlayPos;
        private UnitController mSelectedUnit;
        private List<UnitController> mPlayerUnits = new List<UnitController>();
        private void Awake()
        {
            instance = this;
        }

        public void RegisterUnits (UnitController pUnitController)
        {
            mPlayerUnits.Add(pUnitController);
        }

        private void Update()
        {
            EventSystem currentSystem = EventSystem.current;
            HandleOverlayUI();
            if (currentSystem.IsPointerOverGameObject())
                return;
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000))
                {
                    IClickable iClickable = hit.transform.GetComponent<IClickable>();
                    if (iClickable != null)
                    {
                        iClickable.OnClick(this);
                    }
                    else
                    {
                        if (mSelectedUnit != null)
                        {
                            uiOverlay.CloseInteractions();
                            NavMeshHit nHit;
                            if (NavMesh.SamplePosition(hit.point, out nHit, 1, NavMesh.AllAreas))
                            {
                                mTargetUIOverlayPos = Input.mousePosition;
                                stepOverlayUI.gameObject.SetActive(true);
                                mSelectedUnit.AddSteps(nHit.position);
                            }
                        }
                    }
                }
            }
        }

        public void AssignSelectedUnit (UnitController pTargetUnit)
        {
            if (mSelectedUnit)
                mSelectedUnit.DeHighLightSelected();
            mSelectedUnit = pTargetUnit;
            if (mSelectedUnit.pathSteps_.Count == 0)
            {
                stepOverlayUI.gameObject.SetActive(false);
            }
            else
            {
                mTargetUIOverlayPos = mSelectedUnit.pathSteps_[mSelectedUnit.pathSteps_.Count - 1].targetPosition_;
                stepOverlayUI.gameObject.SetActive(true);
            }
        }

        private void HandleOverlayUI ()
        {
            stepOverlayUI.transform.position = mTargetUIOverlayPos/*Input.mousePosition*/;
        }

        public void ExecuteSteps (int pEventBound)
        {
            for (int i = 0; i < mPlayerUnits.Count; i++)
            {
                mPlayerUnits[i].transform.position += (Vector3.forward * 0.0001f).normalized;
                mPlayerUnits[i].ExecuteSteps(pEventBound);
            }
        }

        public void OnClickInteration (GameObject pGO, InteractionSnapshot pIs)
        {
            if (mSelectedUnit != null)
                stepOverlayUI.gameObject.SetActive(true);
            mCurrentInteractionObj = pGO;
            pGO.GetComponent<BoxCollider>().enabled = false;
            mTargetUIOverlayPos = Input.mousePosition;
            uiOverlay.LoadSnapshot(pIs);
        }

        public void SetStepToEvent (int pEventbound)
        {
            Debug.Log("LLLL" + mSelectedUnit);
            if (mSelectedUnit != null)
            {
                GameObject GO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(GO.GetComponent<SphereCollider>());
                GO.transform.localScale = Vector3.one * 0.1f;
                GO.transform.position = mSelectedUnit.pathSteps_[mSelectedUnit.pathSteps_.Count - 1].targetPosition_ + Vector3.up;
                mSelectedUnit.nextStepEvent_ = pEventbound;
            }   
        }

        public void SetIntegration (int pIndex)
        {
            if (mSelectedUnit != null)  
            {
                //NavMeshHit nHit;
                //if (NavMesh.SamplePosition(mCurrentInteractionObj.transform.position, out nHit, 1, NavMesh.AllAreas))
                //{
                //    Debug.Log("Inside Navmesh SamplePosition " + mCurrentInteractionObj);
                //    mTargetUIOverlayPos = Input.mousePosition;
                //    stepOverlayUI.gameObject.SetActive(true);
                //    mSelectedUnit.AddSteps(nHit.position);
                //}
                mSelectedUnit.AddSteps(mCurrentInteractionObj.transform.position);
                GameObject GO = null;
                Debug.Log("mSelectedUnit : " + mSelectedUnit.pathSteps_.Count);
                PathStep pathStep = mSelectedUnit.pathSteps_[mSelectedUnit.pathSteps_.Count - 1];
                switch (pIndex)
                {
                    default:
                    case 0:
                        GO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        pathStep.debugOnPathReach = "kick";
                        break;
                    case 1:
                        GO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        pathStep.debugOnPathReach = "open";
                        break;
                    case 2:
                        GO = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        pathStep.debugOnPathReach = "flashbang";
                        break;
                }
                GO.transform.localScale = Vector3.one * 0.1f;
                GO.transform.position = mSelectedUnit.pathSteps_[mSelectedUnit.pathSteps_.Count - 1].targetPosition_ + Vector3.up;
                Destroy(GO.GetComponent<SphereCollider>());
            }
        }
    }
}
