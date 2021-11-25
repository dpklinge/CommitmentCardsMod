
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
            gun.GetAdditionalData().shockBlastBaseForce += 20000;
            gun.GetAdditionalData().shockBlastRange += 2;
            ObjectsToSpawn shockBlast = new ObjectsToSpawn() { };
            shockBlast.AddToProjectile = new GameObject("ShockBlastSpawner", typeof(ShockBlastSpawner));
            gun.objectsToSpawn = new ObjectsToSpawn[] { shockBlast };
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
            return "Replace your bullets with a shockwave that damages and repulses enemies! Damage and size scale with bullet damage and projectile speed.";
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
