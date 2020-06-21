using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Raavanan
{
    public class UnitController : MonoBehaviour, IClickable
    {
        #region Public Variables
        public List<PathStep> pathSteps_ = new List<PathStep>();
        public int nextStepEvent_ = 0;
        public bool isDead;
        #endregion

        #region SerializedFields Variables
        [SerializeField] private bool mIsPlayer;
        [SerializeField] private LineRenderer mPathView;
        [SerializeField] private GameObject mHighlighterGO;
        #endregion

        #region Private Variables
        private NavMeshAgent mAgent;
        private PathStep mCurrentStep;
        private bool mIsMovementActive;

        private UnitController mCurrentTarget;
        private float mFOVRadius = 20;
        private float mFOVAngle = 30;
        private float mfireRate = 1;
        private float mLastShot;
        private LayerMask mControllerLayer;
        private int mStepIndex;

        #endregion

        private void Start()
        {
            mAgent = GetComponent<NavMeshAgent>();
            gameObject.layer = 8;
            mControllerLayer = (1 << 8);
            if (mIsPlayer)
            {
                GameObject GO = Instantiate(mPathView.gameObject) as GameObject;
                mPathView = GO.GetComponent<LineRenderer>();
                InputManager.instance.RegisterUnits(this);
            }
        }

        private void Update()
        {
            if (isDead)
                return;
            HandleMovement();
            HandleDetection();
            HandleAttack();
        }

        private void HandleMovement()
        {
            if (mIsMovementActive)
            {
                if (mAgent.hasPath)
                {
                    if (mAgent.remainingDistance < mAgent.stoppingDistance)
                    {
                        mIsMovementActive = false;
                        Debug.Log(mCurrentStep.debugOnPathReach);
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
        }

        private void HandleDetection ()
        {
            if (mCurrentTarget == null)
            {
                Collider[] targets = Physics.OverlapSphere(transform.position, mFOVRadius, mControllerLayer);
                for (int i = 0; i < targets.Length; i++)
                {
                    UnitController controller = targets[i].GetComponent<UnitController>();
                    if (controller != null)
                    {
                        if (controller.isDead)
                            continue;
                        Vector3 dir = controller.transform.position - transform.position;
                        dir.Normalize();
                        float angle = Vector3.Angle(dir, transform.forward);
                        if (angle < mFOVAngle)
                        {
                            if (mIsPlayer)
                            {
                                if (!controller.mIsPlayer)
                                {
                                    CheckForOpponent(controller, dir);
                                }
                            }
                            else
                            {
                                if (controller.mIsPlayer)
                                {
                                    //CheckForOpponent(controller, dir);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void HandleAttack ()
        {
            if(mCurrentTarget != null)
            {
                mAgent.updateRotation = false;
                Vector3 dir = mCurrentTarget.transform.position - transform.position;
                dir.y = 0;
                dir.Normalize();
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 3);
                transform.rotation = targetRotation;

                float angle = Vector3.Angle(transform.forward, dir);
                if (angle < 5)
                {
                    if (Time.realtimeSinceStartup - mLastShot > mfireRate)
                    {
                        mLastShot = Time.realtimeSinceStartup;
                        int random = UnityEngine.Random.Range(0, 10);
                        if (random < 8)
                        {
                            mCurrentTarget.isDead = true;
                            mCurrentTarget.gameObject.SetActive(false);
                            mCurrentTarget = null;
                            mAgent.updateRotation = true;
                        }
                    }
                }
            }
        }
        private void CheckForOpponent(UnitController controller, Vector3 dir)
        {
            RaycastHit hit;
            Debug.DrawRay(transform.position + (Vector3.up * 0.5f), dir * 100);
            if (Physics.Raycast(transform.position + (Vector3.up * 0.5f), dir, out hit, 100))
            {
                UnitController newController = hit.transform.GetComponent<UnitController>();
                if (newController != null && !newController.mIsPlayer)
                {
                    mCurrentTarget = newController;
                }
            }
        }

        public void ExecuteSteps (int pBounds)
        {
            if (pathSteps_.Count == 0)
                return;
            if (pathSteps_[0].eventBound_ == pBounds)
            {
                mCurrentStep = pathSteps_[0];
                MoveToPosition(mCurrentStep.targetPosition_);
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

        public void DeHighLightSelected()
        {
            mHighlighterGO.SetActive(false);
        }

        public void OnClick(InputManager inputManager)
        {
            mHighlighterGO.SetActive(true);
            inputManager.AssignSelectedUnit(this);
        }
    }
}