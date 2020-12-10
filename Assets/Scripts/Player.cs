using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace GoldenMiner
{
    public enum RotaDir
    {
        left,
        right,
    }

    public class Player : MonoBehaviour
    {

        public Transform startTrans; //起始点
        public RotaDir nowDir;
        public float angleSpeed;
        public float moveSpeed;
        LineRenderer lineRenderer;
        public bool isFire;
        public Vector3 playStartPoint;
        public bool isBack; //玩家是否返回
        public PropScript chidrenObj; //现在被抓到的物体
        private float startSpeed;
        
        public GameObject boomObj; //炸弹的预制体

        private float t1, t2; //鼠标双击计时器
        public bool boomFire; //当前是否扔下了炸弹

        private bool almostGet; //防止出现炸弹旋转现象

        private int hookGetAccount = 0;//物体全部抓完或计时结束 进入下关后重新置0

        void Start()
        {
            isFire = false;
            isBack = false;
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            startSpeed = moveSpeed;
        }

        void Update()
        {

            if (GameManager.Instance.isPause)
            {
                hookGetAccount = 0;
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {

                if (!isFire)
                {
                    isFire = true;
                    playStartPoint = transform.position;
                }
            }

            //双击生成炸弹
            if (Input.GetMouseButtonDown(0))
            {
                t2 = Time.deltaTime;

                if (t2 - t1 < 0.1f && chidrenObj != null && boomFire == false && almostGet == false)
                {
                    Debug.Log("炸弹生成");

                    BoomFire();

                }

                t1 = t2;
            }

            if (isFire && !isBack)
            {
                PlayMoveForward();
                CheckExceed();
            }
            else if (isFire && isBack)
            {
                PlayBackMove();
                CheckMoveToStart();
            }

            if (!isFire)
            {
                PlayRotate();
            }

            UpdataLine();
        }

        public void PlayRotate()
        {
            float rightAngle = Vector3.Angle(transform.up * -1, Vector3.right);

            if (nowDir == RotaDir.left)
            {
                if (rightAngle < 170)
                {
                    transform.RotateAround(startTrans.position, Vector3.forward, angleSpeed * Time.deltaTime);
                }
                else
                {
                    nowDir = RotaDir.right;
                }

            }
            else
            {
                if (rightAngle > 10)
                {
                    transform.RotateAround(startTrans.position, Vector3.forward, -angleSpeed * Time.deltaTime);
                }
                else
                {
                    nowDir = RotaDir.left;
                }

            }
        }

        public void UpdataLine()
        {
            lineRenderer.SetPosition(0, startTrans.position);
            lineRenderer.SetPosition(1, transform.position);
        }

        /// <summary>
        /// 发射炸弹
        /// </summary>
        public void BoomFire()
        {
            bool isOK = GameManager.Instance.UseBoomProp();
            if (isOK)
            {
                boomObj = GameManager.Instance.poolData.boomObj.Allocate(startTrans.position);
                //boomFire为true时不能再次释放炸弹
                boomFire = true;
                boomObj.AddComponent<BoomCollison>();
                boomObj.transform.up = transform.up;

            }
        }

        /// <summary>
        /// 玩家返回移动
        /// </summary>
        public void PlayBackMove()
        {
            transform.position += transform.up * moveSpeed * Time.deltaTime;
        }

        /// <summary>
        /// 玩家前进移动
        /// </summary>
        public void PlayMoveForward()
        {
            transform.position += transform.up * -1 * moveSpeed * Time.deltaTime;
        }

        /// <summary>
        /// 检测玩家是否超出边界
        /// </summary>
        public void CheckExceed()
        {
            float x = transform.position.x;
            float y = transform.position.y;
            if (y < GameManager.Instance.minY || x > GameManager.Instance.maxX || x < GameManager.Instance.minX)
            {
                isBack = true; //超出边界开始返回
            }
        }

        /// <summary>
        /// 检测是否回到原点
        /// </summary>
        public void CheckMoveToStart()
        {
            float distance = Vector3.Distance(transform.position, playStartPoint);
            if (distance < 0.3f)
            {
                //若钩子已经接近回收 不能释放炸弹
                almostGet = true;

                transform.position = playStartPoint;
                if (chidrenObj != null)
                {
                    chidrenObj.UseProp();
                    chidrenObj.gameObject.SetActive(false);

                    //GameManager.Instance.poolData.Recycle(chidrenObj.gameObject);
                    hookGetAccount++;

                    Debug.Log(hookGetAccount);
                    Debug.Log(GameManager.Instance.levelObjs.Count);

                    //若场景物体被抓完 打开选择界面下一关
                    if (hookGetAccount == GameManager.Instance.levelObjs.Count)
                    {
                        hookGetAccount = 0;
                        GameManager.Instance.isPause = true;
                        GameManager.Instance.UIManager.OpenCloseSwitchPanel();
                    }
                        
                    chidrenObj = null;
                }

                isFire = false;
                isBack = false;
                RestMoveSpeed();

                //若此钩释放了炸弹 则不能继续释放直到钩子收回
                boomFire = false;
            }
            else
            {
                almostGet = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (chidrenObj != null)
            {
                return;
            }

            PropScript propScript = collision.gameObject.GetComponent<PropScript>();
            if (propScript != null)
            {
                float tempDistance = Vector3.Distance(transform.position, propScript.transform.position);
                propScript.transform.position = transform.position + transform.up * -1 * tempDistance * 0.75f;
                propScript.transform.SetParent(transform);
                chidrenObj = propScript;
                ComputeSpeed(propScript.scaleLevel);
                isBack = true;
            }
            else
            {
                Debug.Log("没有脚本");
            }
        }

        /// <summary>
        /// 计算玩家新的速度
        /// </summary>
        public void ComputeSpeed(int scaleLevel)
        {
            moveSpeed = moveSpeed - moveSpeed * 0.15f * scaleLevel;
        }

        /// <summary>
        /// 玩家速度重置
        /// </summary>
        public void RestMoveSpeed()
        {
            moveSpeed = startSpeed;
        }

        /// <summary>
        /// 玩家状态重置
        /// </summary>
        public void PlayStateRest()
        {
            if (isFire)
            {
                transform.position = playStartPoint;
            }

            if (chidrenObj != null)
            {
                chidrenObj.transform.SetParent(null);
            }

            isFire = false;
            isBack = false;
            RestMoveSpeed();
        }
    }
}