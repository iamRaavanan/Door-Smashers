using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raavanan
{
    public class DoorHandler : MonoBehaviour, IClickable
    {
        public InteractionSnapshot interactionSnapshot;
        public void OnClick(InputManager inputManager)
        {
            inputManager.OnClickInteration(this.gameObject, interactionSnapshot);
        }
    }
}
