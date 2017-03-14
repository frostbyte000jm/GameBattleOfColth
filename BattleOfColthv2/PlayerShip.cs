using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace BattleOfColthv2
{
	public class PlayerShip
	{
		//declarations
		private Texture2D texture;
		private Vector3 position, velocityFwd;
		private Sprite sprite;
		private GraphicsContext graphics;
		private int speed, weaponIndex, hitPoints, points;
		private float shipHeight, shipWidth, maxRight, maxDown;
		private bool isAlive;
		
		public Vector3 Position {get {return sprite.Position;}}
		public float Rotation {get {return sprite.Rotation;}}
		public int WeaponIndex {get {return weaponIndex;} set{weaponIndex = value;}}//You may need to think about this more. 
		public int Points {get {return points;} set{if(value>0) points += value;}}
		public bool IsAlive {get {return isAlive;}}
				
		public PlayerShip (GraphicsContext g)
		{
			graphics = g;
			texture = new Texture2D("/Application/Assets/Player Ship.png", false);
			sprite = new Sprite(graphics,texture);
			shipWidth = texture.Width;
			shipHeight = texture.Height;
			position = new Vector3(graphics.Screen.Rectangle.Width - shipWidth,graphics.Screen.Rectangle.Height - shipHeight,0);
			sprite.Position = position;
			sprite.Center = new Vector2 (.5f,.5f);
			velocityFwd = new Vector3 (1,0,0);
			speed = 5;
			weaponIndex =0;
			hitPoints = 50;
			isAlive = true;
			points = 0;
		}
		
		public void Update(GamePadData gamePadData)
		{
			if (hitPoints <= 0)
				isAlive = false;

			Move (gamePadData);
		}
		
		private void Move (GamePadData gamePadData)
		{
			
			var pressedButtons = gamePadData.ButtonsDown;

			velocityFwd = new Vector3((float)Math.Cos(sprite.Rotation)*speed, (float)Math.Sin(sprite.Rotation)*speed, 0);
			
			if ((gamePadData.Buttons & GamePadButtons.Up) !=0)
			{
				sprite.Position += velocityFwd;
			}
			if ((gamePadData.Buttons & GamePadButtons.Down) !=0)
			{
				sprite.Position -= velocityFwd;
			}
			if ((gamePadData.Buttons & GamePadButtons.Left) !=0)
			{
				sprite.Rotation -= .05f;
			}
			if ((gamePadData.Buttons & GamePadButtons.Right) !=0)
			{
				sprite.Rotation += .05f;
			}
			
			// creating some max movements here.
			maxRight = graphics.Screen.Rectangle.Width;
			maxDown = graphics.Screen.Rectangle.Height;
			if (sprite.Position.X < 0)
				sprite.Position.X = 0;
			else if (sprite.Position.X > maxRight)
				sprite.Position.X = maxRight;
			else if (sprite.Position.Y < 0)
				sprite.Position.Y = 0;
			else if (sprite.Position.Y > maxDown)
				sprite.Position.Y = maxDown;
		}
		
		public void TakeDamage(int dm)
		{
			hitPoints -= dm;
		}
		
		public void Render ()
		{
			sprite.Render ();
		}
	}
}

