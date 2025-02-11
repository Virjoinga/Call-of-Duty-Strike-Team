using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class PopulateDebugAssetServerList : MonoBehaviour
{
	public GameObject PrefabButton;

	public UIScrollList ScrollList;

	public string[] ServerIps;

	private void Start()
	{
		if (Application.CanStreamedLevelBeLoaded(15))
		{
			DebugDlcServer.Ip = "127.0.0.1";
			Application.LoadLevel("dlc_loader");
			return;
		}
		if (ScrollList == null)
		{
			ScrollList = base.gameObject.GetComponent<UIScrollList>();
		}
		AddRescanButton();
		StartCoroutine("ScanForAssetServers");
	}

	private void AddRescanButton()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(PrefabButton) as GameObject;
		UIButton component = gameObject.GetComponent<UIButton>();
		component.Text = "Rescan For Servers";
		component.AddValueChangedDelegate(RescanForAssetServers);
		component.SetColor(Color.blue);
		ScrollList.AddItem(gameObject);
		string[] serverIps = ServerIps;
		foreach (string text in serverIps)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(PrefabButton) as GameObject;
			UIButton component2 = gameObject2.GetComponent<UIButton>();
			component2.Text = text + " checking";
			component2.AddValueChangedDelegate(SetDesiredDlcServer);
			ScrollList.AddItem(gameObject2);
		}
	}

	private void RescanForAssetServers(IUIObject obj)
	{
		ScrollList.ClearList(true);
		AddRescanButton();
		StartCoroutine("ScanForAssetServers");
	}

	private IEnumerator ScanForAssetServers()
	{
		int buttonsIdx = 1;
		string[] serverIps = ServerIps;
		foreach (string ip in serverIps)
		{
			Ping ping = new Ping(ip);
			yield return new WaitForSeconds(0.5f);
			IUIListObject listObj = ScrollList.GetItem(buttonsIdx);
			GameObject button = listObj.gameObject;
			UIButton butComp = button.GetComponent<UIButton>();
			if (ping.time >= 0)
			{
				butComp.Text = ip + string.Empty;
				int portno = 8000;
				string[] ipBits = ip.Split('.');
				byte[] ipBytes = new byte[4]
				{
					byte.Parse(ipBits[0]),
					byte.Parse(ipBits[1]),
					byte.Parse(ipBits[2]),
					byte.Parse(ipBits[3])
				};
				IPAddress ipa = new IPAddress(ipBytes);
				string hostName2 = Dns.GetHostEntry(ipa).HostName;
				hostName2 = hostName2.Replace(".activision.com", string.Empty);
				butComp.SetColor(Color.red);
				Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
				{
					Blocking = false
				};
				IAsyncResult result = sock.BeginConnect(ipa, portno, null, null);
				yield return new WaitForSeconds(0.5f);
				if (!result.IsCompleted)
				{
					yield return new WaitForSeconds(0.5f);
				}
				if (result.IsCompleted && sock.Connected)
				{
					butComp.Text = hostName2 + " RUNNING";
					butComp.SetColor(Color.green);
				}
				else if (!result.IsCompleted)
				{
					butComp.Text = hostName2 + " SLEEPING";
					butComp.SetColor(Color.grey);
				}
				else
				{
					butComp.Text = hostName2 + " NOT RUNNING";
				}
				sock.Close();
			}
			else
			{
				butComp.Text = ip + " ERR";
			}
			buttonsIdx++;
		}
	}

	private void SetDesiredDlcServer(IUIObject obj)
	{
		UIButton component = obj.gameObject.GetComponent<UIButton>();
		string ip = component.Text.Split(' ')[0];
		DebugDlcServer.Ip = ip;
		Application.LoadLevel("dlc_loader");
	}

	private void Update()
	{
	}
}
