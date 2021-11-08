using BepInEx;
using UnboundLib;
using UnboundLib.Cards;
using CommitmentCards.Cards;
using HarmonyLib;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using CommitmentCards.Scripts;

namespace CommitmentCards
{
    // These are the mods required for our mod to work
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    // Declares our mod to Bepin
    [BepInPlugin(ModId, ModName, Version)]
    // The game our mod is associated with
    [BepInProcess("Rounds.exe")]
    public class CommitmentCards: BaseUnityPlugin
    {
        private const string ModId = "com.dk.rounds.CommitmentCards";
        private const string ModName = "CommitmentCards";
        public const string Version = "1.3.0";
        public const string ModInitials = "CC";
        public static CommitmentCards instance { get; private set; }

        void Awake()
        {
            // Use this to call any harmony patch files your mod may have
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }
        void Start()
        {
            instance = this;
            CustomCard.BuildCard<Copy>();
            //CustomCard.BuildCard<Distill>(); Disabled pending fix
            CustomCard.BuildCard<Refine>();
            CustomCard.BuildCard<Hose>();
            CustomCard.BuildCard<LeadMagazine>();
            CustomCard.BuildCard<DriftMines>();
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
