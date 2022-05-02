
using CommitmentCards.Scripts;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using UnboundLib.Utils;
using System.Reflection;
using UnboundLib.Cards;
using UnityEngine;
using CommitmentCards.Extensions;
using HarmonyLib;

namespace CommitmentCards.Cards
{
    class ShockBlast : CustomCard
    {

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            CommitmentCards.Log($"[{CommitmentCards.ModInitials}][Card] {GetTitle()} has been setup.");
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            if (gun.GetAdditionalData().shockBlastBaseForce == 0)
            {
                gun.GetAdditionalData().shockBlastBaseForce += 10000;
                gun.GetAdditionalData().shockBlastRange += 2.5f;
            }
            else
            {
                gun.GetAdditionalData().shockBlastBaseForce += 5000;
                gun.GetAdditionalData().shockBlastRange += 1.75f;
            }
            ObjectsToSpawn shockBlast = new ObjectsToSpawn() { };
            shockBlast.AddToProjectile = new GameObject("ShockBlastSpawner", typeof(ShockBlastSpawner));
            CommitmentCards.Log("OBJECTSTOSPAWN at shockblast add");
            var spawnList = gun.objectsToSpawn.ToList<ObjectsToSpawn>();
            spawnList.ForEach(it => CommitmentCards.Log("Projectile: "+ it.AddToProjectile+" effect: "+it.effect));
            spawnList.Add(shockBlast);
            gun.objectsToSpawn = spawnList.ToArray();
            CommitmentCards.Log($"[{CommitmentCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");
            //Edits values on player when card is selected
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            CommitmentCards.Log($"[{CommitmentCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");

            //Run when the card is removed from the player
        }

        protected override string GetTitle()
        {
            return "Shock Blast";
        }
        protected override string GetDescription()
        {
            return "Replace your bullets with a shockwave that damages and repulses enemies! Damage and push scale with bullet damage, size scales with projectile speed. Now triggers many on-hit effects!";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]{
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DestructiveRed;
        }
        public override string GetModName()
        {
            return CommitmentCards.ModInitials;
        }
   
    }
}
