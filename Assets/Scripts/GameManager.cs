using System.Collections;
using System.Collections.Generic;
using QFramework;
using QFramework.Example;
using UnityEngine;

using GoldenMiner;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get => _instance;
    }

    public ResLoader resLoader; //资源加载模块
    public PoolData poolData; //对象池
    public TerrainCanvas terrainCanvas;

    public GameObject uiRoot;
    [Header("当前游戏分数")] public int gameFraction;
    [Header("当前游戏时间")] public int time;
    [Header("当前游戏目标分数")] public int passFraction;
    [Header("当前游戏关卡数")] public int level;
    [HideInInspector] public string[] objNames; //所有分数道具的物体名称
    [HideInInspector] public int[] fractionData; //分数数据
    [HideInInspector] public string[] propName; //特殊道具名称
    [HideInInspector] public int[] targetFraction; //关卡的目标分数
    [HideInInspector] public float[] scaleData; //缩放等级

    public int[] gameTimes; //当前关卡的时间
    [Header("当前关卡生成的所有物体")] public List<GameObject> levelObjs;
    public bool isDouble; //是否进行双倍得分
    public int minFraction = 1500; //  场景中生成的道具分数总和比目标最小值多1500


    [Header("生成的最小X值")] public float minX;
    [Header("生成的最大X值")] public float maxX;
    [Header("生成的最小y值")] public float minY;
    [Header("生成的最大y值")] public float maxY;

    public float gameTime; //游戏时间
    public bool isPause; //是否暂停
    [HideInInspector] public UIHomePanel UIManager; //UI管理器
    [HideInInspector] public Player player; //玩家

    public Material lineMater;


    private int[,] point; // 二维数组初始化物体位置 
    private int width;
    private int depth;

    private int targetValue; //第一关分数

    private void Awake()
    {
        _instance = this;
        ResMgr.Init();
        resLoader = ResLoader.Allocate();
    }

    void Start()
    {
        poolData = new PoolData(terrainCanvas.Map);

        UIKit.OpenPanel<UIHomePanel>();
        UIManager = uiRoot.GetComponent<UIRoot>().CommonTrans.Find("UIHomePanel").gameObject
            .GetComponent<UIHomePanel>();

        player = FindObjectOfType<Player>();
        targetFraction = new int[] { 3000, 4000, 5000, 7000, 9000, 12000, 15000, 20000, 25000 };
        gameTimes = new int[] { 30, 40, 45, 50, 55, 65, 65, 70, 70 };
        //gameTimes = new int[] { 5, 5, 5, 5, 5, 5, 5, 5, 5 };
        objNames = new string[] { "Diamonds", "gold", "goldTwo", "stoneOne", "stoneTwo" };
        fractionData = new int[] { 1000, 400, 500, 80, 100 };
        propName = new string[] { "explosive", "Potion" };
        scaleData = new float[] { 1.0f, 1.2f, 1.5f, 1.8f, 2.0f };
        levelObjs = new List<GameObject>(); //当前等级场上物体

        //适配
        float screen = (Screen.width / (float)Screen.height);
        minX = -2.8f * screen / (float)(750 / 1334f);
        maxX = 2.8f * screen / (float)(750 / 1334f);
        minY = -4.6f * screen / (float)(750 / 1334f);
        maxY = 2.4f * screen / (float)(750 / 1334f);


        width = (int)((maxX - minX) / 0.6f);
        depth = (int)((maxY - minY) / 0.6f);
        point = new int[width, depth];

        level = 0;
        SwitchLevel();
        gameTime = gameTimes[level];

        UIManager.UpdateLevelText(level);
        UIManager.UpdateTargetText(targetFraction[level]);
        targetValue = targetFraction[0];

        //gameFraction = 1000000;
    }

    void Update()
    {
        DrawBoxLine();
        if (!isPause)
        {
            gameTime -= Time.deltaTime;
            UIManager.UpdateTimeText(gameTime);
            if (gameTime < 0)
            {
                isPause = true;
                //关卡结束比较分数
                if (gameFraction > targetValue)
                {
                    UIManager.OpenCloseSwitchPanel();
                }
                else
                {
                    UIManager.OpenDeathPanle();
                }
            }
        }
    }

    /// <summary>
    /// 切换关卡方法
    /// </summary>
    public void SwitchFunc()
    {
        level++;
        isPause = false;
        isDouble = false;
        UIManager.UpdateLevelText(level);
        //第九关结束
        if (level >= 9) UIManager.OpenDeathPanle();

        SwitchLevel();
        gameTime = gameTimes[level];
        UpdateTargetVaule();
        player.PlayStateRest();

        for (int i = 0; i < width; i++)
            for (int j = 0; j < depth; j++)
            {
                point[i, j] = 0;
            }
    }

    /// <summary>
    /// 随机一个道具
    /// </summary>
    /// <returns></returns>
    public GameObject RandomProp(string name)
    {
        Vector3 tempPoint = RandomPos();

        GameObject tempObj = null;
        GameObject t = null;
        switch (name)
        {
            case "Diamonds":
                tempObj = Instance.poolData.diamond.Allocate(tempPoint);
                break;
            case "gold":
                tempObj = Instance.poolData.goldOne.Allocate(tempPoint);
                break;
            case "goldTwo":
                tempObj = Instance.poolData.goldTwo.Allocate(tempPoint);
                tempObj.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                break;
            case "stoneOne":
                tempObj = Instance.poolData.stoneOne.Allocate(tempPoint);
                break;
            case "stoneTwo":
                tempObj = Instance.poolData.stoneTwo.Allocate(tempPoint);
                break;
        }

        return tempObj;
    }

    /// <summary>
    /// 随机生成特殊道具
    /// </summary>
    /// <returns></returns>
    public GameObject RandomSpecialProp()
    {
        int index = Random.Range(0, propName.Length);
        string name = propName[index];
        Vector3 tempPoint = RandomPos();
        GameObject tempObj = null;
        switch (name)
        {
            case "explosive":
                tempObj = Instance.poolData.explosive.Allocate(tempPoint);
                break;
            case "Potion":
                tempObj = Instance.poolData.potion.Allocate(tempPoint);
                break;
        }

        tempObj.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);

        PropScript propScript = tempObj.GetComponent<PropScript>();
        if (index == 0)
        {
            propScript.nowType = PropType.Boom;
        }
        else
        {
            propScript.nowType = PropType.Potion;
        }

        return tempObj;
    }

    /// <summary>
    /// 随机道具位置
    /// </summary>
    /// <returns></returns>
    public Vector3 RandomPos()
    {
        int rx = Random.Range(0, width);
        int ry = Random.Range(0, depth);
        while (point[rx, ry] == 1)
        {
            rx = Random.Range(0, width);
            ry = Random.Range(0, width);
        }

        point[rx, ry] = 1;
        Vector3 tempPoint = new Vector3(minX + 0.6f * (rx + 1), maxY - 0.6f * (ry + 1), 0);
        return tempPoint;
    }

    /// <summary>
    /// 随机道具缩放 返回缩放值，用于计算 返回大小 out重量权值
    /// </summary>
    public float RandomScale(out int scaleLevel)
    {
        int index = Random.Range(0, scaleData.Length);
        scaleLevel = index + 1;
        float scaleVaule = scaleData[index];
        return scaleVaule;
    }

    /// <summary>
    /// 随机道具旋转
    /// </summary>
    /// <returns></returns>
    public Quaternion RandomRotate()
    {
        float angle = Random.Range(0, 360);
        Quaternion tempQuat = Quaternion.AngleAxis(angle, Vector3.forward);
        return tempQuat;
    }

    /// <summary>
    /// 关卡道具生成
    /// </summary>
    public void SwitchLevel()
    {
        SceneObjs();
        int creatfraction = 0; //现在已经生成的分数
        int tempfraction = targetFraction[level];
        tempfraction += minFraction;

        while (creatfraction < tempfraction)
        {
            //随机取分数道具
            int objIndex = Random.Range(0, objNames.Length);
            string tempName = objNames[objIndex];
            GameObject tempObj = RandomProp(tempName);

            float tempScale = 1;
            int scaleLevel = 1; //分数物体权重下标

            if (objIndex != 0) //钻石不进行旋转缩放操作
            {
                Quaternion tempQuat = RandomRotate();
                tempObj.transform.rotation = tempQuat;
                tempScale = RandomScale(out scaleLevel);
                tempObj.transform.localScale *= tempScale;
            }
            else
                tempObj.transform.rotation = Quaternion.Euler(0, 0, 0);

            var tempScript = tempObj.GetComponent<PropScript>();
            tempScript.nowType = PropType.Fraction;

            //石头重量赋值
            tempScript.scaleLevel = scaleLevel;

            int fraction = fractionData[objIndex]; //分数
            fraction = (int)(fraction * tempScale);
            tempScript.fraction = fraction;

            creatfraction += fraction;
            levelObjs.Add(tempObj);

        }

        //特殊道具生成
        int propCount = Random.Range(0, 3); //场上最多2个道具
        while (propCount > 0)
        {
            propCount--;
            GameObject tempObj = RandomSpecialProp();
            tempObj.transform.rotation = Quaternion.Euler(0, 0, 0);
            levelObjs.Add(tempObj);
        }
    }

    /// <summary>
    /// 场景物体回收
    /// </summary>
    public void SceneObjs()
    {
        if (levelObjs.Count > 0)
        {
            for (int i = 0; i < levelObjs.Count; i++)
            {
                Instance.poolData.Recycle(levelObjs[i]);
            }
        }

        levelObjs.Clear();
    }

    /// <summary>
    /// 分数增加
    /// </summary>
    /// <param name="fraction"></param>
    public void AddFraction(int fraction)
    {
        if (isDouble)
        {
            fraction *= 2; //双倍得分 分数翻倍
        }

        gameFraction += fraction;
        UIManager.UpdateMoneyText(gameFraction);
        UIManager.CreatTipsText(fraction);
    }

    /// <summary>
    /// 计算下一关的目标分数
    /// </summary>
    public void UpdateTargetVaule()
    {
        targetValue = 0;
        for (int i = 0; i <= level; i++)
        {
            targetValue += targetFraction[i];
        }

        UIManager.UpdateTargetText(targetValue);
    }

    public void AddBoomProp()
    {
        UIManager.BoomAddCount();
    }

    public bool UseBoomProp()
    {
        bool isComplte = UIManager.UseBoom();
        return isComplte;
    }

    public void DrawBoxLine()
    {
        Debug.DrawLine(new Vector3(minX, minY, 90), new Vector3(maxX, minY, 90), Color.red);
        Debug.DrawLine(new Vector3(minX, minY, 90), new Vector3(minX, maxY, 90), Color.red);
        Debug.DrawLine(new Vector3(minX, maxY, 90), new Vector3(maxX, maxY, 90), Color.red);
        Debug.DrawLine(new Vector3(maxX, minY, 90), new Vector3(maxX, maxY, 90), Color.red);
    }

    /// <summary>
    /// 炸弹碰到了物体
    /// </summary>
    public void BoomFunc()
    {
        player.RestMoveSpeed();
        if (player.chidrenObj != null)
        {
            player.isBack = true;
            Instance.poolData.Recycle(player.chidrenObj.gameObject);
        }

        player.chidrenObj = null;
    }
}
