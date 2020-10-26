using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//The Interface class itself

public class Interface : MonoBehaviour
{
	
	private GameObject MainCamera;
	public GameObject CameraPlayer1;
	public GameObject CameraPlayer2;
	private screenMode mode;
	public int winner;
	private GameObject[] winmsgs;
	public Texture screenborder;
	public Sprite player1Tail;
	public Sprite player2Tail;
	public Sprite playborder;
	
	public enum Screen { None, Title, Play, Over };
	
    // Start is called before the first frame update
    void Start()
    {
		MainCamera = GameObject.Find("Main Camera");
		State(Screen.Title);
		winmsgs = new GameObject[] {
			GameObject.Find("Tie"),
			GameObject.Find("WinMsg1"),
			GameObject.Find("WinMsg2")
		};
    }

    // Update is called once per frame
	void Update()
	{
		mode.Update(this);
	}
	
	void OnGUI()
	{
		mode.OnGUI(this);
	}
	
	public void State(Screen n)
	{
		switch (n) {
			case Screen.Title :
			    MainCamera.GetComponent<Camera>().enabled = true;
				CameraPlayer1.GetComponent<Camera>().enabled = false;
				CameraPlayer2.GetComponent<Camera>().enabled = false;
				MainCamera.GetComponent<Transform>().position = new Vector3(-100, -124, -100);
				mode = new Title();
				break;
			case Screen.Play :
				MainCamera.GetComponent<Camera>().enabled = false;
				CameraPlayer1.GetComponent<Camera>().enabled = true;
				CameraPlayer2.GetComponent<Camera>().enabled = true;
				mode = new Play(2, 25, 25, this);
				break;
			case Screen.Over :
			    MainCamera.GetComponent<Camera>().enabled = true;
				CameraPlayer1.GetComponent<Camera>().enabled = false;
				CameraPlayer2.GetComponent<Camera>().enabled = false;
				MainCamera.GetComponent<Transform>().position = new Vector3(-120, -124, -100);
				foreach (GameObject msg in winmsgs) {
					msg.GetComponent<Transform>().position = new Vector3(-120, -1240, -100);
				}
				winmsgs[winner].GetComponent<Transform>().position = new Vector3(-120, -124, -50);
				mode = new Over(winner);
				break;
			default :
				break;
		}
	}
}

interface screenMode
{
	void Update(Interface parent);
	void OnGUI(Interface parent);
	void Hurt(Interface parent, SnakePlayer n);
}

//Handles Title Screen control
public class Title : screenMode
{
	public Title() {
		GameObject.Find("EndMusic").GetComponent<AudioSource>().Stop();
		GameObject.Find("MenuMusic").GetComponent<AudioSource>().Play();
	}
	public void Update(Interface parent)
	{
		if (Input.GetButton("Player1Fire") && Input.GetButton("Player2Fire"))
		{
			parent.State(Interface.Screen.Play);
		}
	}
	public void OnGUI(Interface parent)
	{
		;
	}
	public void Hurt(Interface parent, SnakePlayer n)
	{
		;
	}
}

//Handles Game Over Screen control
public class Over : screenMode
{
	private Over(){}
	public Over(int winner)
	{
		this.winner = winner;
		GameObject.Find("GameMusic").GetComponent<AudioSource>().Stop();
		GameObject.Find("EndMusic").GetComponent<AudioSource>().Play();
	}
	
	private int winner;
	private bool returning = false;
	public void Update(Interface parent)
	{
		if (Input.GetButton("Player1Fire") && Input.GetButton("Player2Fire"))
		{
			returning = true;
		}
		if (returning && !Input.GetButton("Player1Fire") && !Input.GetButton("Player2Fire"))
		{
			parent.State(Interface.Screen.Title);
		}
	}
	public void OnGUI(Interface parent)
	{
		;
	}
	public void Hurt(Interface parent, SnakePlayer n)
	{
		;
	}
}

//Handles Gameplay Screen control
public class Play : screenMode
{
	private int[] lifecount;
	private Board playboard;
	
	private Play(){}	//No parameterless constructor
	public Play(int playerCount, float boardSizeX, float boardSizeY, Interface parent)
	{
		lifecount = new int[playerCount];
		playboard = new Board(boardSizeX, boardSizeY, parent);
		GameObject.Find("GameMusic").GetComponent<AudioSource>().Play();
		GameObject.Find("MenuMusic").GetComponent<AudioSource>().Stop();
	}
	
    public void Update(Interface parent)
    {
		if (Input.GetButtonDown("Player1Up")) {
			playboard.playerList[0].ChangeDir(SnakePlayer.Direction.up);
		}
		if (Input.GetButtonDown("Player1Down")) {
			playboard.playerList[0].ChangeDir(SnakePlayer.Direction.down);
		}
		if (Input.GetButtonDown("Player1Left")) {
			playboard.playerList[0].ChangeDir(SnakePlayer.Direction.left);
		}
		if (Input.GetButtonDown("Player1Right")) {
			playboard.playerList[0].ChangeDir(SnakePlayer.Direction.right);
		}
		if (Input.GetButtonDown("Player2Up")) {
			playboard.playerList[1].ChangeDir(SnakePlayer.Direction.up);
		}
		if (Input.GetButtonDown("Player2Down")) {
			playboard.playerList[1].ChangeDir(SnakePlayer.Direction.down);
		}
		if (Input.GetButtonDown("Player2Left")) {
			playboard.playerList[1].ChangeDir(SnakePlayer.Direction.left);
		}
		if (Input.GetButtonDown("Player2Right")) {
			playboard.playerList[1].ChangeDir(SnakePlayer.Direction.right);
		}
        playboard.Collide();
		playboard.Update();
		var cam1trans = parent.CameraPlayer1.GetComponent<Transform>();
		var cam2trans = parent.CameraPlayer2.GetComponent<Transform>();
		cam1trans.position = new Vector3(playboard.playerList[0].X, playboard.playerList[0].Y, cam1trans.position.z);
		cam2trans.position = new Vector3(playboard.playerList[1].X, playboard.playerList[1].Y, cam2trans.position.z);
    }
	
	public void OnGUI(Interface parent)
	{
		playboard.Draw();
		GUI.DrawTexture(new Rect(Screen.width/2-10, -10, 30, Screen.height+20), parent.screenborder);
	}
	
	public void Hurt(Interface parent, SnakePlayer n)
	{
		lifecount[Array.IndexOf(playboard.playerList, n)]--;
		if (lifecount[0] == 0 && lifecount[1] == 0)
		{
			parent.winner = 0;
			playboard.GameEnd();
			parent.State(Interface.Screen.Over);
		}
	}
}

