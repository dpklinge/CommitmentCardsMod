using CommitmentCards.Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommitmentCards.Patches
{

    [Serializable]
    [HarmonyPatch(typeof(GunAmmo), "ReloadAmmo")]
    class OnReloadPatch
    {
        private static void Prefix(GunAmmo __instance)
        {
            CommitmentCards.Log("On reloading");
            var gun = __instance.GetComponentInParent<Gun>();
            
            if (gun && gun.GetAdditionalData().stacksOfMaxReload > 0)
            {
                var stacks = gun.GetAdditionalData().stacksOfMaxReload;
                gun.numberOfProjectiles += stacks;
                gun.spread += 0.1f;
            }
        }
    }
}
