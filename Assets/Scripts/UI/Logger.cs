using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Logger
{
    #region Methods
    public static void SendMessageToUI(string message)
    {
        Data.UIController.ReceiveMessage(message);
    }
    #endregion Methods
}