// Copyright (C) 2015-2021 gamevanilla - All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement.
// A Copy of the Asset Store EULA is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace UltimateClean
{
    /// <summary>
    /// The component used for launching notifications.
    /// </summary>
    public class ResultInfo : MonoBehaviour
    {
        private static ResultInfo instance = null;
        public static ResultInfo Instance
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
        public GameObject PrefabSuccess;
        public GameObject PrefabFail;
        public GameObject PrefabWrong;
        public Canvas Canvas;

        public NotificationType Type;
        public NotificationPositionType Position;

        public float Duration;
        public string Title;
        public string Message;

        private NotificationQueue queue;

        private void Start()
        {
            queue = FindObjectOfType<NotificationQueue>();
            if (Canvas == null)
            {
                Canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            }
        }

        public void LaunchNotification(bool isSuccess, string message)
        {
            Title = isSuccess ? "성공" : "실패";
            if (queue != null)
            {
                queue.EnqueueNotification(isSuccess == true ? PrefabSuccess : PrefabFail, Canvas, Type, Position, Duration, Title, message);
            }
            else
            {
                var go = Instantiate(isSuccess == true ? PrefabSuccess: PrefabFail);
                go.transform.SetParent(Canvas.transform, false);

                var notification = go.GetComponent<Notification>();
                notification.Launch(Type, Position, Duration, Title, message);
            }
        }
        public void InputWrong(string message)
        {
            Title = "부적합";
            if (queue != null)
            {
                queue.EnqueueNotification(PrefabWrong, Canvas, Type, Position, Duration, Title, message);
            }
            else
            {
                var go = Instantiate(PrefabWrong);
                go.transform.SetParent(Canvas.transform, false);

                var notification = go.GetComponent<Notification>();
                notification.Launch(Type, Position, Duration, Title, message);
            }
        }
    }
}