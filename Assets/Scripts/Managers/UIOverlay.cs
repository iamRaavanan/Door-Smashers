using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raavanan
{
    public class UIOverlay : MonoBehaviour
    {
        public GameObject open;
        public GameObject kick;
        public GameObject flashbang;

        public void LoadSnapshot (InteractionSnapshot pIs)
        {
            open.SetActive(pIs.canOpen);
            kick.SetActive(pIs.canKick);
            flashbang.SetActive(pIs.canFlashbang);
        }

        public void CloseInteractions ()
        {
            open.SetActive(false);
            kick.SetActive(false);
            flashbang.SetActive(false);
        }
    }
}
