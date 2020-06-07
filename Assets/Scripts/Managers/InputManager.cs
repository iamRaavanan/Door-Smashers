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
        public Transform mStepOverlayUI;
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
            if (currentSystem.IsPointerOverGameObject())
                return;
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000))
                {
                    UnitController InUnitController = hit.transform.GetComponent<UnitController>();
                    if (InUnitController != null)
                    {
                        mSelectedUnit = InUnitController;
                        mStepOverlayUI.gameObject.SetActive(false);
                    }
                    else
                    {
                        if (mSelectedUnit != null)
                        {
                            NavMeshHit nHit;
                            if (NavMesh.SamplePosition(hit.point, out nHit, 1, NavMesh.AllAreas))
                            {
                                mStepOverlayUI.transform.position = Input.mousePosition;
                                mStepOverlayUI.gameObject.SetActive(true);
                                mSelectedUnit.AddSteps(nHit.position);
                            }
                        }
                    }
                }
            }
        }

        public void ExecuteSteps (int pEventBound)
        {
            for (int i = 0; i < mPlayerUnits.Count; i++)
            {
                mPlayerUnits[i].transform.position += (Vector3.forward * 0.0001f).normalized;
                mPlayerUnits[i].ExecuteSteps(pEventBound);
            }
        }

        public void SetStepToEvent (int pEventbound)
        {
            if (mSelectedUnit != null)
            {
                GameObject GO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(GO.GetComponent<SphereCollider>());
                GO.transform.localScale = Vector3.one * 0.1f;
                GO.transform.position = mSelectedUnit.pathSteps_[mSelectedUnit.pathSteps_.Count - 1].targetPosition_ + Vector3.up;
                mSelectedUnit.pathSteps_[mSelectedUnit.pathSteps_.Count - 1].eventBound = pEventbound;
            }   
        }
    }
}
