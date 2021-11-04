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
        public const string Version = "1.0.1";
        public const string ModInitials = "CC";
        public static CommitmentCards instance { get; private set; }

#if DEBUG
        public static readonly bool DEBUG = true;
#else
        public static readonly bool DEBUG = false;
#endif

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
            CustomCard.BuildCard<Distill>();
            CustomCard.BuildCard<Refine>();
            CustomCard.BuildCard<Hose>();

            gameObject.GetOrAddComponent<HandManipulator>();
        }

        internal static void Log(string message)
        {
            if (true)
            {
                UnityEngine.Debug.Log(message);
            }
        }

    }
}
