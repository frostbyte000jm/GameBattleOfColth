using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace BattleOfColthv2
{
	public class PowerStation
	{
		//declarations
		private Texture2D texture;
		private Vector3 position;
		private Sprite sprite;
		private GraphicsContext graphics;
		private float spriteHeight, spriteWidth;
		private int hitPoints;
		private bool isAlive;
		
		public bool IsAlive {get {return isAlive;}}
		public Vector3 Position {get {return position;}}
		
		public PowerStation (GraphicsContext g)
		{
			graphics = g;
			texture = new Texture2D("/Application/Assets/PowerStation.png", false);
			sprite = new Sprite(graphics,texture);
			spriteWidth = sprite.Width;
			spriteHeight = sprite.Height;
			position = new Vector3(graphics.Screen.Rectangle.Width - spriteWidth/2,graphics.Screen.Rectangle.Height - spriteHeight/2,0);
			sprite.Position = position;
			sprite.Center = new Vector2 (.5f,.5f);
			isAlive = true;
			
			hitPoints = 500;
		}
		
		public void Update()
		{
			if (hitPoints <= 0)
				isAlive = false;
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

