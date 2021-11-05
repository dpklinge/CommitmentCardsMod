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
    class Copy : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            CommitmentCards.Log($"[{CommitmentCards.ModInitials}][Card] {GetTitle()} has been setup.");
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            CommitmentCards.Log($"[{CommitmentCards.ModInitials}][Card] {GetTitle()} OnAddCard called.");
            var card = HandManipulator.instance.DuplicateRandomCard(player, 1);
            CommitmentCards.Log($"[{CommitmentCards.ModInitials}][Card] {GetTitle()} OnAddCard has completed DuplicateRandomCard with card  {card}.");
            CommitmentCards.instance.ExecuteAfterSeconds(0.1f,  () => {
                CommitmentCards.Log($"[{CommitmentCards.ModInitials}][Card] {GetTitle()} Removing self.");
                HandManipulator.instance.RemoveCardType(player, ModdingUtils.Utils.Cards.instance.GetCardWithName(GetTitle()));
                CommitmentCards.Log($"[{CommitmentCards.ModInitials}][Card] {GetTitle()} Self removed.");

            });
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
            return "Copy";
        }
        protected override string GetDescription()
        {
            return "Creates a copy of a random card with stats in your hand. No viable targets? Have a consolation prize!";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
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
