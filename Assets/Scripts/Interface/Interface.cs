using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raavanan
{
    public interface IClickable
    {
        void OnClick(InputManager pInputManager);
    }

    public interface ITarget
    {
        Transform OnTargetAcquired(UnitController pUnitController);
    }
}
