
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
    class BattleOfTitans : CustomCard
    {

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            CommitmentCards.Log($"[{CommitmentCards.ModInitials}][Card] {GetTitle()} has been setup.");
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            foreach( Player otherPlayer in PlayerManager.instance.players)
            {
                CommitmentCards.Log("Increasing health of player: "+player.playerID);
                if (otherPlayer.playerID != player.playerID)
                {
                    otherPlayer.GetComponent<CharacterData>().maxHealth *= 4;
                    otherPlayer.GetComponent<CharacterStatModifiers>().sizeMultiplier *= .8f;
                }
            }
            data.maxHealth *= 7.5f;
            characterStats.sizeMultiplier *= .75f;
            
            //Edits values on player when card is selected
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            CommitmentCards.Log($"[{CommitmentCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");

            //Run when the card is removed from the player
        }

        protected override string GetTitle()
        {
            return "Battle of Titans";
        }
        protected override string GetDescription()
        {
            return "Tired of ending too quick? Make everyone a little tougher! And you the toughest, of course.";
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
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Other players health:",
                    amount = "+400%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Your health:",
                    amount = "+750%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
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
