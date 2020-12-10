using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;

namespace GoldenMiner
{
    public class PoolData : MonoBehaviour
    {
        public SimpleObjectPool<GameObject> diamond;
        public SimpleObjectPool<GameObject> goldOne;
        public SimpleObjectPool<GameObject> goldTwo;
        public SimpleObjectPool<GameObject> stoneOne;
        public SimpleObjectPool<GameObject> stoneTwo;
        public SimpleObjectPool<GameObject> explosive;
        public SimpleObjectPool<GameObject> potion;
        public SimpleObjectPool<GameObject> boomUi;
        public SimpleObjectPool<GameObject> boomObj;
        public SimpleObjectPool<GameObject> fractionText;

        public Transform allocateTrans;

        public Dictionary<string, SimpleObjectPool<GameObject>> poolAssetSearch;
        public Dictionary<string, SimpleObjectPool<GameObject>> poolNameSearch;

        public PoolData(Transform allocateTrans)
        {
            this.allocateTrans = allocateTrans;

            poolAssetSearch = new Dictionary<string, SimpleObjectPool<GameObject>>();
            poolNameSearch = new Dictionary<string, SimpleObjectPool<GameObject>>();

            diamond = MakePool(QAssetBundle.Prefabs.DIAMONDS, "Diamonds");
            goldOne = MakePool(QAssetBundle.Prefabs.GOLD, "gold");
            goldTwo = MakePool(QAssetBundle.Prefabs.GOLDTWO, "goldTwo");
            stoneOne = MakePool(QAssetBundle.Prefabs.STONEONE, "stoneOne");
            stoneTwo = MakePool(QAssetBundle.Prefabs.STONETWO, "stoneTwo");
            explosive = MakePool(QAssetBundle.Prefabs.EXPLOSIVE, "explosive");
            potion = MakePool(QAssetBundle.Prefabs.POTION, "Potion");
            boomUi = MakePool(QAssetBundle.Prefabs.BOOMUI, "boomUI");
            boomObj = MakePool(QAssetBundle.Prefabs.BOOMOBJ, "boomObj");
            fractionText = MakePool(QAssetBundle.Prefabs.FRACTIONTEXT, "fractionText");
        }

        private SimpleObjectPool<GameObject> MakePool(string assetStr, string nameStr, int num = 0)
        {
            SimpleObjectPool<GameObject> newPool = new SimpleObjectPool<GameObject>(() =>
                    GameManager.Instance.resLoader.LoadSync<GameObject>(assetStr)
                        .InstantiateWithParent(GameManager.Instance.transform).Hide(),
                GameManager.Instance.transform, allocateTrans, null, num);
            poolAssetSearch.Add(assetStr, newPool);
            poolNameSearch.Add(nameStr, newPool);
            return newPool;
        }

        public bool Recycle(GameObject grid)
        {
            try
            {
                string poolName = grid.name.RemoveString("(Clone)");
                GameManager.Instance.poolData.poolNameSearch[poolName].Recycle(grid.gameObject);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }
    }
}