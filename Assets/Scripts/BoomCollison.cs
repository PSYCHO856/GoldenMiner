using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoldenMiner
{
    public class BoomCollison : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            Move();
        }

        public void Move()
        {
            transform.position += transform.up * -1 * 6 * Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {

            PropScript prop = collision.GetComponent<PropScript>();
            if (prop != null)
            {
                GameManager.Instance.BoomFunc();
                GameManager.Instance.poolData.Recycle(gameObject);
            }

            Debug.Log(collision.name + "=====");
        }
    }
}