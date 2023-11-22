using System.Collections;
using System.Collections.Generic;
using UltimateClean;
using UnityEngine;

public class ResultInfoWaitingRoom : MonoBehaviour
{
    private static ResultInfoWaitingRoom instance = null;
    public static ResultInfoWaitingRoom Instance
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
        Title = isSuccess ? "준비완료" : "시작불가";

        if (queue != null)
        {
            queue.EnqueueNotification(isSuccess == true ? PrefabSuccess : PrefabFail, Canvas, Type, Position, Duration, Title, message);
        }
        else
        {
            var go = Instantiate(isSuccess == true ? PrefabSuccess : PrefabFail);
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
