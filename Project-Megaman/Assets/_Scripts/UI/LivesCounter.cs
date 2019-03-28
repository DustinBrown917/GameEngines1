using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MEGA
{
    public class LivesCounter : MonoBehaviour
    {
        [SerializeField] private Text text;

        // Start is called before the first frame update
        void Start()
        {
            PlayerController.Instance.LivesChanged += Player_LivesChanged;
            text.text = "x " + PlayerController.Instance.LivesRemaining.ToString();
        }

        private void Player_LivesChanged(object sender, PlayerController.LivesChangedArgs e)
        {
            text.text = "x " + PlayerController.Instance.LivesRemaining.ToString();
        }
    }
}

