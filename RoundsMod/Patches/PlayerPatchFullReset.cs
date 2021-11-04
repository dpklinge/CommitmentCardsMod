using System;
using HarmonyLib;
using CommitmentCards.Scripts;
using UnityEngine;

namespace CommitmentCards.Patches
{
    // patch to reset card effects
    [Serializable]
    [HarmonyPatch(typeof(Player), "FullReset")]
    class PlayerPatchFullReset
    {
        private static void Prefix(Player __instance)
        {
            if (__instance.GetComponent<ConstantFire>() != null) { UnityEngine.GameObject.Destroy(__instance.GetComponent<ConstantFire>()); }
        }
    }
}
