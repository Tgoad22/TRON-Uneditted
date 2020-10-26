using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class SnakePlayer
{
    public enum Direction { up, down, left, right };

	public class TailPiece {
		private TailPiece(){}
		public TailPiece(float X, float Y, int ttl, GameObject vis)
		{
			Xpos = X;
			Ypos = Y;
			lifetime = ttl;
			visual = vis;
		}
		public float Xpos;
		public float Ypos;
		public int lifetime;
		public GameObject visual;
	}

    private SnakePlayer() { }
    public SnakePlayer(float X, float Y, GameObject shade, Sprite tail, Board them) //Color shade) Skipping for now
    {
        this.X = X;
        this.Y = Y;
        lives = 5;
        currDir = Direction.up;
		visual = shade;
		this.tail = new List<TailPiece>();
		tailColor = tail;
		tailTimer = -1;
		parent = them;
    }

	public Board parent;
    public float X;
    public float Y;
    public int lives;
    public Direction currDir;
    public float Speed = 0.11f;
	public GameObject visual;
	public int tailTimer;
	public List<TailPiece> tail;
	public Sprite tailColor;
	public float hitdist = 0.1f;
	public int invincible;

    public bool CheckCollide(float X, float Y)
    {
        bool collision = false;
        if (CheckCollideHead(X, Y))
        {
            collision = true;
        }
        else if (CheckCollideTail(X, Y))
        {
            collision = true;
        }
        return collision;
    }
	
	public bool CheckCollideTail(float X, float Y)
	{
        bool collision = false;
        foreach (TailPiece piece in tail)
		{
			if ((piece.Xpos - hitdist < X && piece.Xpos + hitdist > X) && (piece.Ypos - hitdist < Y && piece.Ypos + hitdist > Y))
			{
				collision = true;
				break;
			}
		}
        return collision;
	}

    public bool CheckCollideHead(float X, float Y)
    {
        return (this.X - hitdist < X && this.X + hitdist > X) && (this.Y - hitdist < Y && this.Y + hitdist > Y);
    }

    public void Draw()
    {
		visual.GetComponent<Transform>().position = new Vector3(X, Y, -50);
    }

    public void Move()
    {
        switch(currDir)
        {
            case Direction.up:
                Y += Speed;
                break;
            case Direction.down:
                Y -= Speed;
                break;
            case Direction.left:
                X -= Speed;
                break;
            case Direction.right:
                X += Speed;
                break;
        }
		//Check self collision
		if (CheckCollideTail(X, Y))
		{
			Die();
		}
		TailPiece newTrail = new TailPiece(X, Y, tailTimer, new GameObject("tail", typeof(SpriteRenderer)));
		tail.Add(newTrail);
		newTrail.visual.GetComponent<Transform>().position = new Vector3(newTrail.Xpos, newTrail.Ypos, -40);
		newTrail.visual.GetComponent<SpriteRenderer>().sprite = tailColor;
    }

    public void ChangeDir(Direction dir)
    {

        if (currDir == Direction.down && dir == Direction.up)
        {
            return;
        }
        else if (currDir == Direction.up && dir == Direction.down)
        {
            return;
        }
        else if (currDir == Direction.left && dir == Direction.right)
        {
            return;
        }
        else if (currDir == Direction.right && dir == Direction.left)
        {
            return;
        }
        else
        {
            currDir = dir;
			int facing = 0;
			switch (dir) {
				case Direction.up :
					facing = 0;
					break;
				case Direction.down :
					facing = 180;
					break;
				case Direction.right :
					facing = -90;
					break;
				case Direction.left :
					facing = 90;
					break;
			}
			visual.GetComponent<Transform>().eulerAngles = new Vector3(0, 0, facing);
        }
    }

    public void Die()
    {
		if (invincible > 0) {
			return;
		}
		GameObject.Find("CrashSound").GetComponent<AudioSource>().Play();
		foreach (TailPiece piece in tail) {
			GameObject.Destroy(piece.visual);
		}
		tail = new List<TailPiece>();
		lives--;
		invincible = 60;
		Vector2 pos = parent.GetSafePos();
		X = pos.x;
		Y = pos.y;
    }

    // Update is called once per frame
    public void Update()
    {
		//Tail updates
		tail.ForEach(x => x.lifetime -= 1);
		tail.FindAll(x => x.lifetime == 0).ForEach(x => GameObject.Destroy(x.visual));
		tail.RemoveAll(x => x.lifetime == 0);
		//Self updates
		if (invincible > 0) {
			invincible--;
		}
    }
}
