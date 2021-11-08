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
            CommitmentCards.Log("Looking to delete component");

            if (__instance.GetComponent<ConstantFire>() != null) { CommitmentCards.Log("Deleting component"); UnityEngine.GameObject.Destroy(__instance.GetComponent<ConstantFire>()); }
        }
    }
}
