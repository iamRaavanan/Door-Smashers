using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Raavanan
{
    [System.Serializable]
    public class PathStep
    {
        public Vector3 targetPosition_;
        public int eventBound_;
        public NavMeshPath path_;
        public string debugOnPathReach = "None";
        public PathStep (Vector3 pTargetPosition, NavMeshPath pPath)
        {
            targetPosition_ = pTargetPosition;
            path_ = pPath;
        }
    }
}