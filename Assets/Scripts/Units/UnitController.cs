using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Raavanan
{
    public class UnitController : MonoBehaviour
    {
        #region Public Variables
        
        #endregion

        #region SerializedFields Variables
        [SerializeField] private bool mIsPlayer;
        [SerializeField] private LineRenderer mPathView;
        #endregion

        #region Private Variables
        private NavMeshAgent mAgent;
        
        private bool mIsMovementActive;

        private int mStepIndex;

        private List<PathStep> mPathSteps = new List<PathStep>();
        #endregion

        private void Start()
        {
            mAgent = GetComponent<NavMeshAgent>();
            if (mIsPlayer)
            {
                GameObject GO = Instantiate(mPathView.gameObject) as GameObject;
                mPathView = GO.GetComponent<LineRenderer>();
                InputManager.instance.RegisterUnits(this);
            }
            AddSteps(transform.position);
        }

        private void Update()
        {
            if (mIsMovementActive)
            {
                if (mAgent.hasPath)
                {
                    if (mAgent.remainingDistance < mAgent.stoppingDistance)
                    {
                        mIsMovementActive = false;
                        if (mPathSteps.Count > 0)
                        {
                            mAgent.ResetPath();
                            MoveToPosition(mPathSteps[0].targetPosition_);
                            mPathSteps.RemoveAt(0);
                        }
                    }
                }
                Vector3 dVelocity = mAgent.desiredVelocity;
                dVelocity = transform.InverseTransformVector(dVelocity);
            }
            else
            {

            }
        }

        public void ExecuteSteps ()
        {
            MoveToPosition(mPathSteps[0].targetPosition_);
            mPathSteps.RemoveAt(0);
        }

        public void AddSteps (Vector3 targetPos_)
        {
            float distance = 0f;
            if (mPathSteps.Count > 0)
                distance = Vector3.Distance(targetPos_, mPathSteps[mPathSteps.Count - 1].targetPosition_);
            if (mPathSteps.Count == 0 || distance > mAgent.stoppingDistance)
            {
                mPathSteps.Add(new PathStep(targetPos_));
                UpdatePathView();
            }
        }

        private void UpdatePathView()
        {
            Vector3 offset = Vector3.up * 0.1f;
            mPathView.positionCount = mPathSteps.Count;
            for (int i = 0; i < mPathSteps.Count; i++)
            {
                mPathView.SetPosition(i, mPathSteps[i].targetPosition_ + offset);
            }
        }

        public void MoveToPosition (Vector3 targetPos_)
        {
            mAgent.SetDestination(targetPos_);
            mIsMovementActive = true;
        }
    }
}