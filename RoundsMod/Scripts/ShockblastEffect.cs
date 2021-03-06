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
using ModdingUtils.RoundsEffects;

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
            
            object[] instantiationData = info.photonView.InstantiationData;
            GameObject parent = PhotonView.Find((int)instantiationData[0]).gameObject; 
            gameObject.transform.SetParent(parent.transform);
            CommitmentCards.Log("Photon instantiate for shockblasteffect");
            foreach (Transform child in gameObject.transform.parent)
            {
                CommitmentCards.Log("Children of parent: " + child.gameObject);
            }
            projectile = gameObject.transform.parent.GetComponent<ProjectileHit>();
            foreach (Transform child in projectile.transform)
            {
                CommitmentCards.Log("Children of projectile: " + child.gameObject);
            }
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
            var range = CalculateRange(gun);
            CommitmentCards.Log("Visual generated, triggering");
            TriggerVisual(visual, range);

            CommitmentCards.Log("Getting targets");
            var targetsInRange = GetInRangeTargets(player.transform.position, range);
            CommitmentCards.Log("Beginning explosion logic");
            foreach (RaycastHit2D targetRaycastHit in targetsInRange)
            {
                var target = targetRaycastHit.collider;
                CommitmentCards.Log("Checking for rigidbody on collider " + target+" from attachedRigidbody: "+target.attachedRigidbody+" from component in parent: "+target.GetComponentInParent<Rigidbody2D>());
                var rigidbody = target.GetComponentInParent<Rigidbody2D>();
                if (rigidbody) {
                    CommitmentCards.Log("rigidbody found");
                    DoPushRigidbody(player.transform.position, rigidbody, gun.GetAdditionalData().shockBlastBaseForce * (gun.damage / 2));
                }
                else { 
                CommitmentCards.Log("Checking for player on collider " + target + ": " + target.GetComponentInParent<Player>());
                    var otherPlayer = target.GetComponentInParent<Player>();
                    if (otherPlayer)
                    {
                        CommitmentCards.Log("player found");
                        DoPushCharData(player.transform.position, otherPlayer, gun.GetAdditionalData().shockBlastBaseForce * (gun.damage / 2));
                        ApplyBulletEffects(GetDamageVector(), gun, targetRaycastHit, otherPlayer);
                    }
                }
                CommitmentCards.Log("Checking for damageable on collider " + target);
                var damageable = target.transform.gameObject.GetComponent<Damagable>();
                if (damageable)
                {
                    CommitmentCards.Log("damageable found");
                    DoDamage(damageable);
                }

            }

            projectile.projectileColor = Color.black;
            projectile.bulletCanDealDeamage = false;
            projectile.sendCollisions = false;
            projectile.transform.position = (new Vector3(-1000f, -10000f, -1000f));
        }

        private ISet<RaycastHit2D> GetInRangeTargets(Vector2 origin, float range)
        {
            ISet<RaycastHit2D> targets = new HashSet<RaycastHit2D>();
            var colliders = Physics2D.OverlapCircleAll(origin, range);
            var playerCollider = player.GetComponent<Collider2D>();
            CommitmentCards.Log("Player collider? " + playerCollider + "At position " + player.transform.position);
            foreach (Collider2D collider in colliders)
            {
                CommitmentCards.Log("Looking at collider (" + collider + ") for gameobject " + collider.gameObject + " at position " + collider.transform.position);
                if (!collider.Equals(player.GetComponent<Collider2D>()))
                {
                    CommitmentCards.Log("Checking if collider (" + collider + ") for gameobject " + collider.gameObject + " is visible");
                    //Eliminates encountered colliders for anything without a rigidbody except players, which apparently don't have one. Go figure.
                    var list = Physics2D.RaycastAll(origin, (((Vector2)collider.transform.position) - origin).normalized, range).Select(item => item).Where(item => !item.collider.Equals(playerCollider) && (item.collider.attachedRigidbody || (item.collider.GetComponentInParent<Player>() && item.collider.GetComponentInParent<Player>().playerID != player.playerID))).ToList();
                    list.ForEach(item => CommitmentCards.Log("raycast item: " + item.collider.gameObject + "is the collider we're looking at? " + (item.Equals(collider))));
                    if (list.Count > 0 && list[0].collider.Equals(collider))
                    {
                        CommitmentCards.Log("Item matched, adding to targets: " + collider.transform.gameObject);
                        targets.Add(list[0]);
                    }
                }
            }
            return targets;
        }

        private void DoPushRigidbody(Vector2 origin, Rigidbody2D rigidbody, float force)
        {
            CommitmentCards.Log("Doing push");
            CommitmentCards.Log("Adding force " + force + " in direction " + (rigidbody.position - origin).normalized + "For a net value of " + ((rigidbody.position - origin).normalized * force * rigidbody.mass));
            rigidbody.AddForce((rigidbody.position - origin).normalized * force * rigidbody.mass * 0.75f);
        }

        private void DoPushCharData(Vector2 origin, Player otherPlayer, float force)
        {
            CommitmentCards.Log("Doing push for player");
            CommitmentCards.Log(" adding force "+force+" for total vector " + ((Vector2)otherPlayer.transform.position - origin).normalized * force*2);
            var healthHandler = otherPlayer.GetComponentInChildren<HealthHandler>();
            healthHandler.CallTakeForce(((Vector2)otherPlayer.transform.position - origin).normalized * force*2);
            CommitmentCards.Log("Post force");
        }

        private void DoDamage(Damagable damageable)
        {
      
            CommitmentCards.Log("Doing damage");
            var totalDamageVector = GetDamageVector();
            CommitmentCards.Log("Gun damage: " + gun.damage + " bulletDamageMultiplier " + gun.bulletDamageMultiplier +" Total damage: "+ totalDamageVector.y + " transform? " + player.transform);
            damageable.CallTakeDamage(totalDamageVector, player.transform.position, gun.gameObject, player);
            player.GetComponent<HealthHandler>().Heal(totalDamageVector.y * player.GetComponentInChildren<CharacterStatModifiers>().lifeSteal);
        }
        private Vector2 GetDamageVector()
        {
            var totalDamage = 55 * gun.damage * gun.bulletDamageMultiplier / 1.5f;
            return Vector2.up * totalDamage;
        }

        private void ApplyBulletEffects(Vector2 damage, Gun gun, RaycastHit2D targetRaycastHit, Player otherPlayer)
        {
            CommitmentCards.Log("Starting apply bullet effects");
            var hitInfo = RaycastHit2DToHitInfo(targetRaycastHit);
            foreach(ObjectsToSpawn obj in gun.objectsToSpawn)
            {
                CommitmentCards.Log("Looking at object to spawn- effect:" + obj.effect + " addtoprojectile: " + obj.AddToProjectile);
                if (obj.AddToProjectile && !obj.AddToProjectile.GetComponent<ShockBlastSpawner>())
                {
                    CommitmentCards.Log("Addtoprojectile name: " + obj.AddToProjectile.name);
                    var effect = obj.AddToProjectile.GetComponent<HitEffect>();
                    if (effect)
                    {
                        CommitmentCards.Log("Seeing bullet effect from objectToSpawn.AddToProjectile: " + effect.name);
                        effect.DealtDamage(damage, false, otherPlayer);
                    }
                }
                else if (obj.effect)
                {
                    var effect = obj.effect.GetComponent<HitEffect>();
                    if (effect)
                    {
                        CommitmentCards.Log("Seeing bullet effect from objectToSpawn.effect: " + effect.name);
                        effect.DealtDamage(damage, false, otherPlayer);
                    }
                }
                else if(!obj.AddToProjectile.GetComponent<ShockBlastSpawner>())
                {
                    //Triggers Dazzle, at least, maybe others
                    CommitmentCards.Log("Attempting object spawn?");
                    ObjectsToSpawn.SpawnObject(player.transform, hitInfo, obj, player.GetComponent<HealthHandler>(), player.GetComponent<PlayerSkin>(), damage.y);
                }
            }
            projectile.effects.ForEach(it =>
            {
                CommitmentCards.Log("Triggering bullet effect from projectile: " + it.name);
                it.DoHitEffect(hitInfo);
            });
            player.GetComponents<HitEffect>().ToList().ForEach(it => {
                CommitmentCards.Log("Triggering bullet effect from player: " + it.name);
                it.DealtDamage(damage, false, otherPlayer);
                });
        }


        private HitInfo RaycastHit2DToHitInfo(RaycastHit2D targetRaycastHit)
        {
            var hitInfo = new HitInfo();
            hitInfo.collider = targetRaycastHit.collider;
            hitInfo.normal = targetRaycastHit.normal;
            hitInfo.point = targetRaycastHit.point;
            hitInfo.rigidbody = targetRaycastHit.rigidbody;
            hitInfo.transform = targetRaycastHit.transform;
            return hitInfo;
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
                parts.startColor = projectile.projectileColor == Color.black ? Color.cyan : projectile.projectileColor;
                parts.startLifetime = parts.startLifetime / 2;
            }
            _shockblastVisual.transform.GetChild(1).GetComponent<LineEffect>().colorOverTime.colorKeys = new GradientColorKey[] { new GradientColorKey(projectile.projectileColor == Color.black ? Color.cyan : projectile.projectileColor, 0f) };
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

        private void TriggerVisual(GameObject visual, float range)
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
            visual.transform.GetChild(1).GetComponent<LineEffect>().radius = (range);
            visual.transform.GetChild(1).GetComponent<LineEffect>().SetFieldValue("startWidth", 0.5f);
            visual.transform.position = player.transform.position;
            CommitmentCards.Log("Playing effect");
            visual.transform.GetChild(1).GetComponent<LineEffect>().Play();
        }
        private float CalculateRange(Gun gun)
        {
            var range = gun.GetAdditionalData().shockBlastRange + ((float)Math.Sqrt(gun.projectileSpeed)*1.5f);
            CommitmentCards.Log("gun.projectileSpeed: " + gun.projectileSpeed);
            CommitmentCards.Log("Range: " + range);
            return range;
        }


    }
}

