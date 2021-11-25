using UnityEngine;
using Photon.Pun;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnboundLib.Utils;
using System.Reflection;
using System.Linq;
using UnboundLib;
using UnboundLib.Networking;
using CommitmentCards.Extensions;
using System.Collections;
using HarmonyLib;

namespace CommitmentCards.Scripts
{
    class ShockBlastAssets
    {
        private static GameObject _shockblast = null;

        internal static GameObject shockblast
        {
            get
            {
                if (_shockblast != null) { return _shockblast; }
                else
                {
                    _shockblast = new GameObject("Shock_blast", typeof(ShockBlastEffect), typeof(PhotonView));
                    UnityEngine.GameObject.DontDestroyOnLoad(_shockblast);

                    return _shockblast;
                }
            }
            set { }
        }

        
    }
  
    public class ShockBlastSpawner : MonoBehaviour
    {
        private static bool Initialized = false;

        void Awake()
        {
            if (!Initialized)
            {
                PhotonNetwork.PrefabPool.RegisterPrefab(ShockBlastAssets.shockblast.name, ShockBlastAssets.shockblast);
            }
        }

        void Start()
        {
            CommitmentCards.Log("ShockBlastSpawner Start");
            if (!Initialized)
            {
                Initialized = true;
                return;
            }
            CommitmentCards.Log("Checking is projectile is mine?");
            if (!PhotonNetwork.OfflineMode && !this.gameObject.transform.parent.GetComponent<ProjectileHit>().ownPlayer.data.view.IsMine) return;

            CommitmentCards.Log("Instantiating object");
            var name = ShockBlastAssets.shockblast.name;
            CommitmentCards.Log("Name retrieved "+name);
            PhotonView photonView = gameObject.transform.parent.GetComponent<PhotonView>(); ;
            if (!photonView)
            {
                CommitmentCards.Log("PhotonView is null, aborting start");
                return;
            }
            
            CommitmentCards.Log("PhotonView: " + photonView);
            int viewId = photonView.ViewID;
            CommitmentCards.Log("ViewId retrieved " + viewId);
            PhotonNetwork.Instantiate(
                name,
                transform.position,
                transform.rotation,
                0,
                new object[] { viewId }
            );
        }
    }

    [RequireComponent(typeof(PhotonView))]
    public class ShockBlastEffect : MonoBehaviour, IPunInstantiateMagicCallback
    {
        private Player player;
        private Gun gun;
        private ProjectileHit projectile;
        public void OnPhotonInstantiate(Photon.Pun.PhotonMessageInfo info)
        {
            CommitmentCards.Log("Photon instantiate for shockblasteffect");
            foreach (Transform child in transform)
            {
                CommitmentCards.Log("Child: "+child.gameObject);
            }
            object[] instantiationData = info.photonView.InstantiationData;
            GameObject parent = PhotonView.Find((int)instantiationData[0]).gameObject;
            gameObject.transform.SetParent(parent.transform);
            projectile = gameObject.transform.parent.GetComponent<ProjectileHit>();
            player = projectile.ownPlayer;
            
            gun = player.GetComponent<Holding>().holdable.GetComponent<Gun>();
        }

        void Awake()
        {

        }
        void Start()
        {
            CommitmentCards.Log("Starting shockBlastEffect");
            var visual = GenerateVisual();
            CommitmentCards.Log("Visual generated, triggering");
            TriggerVisual(visual);
            CommitmentCards.Log("Beginning explosion logic");
            var targetsInRange = GetInRangeTargets();
            foreach(Player target in targetsInRange)
            {
                DoPush(player.transform.position, target.GetComponent<Damagable>(), gun.GetAdditionalData().shockBlastBaseForce * (gun.damage/2));
            }

            projectile.bulletCanDealDeamage = false;
            projectile.transform.position = (new Vector3(0, 10000f, 10000f));
        }

        private List<Player> GetInRangeTargets()
        {
            List<Player> players = new List<Player>();
            foreach(Player player in PlayerManager.instance.players.Where(player => player.teamID != this.player.teamID).ToList())
            {
                CharacterData data = player.GetComponent<CharacterData>();
                float dist = Vector2.Distance(this.player.GetComponent<CharacterData>().playerVel.position, data.playerVel.position);
                if (dist <= CalculateRange(gun) && PlayerManager.instance.CanSeePlayer(this.player.transform.position, player).canSee)
                {
                    players.Add(player);
                }
            }
            return players;
        }

        private void DoPush(Vector2 point, Damagable damageable, float force)
        {
            CommitmentCards.Log("Doing push");
            var charData = damageable.GetComponent<CharacterData>();
            var damage = Vector2.up * 55 * gun.damage * gun.bulletDamageMultiplier / 4;
            CommitmentCards.Log("Damage to be dealt: " + damage);
            CommitmentCards.Log("Health handler exists?" + charData.healthHandler);
            CommitmentCards.Log("Gun damage: " + gun.damage + " bulletDamageMultiplier " + gun.bulletDamageMultiplier + " transform? " + player.transform);
            charData.healthHandler.CallTakeDamage(Vector2.up * 55 * gun.damage * gun.bulletDamageMultiplier / 4, player.transform.position);
            CommitmentCards.Log("Damage applied, adding force "+force);
            charData.healthHandler.CallTakeForce(((Vector2)damageable.transform.position - point).normalized * force);
            CommitmentCards.Log("Post force");
        }

        private GameObject GenerateVisual()
        {
            GameObject _shockblastVisual;
            List<CardInfo> activecards = ((ObservableCollection<CardInfo>)typeof(CardManager).GetField("activeCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).ToList();
            List<CardInfo> inactivecards = (List<CardInfo>)typeof(CardManager).GetField("inactiveCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            List<CardInfo> allcards = activecards.Concat(inactivecards).ToList();
            GameObject E_Overpower = allcards.Where(card => card.cardName.ToLower() == "overpower").First().GetComponent<CharacterStatModifiers>().AddObjectToPlayer.GetComponent<SpawnObjects>().objectToSpawn[0];
            _shockblastVisual = UnityEngine.GameObject.Instantiate(E_Overpower, new Vector3(0, 100000f, 0f), Quaternion.identity);
            _shockblastVisual.name = "E_Discombobulate";
            DontDestroyOnLoad(_shockblastVisual);
            foreach (ParticleSystem parts in _shockblastVisual.GetComponentsInChildren<ParticleSystem>())
            {
                parts.startColor = Color.cyan;
                parts.startLifetime = parts.startLifetime / 2;
            }
            _shockblastVisual.transform.GetChild(1).GetComponent<LineEffect>().colorOverTime.colorKeys = new GradientColorKey[] { new GradientColorKey(Color.cyan, 0f) };
            UnityEngine.GameObject.Destroy(_shockblastVisual.transform.GetChild(2).gameObject);
            _shockblastVisual.transform.GetChild(1).GetComponent<LineEffect>().offsetMultiplier = 0f;
            _shockblastVisual.transform.GetChild(1).GetComponent<LineEffect>().playOnAwake = true;
            UnityEngine.GameObject.Destroy(_shockblastVisual.GetComponent<FollowPlayer>());
            _shockblastVisual.GetComponent<DelayEvent>().time = 0f;
            UnityEngine.GameObject.Destroy(_shockblastVisual.GetComponent<SoundImplementation.SoundUnityEventPlayer>());
            UnityEngine.GameObject.Destroy(_shockblastVisual.GetComponent<Explosion>());
            UnityEngine.GameObject.Destroy(_shockblastVisual.GetComponent<Explosion_Overpower>());
            UnityEngine.GameObject.Destroy(_shockblastVisual.GetComponent<RemoveAfterSeconds>());
            return _shockblastVisual;
        }

        private void TriggerVisual(GameObject visual)
        {
            CommitmentCards.Log("Setting scale");
            visual.transform.localScale = new Vector3(1f, 1f, 1f);
            CommitmentCards.Log("Adding removeAfterSeconds");
            visual.AddComponent<RemoveAfterSeconds>().seconds = 5f;
            CommitmentCards.Log("Initializing line effect");
            visual.transform.GetChild(1).GetComponent<LineEffect>().SetFieldValue("inited", false);
            typeof(LineEffect).InvokeMember("Init",
                BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic,
                null, visual.transform.GetChild(1).GetComponent<LineEffect>(), new object[] { });
            CommitmentCards.Log("Adjusting line effect");
            visual.transform.GetChild(1).GetComponent<LineEffect>().radius = (CalculateRange(gun));
            visual.transform.GetChild(1).GetComponent<LineEffect>().SetFieldValue("startWidth", 0.5f);
            visual.transform.position = player.transform.position;
            CommitmentCards.Log("Playing effect");
            visual.transform.GetChild(1).GetComponent<LineEffect>().Play();
        }
        private float CalculateRange(Gun gun)
        {
            CommitmentCards.Log("gun.projectileSpeed: " + gun.projectileSpeed);
            CommitmentCards.Log("Range: " + gun.GetAdditionalData().shockBlastRange + ((float)Math.Sqrt(gun.projectileSpeed)*1.5f));
            return gun.GetAdditionalData().shockBlastRange + ((float)Math.Sqrt(gun.projectileSpeed));
        }


    }
}

