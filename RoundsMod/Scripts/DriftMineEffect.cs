using UnityEngine;
using UnboundLib;
using Photon.Pun;
using ModdingUtils.Extensions;
using System.Reflection;
using System.Collections.Generic;

namespace CommitmentCards.Scripts
{
    public class SpeedUpBulletAssets
    {
        private static GameObject _DirftMineBullet = null;

        internal static GameObject driftMineBullet
        {
            get
            {
                if (SpeedUpBulletAssets._DirftMineBullet != null) { return SpeedUpBulletAssets._DirftMineBullet; }
                else
                {
                    SpeedUpBulletAssets._DirftMineBullet = new GameObject("Drift_Mine", typeof(DriftMineEffect), typeof(PhotonView));
                    UnityEngine.GameObject.DontDestroyOnLoad(SpeedUpBulletAssets._DirftMineBullet);

                    return SpeedUpBulletAssets._DirftMineBullet;
                }
            }
            set { }
        }
    }
        public class DriftMineBulletSpawner : MonoBehaviour
        {
            private static bool Initialized = false;


            void Awake()
            {
                if (!Initialized)
                {
                    PhotonNetwork.PrefabPool.RegisterPrefab(SpeedUpBulletAssets.driftMineBullet.name, SpeedUpBulletAssets.driftMineBullet);
                }
            }

            void Start()
            {
                if (!Initialized)
                {
                    Initialized = true;
                    return;
                }

                if (!PhotonNetwork.OfflineMode && !this.gameObject.transform.parent.GetComponent<ProjectileHit>().ownPlayer.data.view.IsMine) return;


                PhotonNetwork.Instantiate(
                    SpeedUpBulletAssets.driftMineBullet.name,
                    transform.position,
                    transform.rotation,
                    0,
                    new object[] { this.gameObject.transform.parent.GetComponent<PhotonView>().ViewID }
                );
            }
        }
    [RequireComponent(typeof(PhotonView))]
    public class DriftMineEffect : MonoBehaviour, IPunInstantiateMagicCallback
    {
        public void OnPhotonInstantiate(Photon.Pun.PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            GameObject parent = PhotonView.Find((int)instantiationData[0]).gameObject;
            this.gameObject.transform.SetParent(parent.transform);


        }

        void Awake()
        {

        }
        void Start()
        {
            CommitmentCards.Log("Starting DriftMineBulletEffect");
            this.gameObject.transform.parent.GetComponent<ProjectileHit>().transform.Translate(Vector3.forward*1.5f);
        }
    }
}
