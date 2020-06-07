using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Raavanan
{
    public class UnitController : MonoBehaviour
    {
        #region Public Variables
        public List<PathStep> pathSteps_ = new List<PathStep>();
        
        #endregion

        #region SerializedFields Variables
        [SerializeField] private bool mIsPlayer;
        [SerializeField] private LineRenderer mPathView;
        #endregion

        #region Private Variables
        private NavMeshAgent mAgent;
        
        private bool mIsMovementActive;

        private int mStepIndex;

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
                        if (pathSteps_.Count > 0)
                        {
                            mAgent.ResetPath();
                            ExecuteSteps(0);
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

        public void ExecuteSteps (int pBounds)
        {
            if (pathSteps_.Count == 0)
                return;
            if (pathSteps_[0].eventBound == pBounds)
            {
                MoveToPosition(pathSteps_[0].targetPosition_);
                pathSteps_.RemoveAt(0);
            }
        }

        public void AddSteps (Vector3 targetPos_)
        {
            float distance = 0f;
            if (pathSteps_.Count > 0)
                distance = Vector3.Distance(targetPos_, pathSteps_[pathSteps_.Count - 1].targetPosition_);
            if (pathSteps_.Count == 0 || distance > mAgent.stoppingDistance)
            {
                pathSteps_.Add(new PathStep(targetPos_));
                UpdatePathView();
            }
        }

        private void UpdatePathView()
        {
            Vector3 offset = Vector3.up * 0.1f;
            mPathView.positionCount = pathSteps_.Count;
            for (int i = 0; i < pathSteps_.Count; i++)
            {
                mPathView.SetPosition(i, pathSteps_[i].targetPosition_ + offset);
            }
        }

        public void MoveToPosition (Vector3 targetPos_)
        {
            mAgent.SetDestination(targetPos_);
            mIsMovementActive = true;
        }
    }
}