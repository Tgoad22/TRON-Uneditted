using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
	public Board(float Xsize, float Ysize, Interface parent)
	{
		playerList = new SnakePlayer[2];
		playerList[0] = new SnakePlayer(5, 10, GameObject.Find("Player1"), parent.player1Tail, this);
		playerList[1] = new SnakePlayer(15, 10, GameObject.Find("Player2"), parent.player2Tail, this);
		powerupList = new List<PowerUp>();
		maxX = Xsize;
		maxY = Ysize;
		this.parent = parent;
		GameObject border;
		border = new GameObject("BorderBottom", typeof(SpriteRenderer));
		border.GetComponent<Transform>().position = new Vector3(0, 0, -50);
		border.GetComponent<SpriteRenderer>().sprite = parent.playborder;
		border.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Tiled;
		border.GetComponent<SpriteRenderer>().size = new Vector2(Xsize, 0.5f);
		border = new GameObject("BorderLeft", typeof(SpriteRenderer));
		border.GetComponent<Transform>().position = new Vector3(0, 0, -50);
		border.GetComponent<Transform>().Rotate(0, 0, 90);
		border.GetComponent<SpriteRenderer>().sprite = parent.playborder;
		border.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Tiled;
		border.GetComponent<SpriteRenderer>().size = new Vector2(Ysize, 0.5f);
		border = new GameObject("BorderRight", typeof(SpriteRenderer));
		border.GetComponent<Transform>().position = new Vector3(Xsize, 0, -50);
		border.GetComponent<Transform>().Rotate(0, 0, 90);
		border.GetComponent<SpriteRenderer>().sprite = parent.playborder;
		border.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Tiled;
		border.GetComponent<SpriteRenderer>().size = new Vector2(Ysize, 0.5f);
		border = new GameObject("BorderTop", typeof(SpriteRenderer));
		border.GetComponent<Transform>().position = new Vector3(0, Ysize, -50);
		border.GetComponent<SpriteRenderer>().sprite = parent.playborder;
		border.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Tiled;
		border.GetComponent<SpriteRenderer>().size = new Vector2(Xsize, 0.5f);
	}
	
	public SnakePlayer[] playerList;
	public List<PowerUp> powerupList;
	public float maxX;
	public float maxY;
	public Interface parent;
	
	public void GameEnd()
	{
		playerList = null;
		powerupList = null;
		GameObject.Destroy(GameObject.Find("BorderBottom"));
		GameObject.Destroy(GameObject.Find("BorderTop"));
		GameObject.Destroy(GameObject.Find("BorderLeft"));
		GameObject.Destroy(GameObject.Find("BorderRight"));
	}
	public void Collide()
	{
		foreach (SnakePlayer player in playerList) {
		    //Players touching powerups
			foreach (PowerUp powerup in powerupList) {
				if (player.CheckCollideHead(powerup.X, powerup.Y)) {
					powerup.Effect(player);
				}
			}
			//Players touching walls
			if (player.Y > maxY || player.Y < 0 || player.X > maxX || player.X < 0) {
				player.Die();
			}
		}
		   //Players touching players
		if (playerList[0].CheckCollide(playerList[1].X, playerList[1].Y)) {
			//Two hit one
			playerList[1].Die();
		}
		if (playerList[1].CheckCollide(playerList[0].X, playerList[0].Y)) {
			//One hit two
			playerList[0].Die();
		}
	}
	
	public Vector2 GetSafePos()
	{
		Vector2 pos = new Vector2(0, 0);
		while (true) {
			pos.x = Random.Range(1.0f, maxX-1);
			pos.y = Random.Range(1.0f, maxY-1);
			foreach (SnakePlayer player in playerList) {
				//Avoid players
				if (player.CheckCollide(pos.x, pos.y)) {
					continue;
				}
			}
			break;
		}
		return pos;
	}
	
	public void Update()
	{
		foreach (SnakePlayer player in playerList) {
			player.Update();
			player.Move();
		}
		if (playerList[0].lives == 0 && playerList[1].lives == 0) {
			parent.winner = 0;
			parent.State(Interface.Screen.Over);
		} else if (playerList[0].lives == 0) {
			parent.winner = 2;
			parent.State(Interface.Screen.Over);
		} else if (playerList[1].lives == 0) {
			parent.winner = 1;
			parent.State(Interface.Screen.Over);
		}
	}
	public void Draw()
	{
		foreach (SnakePlayer player in playerList) {
			player.Draw();
		}
	}
}
