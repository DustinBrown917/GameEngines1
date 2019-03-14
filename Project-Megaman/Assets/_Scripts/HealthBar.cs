using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MEGA
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image healthBarGraphic;
        // Start is called before the first frame update
        void Start()
        {
            PlayerController.Instance.DamageReceived += Player_DamageReceived;
        }

        private void Player_DamageReceived(object sender, PlayerController.DamageReceivedArgs e)
        {
            healthBarGraphic.fillAmount = e.percentHealthRemaining;
        }

    }
}

