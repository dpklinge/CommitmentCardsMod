using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CommitmentCards.Scripts
{
    class ConstantFire : MonoBehaviour
    {
        public Gun gun;
        void Update()
        {
            UnityEngine.Debug.Log($"[{CommitmentCards.ModInitials}][MonoBehaviour] ConstantFire updating");
            if (gun.IsReady())
            {
                UnityEngine.Debug.Log($"[{CommitmentCards.ModInitials}][MonoBehaviour] ConstantFire gun ready, attacking");
                gun.Attack(1);
            }
        }
    }
}
