using BepInEx;
using UnboundLib;
using UnboundLib.Cards;
using CommitmentCards.Cards;
using HarmonyLib;
using CommitmentCards.Scripts;

namespace CommitmentCards
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.dk.rounds.plugins.zerogpatch", BepInDependency.DependencyFlags.HardDependency)] // allows straight shooting for 0 gravity bullets- fixes drift mines
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class CommitmentCards: BaseUnityPlugin
    {
        private const string ModId = "com.dk.rounds.CommitmentCards";
        private const string ModName = "CommitmentCards";
        public const string Version = "1.6.4";
        public const string ModInitials = "CC";
        public static CommitmentCards instance { get; private set; }

        void Awake()
        {
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }
        void Start()
        {
            instance = this;
            CustomCard.BuildCard<Copy>();
            CustomCard.BuildCard<Distill>(); 
            CustomCard.BuildCard<Hose>();
            CustomCard.BuildCard<LeadMagazine>();
            CustomCard.BuildCard<DriftMines>();
            CustomCard.BuildCard<ActualGun>();
            CustomCard.BuildCard<BattleOfTitans>();
            CustomCard.BuildCard<ShockBlast>();
            CustomCard.BuildCard<ConsolationPrize>((cardInfo) => ModdingUtils.Utils.Cards.instance.AddHiddenCard(cardInfo));

            gameObject.GetOrAddComponent<HandManipulator>();
        }

        internal static void Log(string message)
        {
            if (false)
            {
                UnityEngine.Debug.Log(message);
            }
        }

    }
}
