using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Raavanan
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;
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
                    }
                    else
                    {
                        if (mSelectedUnit != null)
                        {
                            NavMeshHit nHit;
                            if (NavMesh.SamplePosition(hit.point, out nHit, 1, NavMesh.AllAreas))
                            {
                                mSelectedUnit.AddSteps(nHit.position);
                            }
                        }
                    }
                }
            }
        }

        public void ExecuteSteps ()
        {
            for (int i = 0; i < mPlayerUnits.Count; i++)
            {
                mPlayerUnits[i].ExecuteSteps();
            }
        }
    }
}
