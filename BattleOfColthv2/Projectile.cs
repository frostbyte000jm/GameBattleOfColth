using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace BattleOfColthv2
{
	public class Projectile
	{
		private Vector3 velocityFwd;
		private Vector3 center;
		private Sprite sprite;
		private int speed, damage;
		private bool isAlive;
		private float maxRight, maxDown;
		private GraphicsContext graphics;
		
		public bool IsAlive {get {return isAlive;}}
		public Vector3 Pos {get { return sprite.Position; }}
		public int Damage {get {return damage;}}
		
		public Projectile (GraphicsContext g, Texture2D tex, PlayerShip pl, int spd, int dmg)
		{
			graphics = g;
			sprite = new Sprite (graphics, tex);
			sprite.Position = pl.Position;
			sprite.Center = new Vector2 (.5f, .5f);
			speed = spd;
			center = new Vector3 (sprite.Position.X + 5f, sprite.Position.Y + 5f, 0);
			sprite.Rotation = pl.Rotation;
			isAlive = true;
			damage = dmg;
		}
		
		public Projectile (GraphicsContext g, Texture2D tex, Enemy en, int spd, int dmg)
		{
			graphics = g;
			sprite = new Sprite (graphics, tex);
			sprite.Position = en.Pos;
			sprite.Center = new Vector2 (.5f, .5f);
			speed = spd;
			center = new Vector3 (sprite.Position.X + 5f, sprite.Position.Y + 5f, 0);
			sprite.Rotation = en.Rotation;
			isAlive = true;
			damage = dmg;
		}
		
		public void Update()
		{
			velocityFwd = new Vector3((float)Math.Cos(sprite.Rotation)*speed, (float)Math.Sin(sprite.Rotation)*speed, 0);
			sprite.Position += velocityFwd;
			
			CheckOnScreen();
		}
		
		public void Render()
		{
			sprite.Render();
		}
		
		private void CheckOnScreen()
		{
			// creating some max movements here.
			maxRight = graphics.Screen.Rectangle.Width + 10f;
			maxDown = graphics.Screen.Rectangle.Height + 10f;
			if (sprite.Position.X < -10)
				isAlive = false;
			else if (sprite.Position.X > maxRight)
				isAlive = false;
			else if (sprite.Position.Y < -10)
				isAlive = false;
			else if (sprite.Position.Y > maxDown)
				isAlive = false;
		}
	}
}

