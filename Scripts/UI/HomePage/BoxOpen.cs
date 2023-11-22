using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UIElements;

namespace ARPGFX
{

    public class BoxOpen : MonoBehaviour
    {
        private static BoxOpen instance = null;
        public static BoxOpen Instance
        {
            get { return instance; }
        }

        public void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
        public GameObject boxUi;

        [SerializeField]
        List<GameObject> listOfEffectsNoraml;
        [SerializeField]
        List<GameObject> listOfEffectsEpic;
        [SerializeField]
        List<GameObject> listOfEffectsUnique;
        [SerializeField]
        List<GameObject> listOfEffectsLegendary;

        [Header("Loop length in seconds")]
        [SerializeField]
        float loopTimeLength;

        float timeOfLastInstantiate;

        GameObject instantiatedEffect;

        int effectIndex = 0;
        string nowGrade;
        // Use this for initialization
        void Start()
        {
            //Invoke("Waiting", 0.5f);
        }
        public void Open(string result)
        {
            boxUi.SetActive(true);
            ShowResultCos.Instance.CallStart(); // 버튼 비활성화.
            nowGrade = result;
            Invoke("Waiting", 0.5f);
        }
        public void SelectEffect(int index)
        {
            // 등급에 맞는 색상 지정
            switch (nowGrade)
            {
                case "normal":
                    instantiatedEffect = Instantiate(listOfEffectsNoraml[index], transform.position, transform.rotation) as GameObject;
                    break;
                case "epic":
                    instantiatedEffect = Instantiate(listOfEffectsEpic[index], transform.position, transform.rotation) as GameObject;
                    break;
                case "unique":
                    instantiatedEffect = Instantiate(listOfEffectsUnique[index], transform.position, transform.rotation) as GameObject;
                    break;
                case "legendary":
                    instantiatedEffect = Instantiate(listOfEffectsLegendary[index], transform.position, transform.rotation) as GameObject;
                    break;
            }
        }
        public void Waiting()
        {
            effectIndex = 0;
            SelectEffect(effectIndex);
            effectIndex++;
            Invoke("OpenBox", 1.5f);
        }
        public void OpenBox()
        {
            Destroy(instantiatedEffect);
            SelectEffect(effectIndex);
            Invoke("CloseBox", 1.0f);
        }
        public void CloseBox()
        {
            Destroy(instantiatedEffect);
            boxUi.SetActive(false);
        }
        public void ReOpen()
        {
            boxUi.SetActive(true);
            Invoke("Waiting", 0.5f);
        }
    }
}
