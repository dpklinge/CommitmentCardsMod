using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ModdingUtils.Utils;

namespace CommitmentCards.Scripts
{
    class ConstantFire : MonoBehaviour
    {
        public Gun gun;
        void Update()
        {
            if (this.GetComponent<Player>()!=null && !PlayerStatus.PlayerAliveAndSimulated(this.GetComponent<Player>())) { return; }
            CommitmentCards.Log($"[{CommitmentCards.ModInitials}][MonoBehaviour] ConstantFire updating");
            if (gun.IsReady())
            {
                CommitmentCards.Log($"[{CommitmentCards.ModInitials}][MonoBehaviour] ConstantFire gun ready, attacking");
                gun.Attack(1);
            }
        }
    }
}
