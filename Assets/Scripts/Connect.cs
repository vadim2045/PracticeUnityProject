using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ConnectToServer : MonoBehaviour
{
    private class Save
    {
        public string Key;
        public string Rooms;
        public List<Robot> robots;
    }

    private class Robot
    {
        public string Name;
        public string Algorithm;
        /*
        0 Ц движение
        1 Ц повернутьс€
        Х	0 Ц налево
        Х	1 Ц направо
        2 Ц положить/подн€ть
        3 Ц цикл
        4 - ждать
        ‘лаг повторить - 0, один раз - 1
        */
        public Robot(string Name, string Algorithm)
        {
            this.Name = Name;
            this.Algorithm = Algorithm;
        }
    }

    private const string remoteHost = "5.167.50.122";
    private const int remotePort = 443;

    public void Connect()
    {
        try
        {
            TcpClient client = new TcpClient();
            client.Connect(remoteHost, remotePort);
            using (NetworkStream netStream = client.GetStream())
            using (SslStream sslStream = new SslStream(netStream, false,
                (sender, cert, chain, errors) => true))
            {
                sslStream.AuthenticateAsClient(remoteHost);
                SendString(sslStream, "v1\\FACTORYGAME_PROTOCOL.PROT");
                //
                Save save = new Save();
                save.Key = "2u8I34Hh";
                save.Rooms = "100010000000";
                List<Robot> robots = new List<Robot>();
                robots.Add(new Robot("¬асилий", "1:1;0:1;2;2;3(1:0):10;0:1;1:0;0:5;2.1"));
                save.robots = robots;
                //
                Debug.Log("ѕодключилс€ игрок с ключем " + save.Key);
                int k = 0;
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 3; j++)
                    {
                        Debug.Log(" омната [" + i + ";" + j + "]: ");
                        if (save.Rooms[k] == '1')
                            Debug.Log("разблокирована");
                        else Debug.Log("заблокирована");
                        k++;
                    }
                Debug.Log("–оботы игрока " + save.Key + ":");
                foreach (Robot robot in robots)
                {
                    Debug.Log("–обот " + robot.Name + " с алгоритмом");
                    AlgotirhmParse(robot.Algorithm);
                }
            }
        }
        catch (Exception ex)
        { Debug.Log(ex.Message); }
    }

    private void AlgotirhmParse(string alg, bool cicle = false)
    {
        for (int i = 0; i < alg.Length; i++)
        {
            if (alg[i] == '0' && alg[i + 1] == ':')
            {
                i += 2;
                StringBuilder sb = new StringBuilder();
                while (alg[i] != ';')
                {
                    sb.Append(alg[i]);
                    i++;
                }
                Debug.Log(cicle ? "\t" : "" + "ƒвижение на " + sb.ToString() + " клеток");
            }
            else if (alg[i] == '1' && alg[i + 1] == ':')
            {
                i += 2;
                Debug.Log(cicle ? "\t" : "" + "ѕоворот " + (alg[i] == '0' ? "налево" : "направо"));
            }
            else if (alg[i] == '2')
            {
                Debug.Log(cicle ? "\t" : "" + "ѕоложить/подн€ть");
            }
            //if (alg[i] == '0' && alg[i + 1] == ':')
            //{
            //    i += 2;
            //    StringBuilder sb = new StringBuilder();
            //    while (alg[i] != ';')
            //    {
            //        sb.Append(alg[i]);
            //        i++;
            //    }
            //    Debug.Log(cicle ? "\t" : "" + "ƒвижение на " + sb.ToString() + " клеток");
            //}
        }
    }

    private void SendString(SslStream sslStream, string toSend)
    {
        byte[] strBytes = Encoding.UTF8.GetBytes(toSend);
        byte[] strLenBytes = BitConverter.GetBytes(strBytes.Length);
        sslStream.Write(strLenBytes, 0, 4);
        sslStream.Write(strBytes, 0, strBytes.Length);
    }

    private string GetString(SslStream sslStream)
    {
        byte[] strLenBuffer = new byte[4];
        ReadExact(sslStream, strLenBuffer);
        int strLength = BitConverter.ToInt32(strLenBuffer, 0);
        byte[] strBuffer = new byte[strLength];
        ReadExact(sslStream, strBuffer);
        string recievedStr = Encoding.UTF8.GetString(strBuffer);
        return recievedStr;
    }

    private int ReadExact(Stream stream, byte[] buffer)
    {
        int totalRead = 0;
        while (totalRead < buffer.Length)
        {
            int bytesRead = stream.Read(buffer, totalRead, buffer.Length - totalRead);
            if (bytesRead == 0) throw new IOException("—оединение разорвано удаленной стороной.");
            totalRead += bytesRead;
        }
        return totalRead;
    }
}