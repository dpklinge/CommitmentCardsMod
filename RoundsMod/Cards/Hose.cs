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
    class Hose : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            gun.attackSpeed *= 0.5f;
            gun.damage *= .05f;
            gun.reloadTime = .01f;
            gun.projectileSpeed *= .66f;
            UnityEngine.Debug.Log($"[{CommitmentCards.ModInitials}][Card] {GetTitle()} has been setup.");
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var constantFire = player.gameObject.AddComponent<ConstantFire>();
            constantFire.gun = gun;
            UnityEngine.Debug.Log($"[{CommitmentCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");
            //Edits values on player when card is selected
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            Destroy(player.gameObject.GetComponent<ConstantFire>());
            UnityEngine.Debug.Log($"[{CommitmentCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");
            //Run when the card is removed from the player
        }

        protected override string GetTitle()
        {
            return "Hose";
        }
        protected override string GetDescription()
        {
            return "Constantly spray a stream of weak bullets";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Bullet speed",
                    amount = "-33%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Damage",
                    amount = "-90%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                 new CardInfoStat()
                {
                    positive = true,
                    stat = "Attack speed",
                    amount = "+50%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                 new CardInfoStat()
                {
                    positive = true,
                    stat = "Reload speed",
                    amount = "Almost instant",
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
