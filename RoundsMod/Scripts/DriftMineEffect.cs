using UnityEngine;
using Photon.Pun;

namespace CommitmentCards.Scripts
{
    public class DriftMineAssets
    {
        private static GameObject _DriftMineBullet = null;

        internal static GameObject driftMineBullet
        {
            get
            {
                if (_DriftMineBullet != null) { return _DriftMineBullet; }
                else
                {
                    _DriftMineBullet = new GameObject("Drift_Mine", typeof(DriftMineEffect), typeof(PhotonView));
                    UnityEngine.GameObject.DontDestroyOnLoad(_DriftMineBullet);

                    return _DriftMineBullet;
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
                    PhotonNetwork.PrefabPool.RegisterPrefab(DriftMineAssets.driftMineBullet.name, DriftMineAssets.driftMineBullet);
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
                    DriftMineAssets.driftMineBullet.name,
                    transform.position,
                    transform.rotation,
                    0,
                    new object[] { this.gameObject.transform.parent.GetComponent<PhotonView>().ViewID }
                );
            }
        }

    // Causes bullets to start at gun tip, not inside body - prevents self-hits with slow bullets
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
            var projectile = this.gameObject.transform.parent.GetComponent<ProjectileHit>();
            projectile.transform.Translate(Vector3.forward * 1.5f);
        }
    }
}
