using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace BattleOfColthv2
{
	public abstract class Enemy
	{
		//declarations
		private GraphicsContext graphics;
		private Texture2D texture;
		private bool isAlive, isAttacking;
		private int hitPoints, shipPoints;
		private int shipTypeNum, projDmg, projSpeed;
		private long shotTime, shotClock;
		private string projTexture;
				
		//public Stuff
		public Sprite sprite;
		public Vector3 Pos {get { return sprite.Position; }}
		public float Rotation {get {return sprite.Rotation;}}
		public bool IsAlive {get {return isAlive;}}
		public bool IsAttacking {get {return isAttacking;}}
		public int ShipTypeNum {get {return shipTypeNum;}}
		public string ProjTexture {get {return projTexture;}}
		public int ProjDmg {get {return projDmg;}}
		public int ProjSpeed {get {return projSpeed;}}
		public int ShipPoints {get {return shipPoints;}}
		
		public Enemy ():this(null,null,0,0)
		{}
		public Enemy (GraphicsContext g, Texture2D tex, int hp, int st)
		{
			graphics = g;
			texture = tex;
			sprite = new Sprite(graphics,texture);
			hitPoints = hp;
			isAlive = true;
			shipTypeNum = st;
			
			if (shipTypeNum < 3)
			{
				projTexture = "/Application/Assets/BoomStick.png";
				projDmg = 10;
				projSpeed = 5;
				if(shipTypeNum == 1)
					shotTime = 500;
				else
					shotTime = 3000;
			}
			if (shipTypeNum == 3)
			{
				projTexture = "/Application/Assets/bullet.png";
				projDmg = 1;
				projSpeed = 10;
				shotTime = 1000;
			}
			//Points
			if(shipTypeNum == 3)
				shipPoints = 1;
			else if(shipTypeNum == 2)
				shipPoints = 3;
			else
				shipPoints = 10;
				
		}
		
		public void Update(long td)
		{
			
			if (hitPoints <= 0)
				isAlive = false;
			
			Move();
			
			//check timer for shooting
			shotClock += td;
			if (shotClock >shotTime)
			{
				isAttacking = DoAttack();
				if (isAttacking)
					shotClock =0;
			}
			else
				isAttacking = false;
		}
		
		public void TakeDamage(int dm)
		{
			hitPoints -= dm;
		}
				
		public abstract bool DoAttack();
		public abstract void Move();
		public abstract void Render ();
	}
}

