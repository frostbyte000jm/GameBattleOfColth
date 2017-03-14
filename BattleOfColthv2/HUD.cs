using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.HighLevel.UI;

namespace BattleOfColthv2
{
	public class HUD
	{
		//declarations
		private GraphicsContext graphics;
		private Label lblPlayScore;
		
		public HUD (GraphicsContext g)
		{
			graphics = g;
			UISystem.Initialize(graphics);
						
			Scene scene = new Scene();
			
			lblPlayScore = new Label();
			lblPlayScore.X = 0;
			lblPlayScore.Y = 10;
			lblPlayScore.Width = 960;
			lblPlayScore.HorizontalAlignment = HorizontalAlignment.Center;
			scene.RootWidget.AddChildLast(lblPlayScore);
						
			UISystem.SetScene(scene, null);
		}
		
		public void Update(string s)
		{
			lblPlayScore.Text = s;
		}
		
		public void Render()
		{
			UISystem.Render();
		}
	}
}

