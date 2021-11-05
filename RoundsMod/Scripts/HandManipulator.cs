﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace CommitmentCards.Scripts
{
    public class HandManipulator: MonoBehaviour
    {
        public static HandManipulator instance { get; private set; }
        private void Start()
        {
            instance = this;
            CommitmentCards.Log($"[{CommitmentCards.ModInitials}][MonoBehaviour] HandManipulator has been started");
        }
        public CardInfo DuplicateRandomCard(Player player, int amount)
        {
            CommitmentCards.Log($"[{CommitmentCards.ModInitials}][MonoBehaviour] HandManipulator DuplicateRandomCard called for player {player} and amount {amount}");
            var randomCard = SelectRandomCardWithStats(player);
            if (randomCard == null)
            {
                for (int i = 0; i < amount; i++)
                {
                    HandManipulator.instance.AddCardToPlayer(player, ModdingUtils.Utils.Cards.instance.GetCardWithName("ConsolationPrize"));
                    Thread.Sleep(100);
                }
                return null;
            }

            CommitmentCards.Log($"[{CommitmentCards.ModInitials}][MonoBehaviour] HandManipulator DuplicateRandomCard selected card: {randomCard} {randomCard.cardName}");
            for (int i=0; i < amount; i++){
                AddCardToPlayer(player, ModdingUtils.Utils.Cards.instance.GetCardWithName(randomCard.cardName));
                Thread.Sleep(100);
            }
            return randomCard;
        }
        public CardInfo AddCardToPlayer(Player player, CardInfo cardInfo)
        {
            ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, cardInfo, addToCardBar: true);
            return cardInfo;
        }
        public Player RemoveRandomCard(Player player, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var randomCard = SelectRandomCardWithStats(player);
                if (randomCard == null)
                {
                return null;
                }
                RemoveCardType(player, randomCard);
                Thread.Sleep(100);
            }
            return player;
        }
        public Player RemoveCardType(Player player, CardInfo cardInfo)
        {
            CommitmentCards.Log($"[{CommitmentCards.ModInitials}][MonoBehaviour] HandManipulator RemoveCardType called on card {cardInfo}");
            ModdingUtils.Utils.Cards.instance.RemoveCardFromPlayer(player, cardInfo, ModdingUtils.Utils.Cards.SelectionType.Random);
            CommitmentCards.Log($"[{CommitmentCards.ModInitials}][MonoBehaviour] HandManipulator RemoveCardType finished for card {cardInfo}");
            return player;
        }
        private CardInfo SelectRandomCardWithStats(Player player)
        {
            CommitmentCards.Log($"[{CommitmentCards.ModInitials}][MonoBehaviour] HandManipulator SelectRandomCardWithStats called with player {player}");
            player.data.currentCards.ForEach(card => CommitmentCards.Log($"[{CommitmentCards.ModInitials}][MonoBehaviour] HandManipulator Currently held card regardles of stats: {card}"));
            var cards = player.data.currentCards.Where(card => card.cardStats.Length > 0 && ModdingUtils.Utils.Cards.instance.PlayerIsAllowedCard(player, card)).ToList();
            CommitmentCards.Log($"[{CommitmentCards.ModInitials}][MonoBehaviour] HandManipulator SelectRandomCardWithStats Currently held card with stats: {cards}");
            cards.ForEach(card => CommitmentCards.Log($"[{CommitmentCards.ModInitials}][MonoBehaviour] HandManipulator card: {card}"));
            
            if (cards == null)
            {
                CommitmentCards.Log($"[{CommitmentCards.ModInitials}][MonoBehaviour] HandManipulator cards with stats was null ");
                return null;
            }
            if (cards.Count == 0)
            {
                CommitmentCards.Log($"[{CommitmentCards.ModInitials}][MonoBehaviour] HandManipulator cards with stats was empty ");
                return null;
            }
            var randomCard = cards[UnityEngine.Random.Range(0, cards.Count)];
            CommitmentCards.Log($"[{CommitmentCards.ModInitials}][MonoBehaviour] HandManipulator Random card selected: {randomCard}");
            return randomCard;
        }
    }
}
