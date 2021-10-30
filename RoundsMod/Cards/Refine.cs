using CommitmentCards.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace CommitmentCards.Cards
{
    class Refine : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            UnityEngine.Debug.Log($"[{CommitmentCards.ModInitials}][Card] {GetTitle()} has been setup.");
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            HandManipulator.instance.RemoveRandomCard(player, 1);
            CommitmentCards.instance.ExecuteAfterSeconds(0.1f, () =>
            {
                HandManipulator.instance.DuplicateRandomCard(player, 2);
            });
            CommitmentCards.instance.ExecuteAfterSeconds(0.2f, () => {
                UnityEngine.Debug.Log($"[{CommitmentCards.ModInitials}][Card] {GetTitle()} Removing self.");
                HandManipulator.instance.RemoveCardType(player, ModdingUtils.Utils.Cards.instance.GetCardWithName(GetTitle()));
                UnityEngine.Debug.Log($"[{CommitmentCards.ModInitials}][Card] {GetTitle()} Self removed.");

            });
            UnityEngine.Debug.Log($"[{CommitmentCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");
            //Edits values on player when card is selected
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            UnityEngine.Debug.Log($"[{CommitmentCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");

            //Run when the card is removed from the player
        }

        protected override string GetTitle()
        {
            return "Refine";
        }
        protected override string GetDescription()
        {
            return "Removes one card with stats from your hand, and creates two copies of another. (Better have at least two!)";
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
            return new CardInfoStat[]{};
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.FirepowerYellow;
        }
        public override string GetModName()
        {
            return CommitmentCards.ModInitials;
        }
    }
}
