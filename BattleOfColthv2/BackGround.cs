using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace BattleOfColthv2
{
	public class BackGround
	{
		//declarations
		private Sprite imageBackground;
		private Texture2D imageTexture;
		
		public BackGround (GraphicsContext gc, Texture2D tx)
		{
			//load
			GraphicsContext graphics = gc;
			imageTexture = tx;
			
			//create
			imageBackground = new Sprite(graphics, imageTexture);
			
			//place
			imageBackground.Position.X = graphics.Screen.Rectangle.Width / 2 - imageBackground.Width/2;
			imageBackground.Position.Y = graphics.Screen.Rectangle.Height / 2 - imageBackground.Height/2;
		}
		
		public void Render()
		{
			imageBackground.Render();
		}
	}
}

