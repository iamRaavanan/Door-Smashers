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
        public int nextStepEvent_ = 0;
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
            if (pathSteps_[0].eventBound_ == pBounds)
            {
                MoveToPosition(pathSteps_[0].targetPosition_);
                pathSteps_.RemoveAt(0);
            }
        }

        public void AddSteps (Vector3 pTargetPos)
        {
            float distance = 0f;
            Vector3 origin = transform.position;
            if (pathSteps_.Count > 0)
            {
                distance = Vector3.Distance(pTargetPos, pathSteps_[pathSteps_.Count - 1].targetPosition_);
                origin = pathSteps_[pathSteps_.Count - 1].targetPosition_;
            }
            if (pathSteps_.Count == 0 || distance > mAgent.stoppingDistance)
            {
                PathStep ps = new PathStep(pTargetPos, new NavMeshPath());
                if (NavMesh.CalculatePath(origin, pTargetPos, NavMesh.AllAreas, ps.path_))
                {
                    ps.eventBound_ = nextStepEvent_;
                    nextStepEvent_ = 0;
                    pathSteps_.Add(ps);
                    UpdatePathView();
                }
            }
        }

        private void UpdatePathView()
        {
            Vector3 offset = Vector3.up * 0.1f;
            List<Vector3> positions = new List<Vector3>();

            for (int i = 0; i < pathSteps_.Count; i++)
            {
                Vector3[] corners = pathSteps_[i].path_.corners;
                int length = corners.Length;
                for (int index = 0;  index < length; index++)
                {
                    positions.Add(corners[index]);
                    //mPathView.SetPosition(i, pathSteps_[i].targetPosition_ + offset);
                }
            }
            mPathView.positionCount = positions.Count;
            for (int i = 0; i < mPathView.positionCount; i++)
            {
                mPathView.SetPosition(i, positions[i] + offset);
            }
        }

        public void MoveToPosition (Vector3 targetPos_)
        {
            mAgent.SetDestination(targetPos_);
            mIsMovementActive = true;
        }
    }
}