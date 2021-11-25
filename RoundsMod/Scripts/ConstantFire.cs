using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ModdingUtils.Utils;
using CommitmentCards.Extensions;

namespace CommitmentCards.Scripts
{
    class ConstantFire : MonoBehaviour
    {
        public Gun gun;
        void Update()
        {
            if (this.GetComponent<Player>()!=null && !PlayerStatus.PlayerAliveAndSimulated(this.GetComponent<Player>())) { return; }
            CommitmentCards.Log("Gun attack speed: " + gun.attackSpeed + " and multiplier: " + gun.attackSpeedMultiplier +" and damage "+gun.damage+" and damage mult "+gun.bulletDamageMultiplier);
            if (gun.attackSpeed < .15f)
            {
                var multiplier = 1 / gun.attackSpeed;
                var capMultiplier = 1 / 0.15f;
                gun.GetAdditionalData().storedSpeed += multiplier - capMultiplier;
                gun.attackSpeed = .15f;
            }
            if(gun.attackSpeed > 0.15f&& gun.GetAdditionalData().storedSpeed >=0)
            {
                var multiplier = 1 / gun.attackSpeed;
                var capMultiplier = 1 / 0.15f;
                var diff = capMultiplier - multiplier;
                if (diff <= gun.GetAdditionalData().storedSpeed)
                {
                    gun.attackSpeed = 0.15f;
                    gun.GetAdditionalData().storedSpeed -= diff;
                }
                else
                {
                    gun.attackSpeed = 1 / (capMultiplier - (diff - gun.GetAdditionalData().storedSpeed));
                    gun.GetAdditionalData().storedSpeed = 0;
                }
            }
            CommitmentCards.Log($"[{CommitmentCards.ModInitials}][MonoBehaviour] ConstantFire updating");
            if (gun.IsReady())
            {
                CommitmentCards.Log($"[{CommitmentCards.ModInitials}][MonoBehaviour] ConstantFire gun ready, attacking");
                gun.Attack(1);
            }
        }
    }
}
