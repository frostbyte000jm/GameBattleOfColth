using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace BattleOfColthv2
{
	public abstract class Weapon
	{
		private int uniqueID;
		private int ammo;
		private int damage;
		
		public PlayerShip player;
		public Sprite sprite;
		public Vector3 Pos {get { return sprite.Position; }	set { sprite.Position = value; }}
		public float Rotation {get { return sprite.Rotation; }set { sprite.Rotation = value; }}
		public int Ammo {get { return ammo; }set { ammo += value; }}
		public int Damage {get {return damage;} set{ damage = value;}}
		public int UniqueID {get {return uniqueID;}}
         
		public Weapon (GraphicsContext g, Texture2D tex, PlayerShip pl, int uid)
		{
			player = pl;
			uniqueID = uid;
			damage = 0;
			
			//create
			sprite = new Sprite (g, tex);
			sprite.Position = pl.Position;
			sprite.Center = new Vector2 (.5f, .5f);
     
			sprite.Rotation = pl.Rotation;
		}
		
		//there has to be a way to do this (crap inside) only once. it must be something simple and stupid if I can't remember. 
		public Weapon (GraphicsContext g, Texture2D tex, Vector3 p, float rot, int uid)
		{
			sprite = new Sprite (g, tex);
			sprite.Position = p;
			sprite.Center = new Vector2 (.5f, .5f);
     		sprite.Rotation = rot;
           	uniqueID = uid;
			damage = 0;
		}
		
		public bool Shoot()
		{
			bool doShoot = false;
			if( ammo >0)
			{
				ammo--;
				doShoot = true;
			}
			return doShoot;
		}

		public abstract void Reload();
		public abstract void Update();
		public abstract void Render ();
	}
}

