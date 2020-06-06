using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raavanan
{
    [System.Serializable]
    public class PathStep
    {
        public Vector3 targetPosition_;

        public PathStep (Vector3 pTargetPosition)
        {
            targetPosition_ = pTargetPosition;
        }
    }
}