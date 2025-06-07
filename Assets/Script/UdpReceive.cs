using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using TMPro;

public class UdpReceive : MonoBehaviour
{
    private int R_port;
    public TextMeshProUGUI R_PortNumber;
    public TMP_InputField R_Port_InputField;

    private UdpClient udpClient;
    private Subject<string> subject = new Subject<string>();
    [SerializeField] TextMeshProUGUI message;

    void Start()
    {
        R_port = PlayerPrefs.GetInt("R_PORT", 64276);
        R_Port_InputField.text = R_port.ToString();
        R_PortNumber.text = R_port.ToString();

        udpClient = new UdpClient(R_port);
        udpClient.BeginReceive(OnReceived, udpClient);

        subject
            .ObserveOnMainThread()
            .Subscribe(msg => {
                message.text = msg;
            }).AddTo(this);
    }

    private void OnReceived(System.IAsyncResult result)
    {
        UdpClient getUdp = (UdpClient)result.AsyncState;
        IPEndPoint ipEnd = null;

        byte[] getByte = getUdp.EndReceive(result, ref ipEnd);

        var message = Encoding.UTF8.GetString(getByte);
        subject.OnNext(message);
        Debug.Log(message);

        getUdp.BeginReceive(OnReceived, getUdp);
    }

    private void OnDestroy()
    {
        udpClient.Close();
    }

    public void SetText()
    {
        if (udpClient != null)
        {
            udpClient.Close();
            udpClient = null;
        }

        if (R_Port_InputField.text == "" || int.Parse(R_Port_InputField.text) >= 65536 || int.Parse(R_Port_InputField.text) <= -1)
        {
            R_Port_InputField.text = R_port.ToString();
        }

        R_port = int.Parse(R_Port_InputField.text);
        R_Port_InputField.text = R_port.ToString();
        R_PortNumber.text = R_port.ToString();

        PlayerPrefs.SetInt("R_PORT", R_port);
        PlayerPrefs.Save();

        udpClient = new UdpClient(R_port);
        udpClient.BeginReceive(OnReceived, udpClient);
    }
}
