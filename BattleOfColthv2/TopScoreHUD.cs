using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.HighLevel.UI;

namespace BattleOfColthv2
{
	public class TopScoreHUD
	{
		//declarations
		private GraphicsContext graphics;
		private Label lblTopScore;
		private Score[] scorelist;
		private string newHighScoreTxt, characterList, init1, init2, init3, playersInitials;
		private bool doNewHighScore;
		private int score, indexPlace, indexChar1, indexChar2, indexChar3, playersScore;
		
		public String PlayersInitials {get {return playersInitials;}}
		public int PlayersScore {get {return playersScore;}}
		
		
		
		public TopScoreHUD (GraphicsContext g, Score[] sl, int sc)
		{
			graphics = g;
			UISystem.Initialize(graphics);
			scorelist = sl;
			newHighScoreTxt ="";
			characterList = "_ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!";
			init1 = "";
			init2 = "";
			init3 = "";
			playersInitials = scorelist[4].PlayerInitials;
			playersScore = scorelist[4].PlayerScore;
			score = sc;
			indexPlace=0;
			indexChar1=0;
			indexChar2=0;
			indexChar3=0;
			doNewHighScore = false;
			
			//New High Score Check
			if (score > scorelist[4].PlayerScore)
			{
				doNewHighScore = true;
				init1= "_";
				init2= "_";
				init3= "_";
			}
			
			//Create Text
			Scene scene = new Scene();
			
			lblTopScore = new Label();
			lblTopScore.X = 0;
			lblTopScore.Y = 10;
			lblTopScore.Width = graphics.Screen.Rectangle.Width;
			lblTopScore.Height = graphics.Screen.Rectangle.Height;
			lblTopScore.Text = "";
					
			lblTopScore.HorizontalAlignment = HorizontalAlignment.Center;
			lblTopScore.VerticalAlignment = VerticalAlignment.Top;
			scene.RootWidget.AddChildLast(lblTopScore);
						
			UISystem.SetScene(scene, null);
		}
		
		public void Update(GamePadData gamePadData)
		{
			var pressedButtons = gamePadData.ButtonsDown;
			
			if (doNewHighScore)
			{
				newHighScoreTxt = 
					"Congratualtions! You have a new High Score: "+score+"\n" +
					"Please enter your initials";
				
				//Enter Name
				if ((pressedButtons & GamePadButtons.Right) == GamePadButtons.Right)
				{
					indexPlace = Math.Min(indexPlace+1,2);
				}
				if ((pressedButtons & GamePadButtons.Left) == GamePadButtons.Left)
				{
					indexPlace = Math.Max(indexPlace-1,0);
				}
				if ((pressedButtons & GamePadButtons.Up) == GamePadButtons.Up)
				{
					if(indexPlace == 0)
					{
						indexChar1 = Math.Min(indexChar1+1,characterList.Length-1);
						init1 = characterList[indexChar1].ToString();
					}
					if(indexPlace == 1)
					{
						indexChar2 = Math.Min(indexChar2+1,characterList.Length-1);
						init2 = characterList[indexChar2].ToString();
					}
					if(indexPlace == 2)
					{
						indexChar3 = Math.Min(indexChar3+1,characterList.Length);
						init3 = characterList[indexChar3].ToString();
					}
				}
				if ((pressedButtons & GamePadButtons.Down) == GamePadButtons.Down)
				{
					if(indexPlace == 0)
					{
						indexChar1 = Math.Max(indexChar1-1,0);
						init1 = characterList[indexChar1].ToString();
					}
					if(indexPlace == 1)
					{
						indexChar2 = Math.Max(indexChar2-1,0);
						init2 = characterList[indexChar2].ToString();
					}
					if(indexPlace == 2)
					{
						indexChar3 = Math.Max(indexChar3-1,0);
						init3 = characterList[indexChar3].ToString();
					}
				}
				playersInitials = init1+init2+init3;
				playersScore = score;
			}
			
			lblTopScore.Text = 
					"High Scores\n" +	
					scorelist[0].PlayerInitials+" "+scorelist[0].PlayerScore+"\n" +
					scorelist[1].PlayerInitials+" "+scorelist[1].PlayerScore+"\n" +
					scorelist[2].PlayerInitials+" "+scorelist[2].PlayerScore+"\n" +
					scorelist[3].PlayerInitials+" "+scorelist[3].PlayerScore+"\n" +
					scorelist[4].PlayerInitials+" "+scorelist[4].PlayerScore+"\n\n" +
					newHighScoreTxt+"\n"+
					init1+" "+init2+" "+init3;
		}
		
		public void Render()
		{
			UISystem.Render();
		}
	}
}

