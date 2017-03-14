using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Audio;

//Darris James Martin ID 000203627
namespace BattleOfColthv2
{
	public class AppMain
	{
		//declarations
		private static GraphicsContext graphics;
		private static BackGround background, backgroundPaused;
		private static PlayerShip playership;
		private static PowerStation powerstation;
		private static HUD hud;
		private static TopScoreHUD topScoreHUD;
		private static List<Enemy> enemyList;
		private static List<Weapon> weaponPlayerList, weaponScaterList;
		private static List<Projectile> projectileList, projectileListE;
		private static Score[] scoreList;
		private static Stopwatch clock;
		private static long startTime, stopTime, timeDelta, badguyTimer;
		private static Texture2D texture;
		private static string gunTexFile, missleTexFile, DropOffTexFile;
		private static Random r;
		private static bool DoRunGame;
		private enum GameState{Menu, Playing, Paused, GameOver, Instructions, Credits, HighScore};
		private static GameState currentState;
		private static BgmPlayer bgmp;
		private static SoundPlayer bulletSound, missleSound, dropBombSound;
		
		
		public static void Main (string[] args)
		{
			Initialize ();

			while (DoRunGame) 
			{
				//I plan on useing a timer later. It is here for decorations right now.
				startTime = clock.ElapsedMilliseconds;
				SystemEvents.CheckEvents ();
				Update ();
				Render ();
				stopTime = clock.ElapsedMilliseconds;
				timeDelta = stopTime - startTime;
			}
		}

		public static void Initialize ()
		{
			// Set up the graphics system
			graphics = new GraphicsContext ();
			
			//Load Stuff
			clock = new Stopwatch();
			clock.Start();
			DoRunGame = true;
			scoreList = new Score[5];
			ReadScores();
			
			//Opening Menu
			InitializeMenu();
		}

		public static void Update ()
		{
			// Query gamepad for current state
			var gamePadData = GamePad.GetData (0);
			
			//Pick State
			switch(currentState)
			{
				case GameState.Menu : UpdateMenu(gamePadData); break;
				case GameState.Credits : UpdateCredits(gamePadData); break;
				case GameState.Paused : UpdatePaused(gamePadData); break;
				case GameState.Playing : UpdatePlay(gamePadData); break;
				case GameState.GameOver : UpdateGameOver(gamePadData); break;
				case GameState.HighScore : UpdateHighScore(gamePadData); break;
				case GameState.Instructions : UpdateInstructions(gamePadData); break;
			}
		}

		public static void Render ()
		{
			// Clear the screen
			graphics.SetClearColor (0.0f, 0.0f, 0.0f, 0.0f);
			graphics.Clear ();
			
			//Pick State
			switch(currentState)
			{
				case GameState.Menu : RenderMenu(); break;
				case GameState.Credits : RenderCredits(); break;
				case GameState.Paused : RenderPaused(); break;
				case GameState.Playing : RenderPlay(); break;
				case GameState.GameOver : RenderGameOver(); break;
				case GameState.HighScore : RenderHighScore(); break;
				case GameState.Instructions : RenderInstructions(); break;
			}

			// Present the screen
			graphics.SwapBuffers ();
		}
		
//*****************************************************************************
//*****************************************************************************
//                            Handling
//*****************************************************************************
//*****************************************************************************
		
		private static void ShipCollision ()
		{
			float d = 20.0f;
			
			for (int i= enemyList.Count-1; i>=0; i--) 
			{
				//this is for debugging
//				float test = Vector3.DistanceSquared (playership.Position, enemyList[i].Pos);
		
				if (Vector3.DistanceSquared (playership.Position, enemyList[i].Pos) < d*d) 
				{
					enemyList[i].TakeDamage(10);
					playership.TakeDamage(10);
					break;
				}
			}	
		}
		
		private static void BulletDamage ()
		{
			float d = 50.0f;
			
			for (int j = projectileList.Count-1; j>=0; j--)
			{
				for (int i= enemyList.Count-1; i>=0; i--) 
				{
					if (Vector3.DistanceSquared (projectileList[j].Pos, enemyList[i].Pos) < d*d) 
					{
						enemyList[i].TakeDamage(projectileList[j].Damage);;
						projectileList.RemoveAt(j);
						break;
					}
				}	
			}
			
			for (int j = projectileListE.Count-1; j>=0; j--)
			{
				if (Vector3.DistanceSquared (projectileListE[j].Pos, playership.Position) < d*d) 
				{
					playership.TakeDamage(projectileListE[j].Damage);;
					projectileListE.RemoveAt(j);
					break;
				}
				if (Vector3.DistanceSquared (projectileListE[j].Pos, powerstation.Position) < d*d) 
					{
						powerstation.TakeDamage(projectileListE[j].Damage);;
						projectileListE.RemoveAt(j);
						break;
					}
			}
		}
		
		private static void CleanupStuff()
		{
			//cleanup dead badguys
			for (int i = enemyList.Count-1; i>=0; i--)
			{
				bool isAlive = enemyList[i].IsAlive;
				if (isAlive != true)
				{
					playership.Points = enemyList[i].ShipPoints;
					enemyList.RemoveAt(i);
				}
			}
			
			//Clean up projectiles
			for (int i = projectileList.Count-1; i>=0; i--)
			{
				bool isAlive = projectileList[i].IsAlive;
				if(isAlive != true)
					projectileList.RemoveAt(i);
			}
			for (int i = projectileListE.Count-1; i>=0; i--)
			{
				bool isAlive = projectileListE[i].IsAlive;
				if(isAlive != true)
					projectileListE.RemoveAt(i);
			}
		}
				
		private static void HandlePickupWeapon(GamePadData gp)
		{
			var gamePadData = gp;
			var pressedButtons = gamePadData.ButtonsDown;
			
			//pickup weapon
			if ((pressedButtons & GamePadButtons.Cross) == GamePadButtons.Cross) 
			{
				for (int i = weaponScaterList.Count-1; i >= 0; i--)
				{
					float dist = Vector3.DistanceSquared (playership.Position, weaponScaterList[i].Pos);
					if(dist < 400)
					{
						if (weaponScaterList[i].UniqueID == 1)
						{
							texture = new Texture2D(gunTexFile, false);
							weaponPlayerList.Add(new Guns(graphics, texture, playership, 1));
							weaponScaterList.RemoveAt(i);
							break;
						}
						if (weaponScaterList[i].UniqueID == 2)
						{
							texture = new Texture2D(missleTexFile, false);
							weaponPlayerList.Add(new Missle(graphics, texture, playership, 2));
							weaponScaterList.RemoveAt(i);
							break;
						}
						if (weaponScaterList[i].UniqueID == 3)
						{
							texture = new Texture2D(DropOffTexFile, false);
							weaponPlayerList.Add(new DropOff(graphics, texture, playership, 3));
							weaponScaterList.RemoveAt(i);
							break;
						}
					}
				}
			}
		}
		
		//When is the game over?
		private static void GameOver ()
		{
			bool doGameOver = false;
			if (enemyList.Count ==0)
				doGameOver = true;
			if (playership.IsAlive == false || powerstation.IsAlive == false)
				doGameOver = true;
			
			// Now we end the game.
			if (doGameOver)
			{
				InitializeGameOver();
			}
		}
		
		//High Score List
		private static void WriteScores()
		{
			StreamWriter sw= new StreamWriter("/Documents/HighScores.txt");
			
			for (int i = 0; i< scoreList.GetLength(0); i++)
			{
				sw.WriteLine(scoreList[i].PlayerInitials+"|"+scoreList[i].PlayerScore);
			}
			sw.Close();
		}
		
		//Read High Scores
		private static void ReadScores()
		{
			string row;
			char[] delims = { '|' };
			string[] tokens;
			int index = 0;
			try
			{
				StreamReader sr = new StreamReader("/Documents/HighScores.txt");
				
				while (!sr.EndOfStream)
				{
					row = sr.ReadLine();
					tokens = row.Split(delims);
					scoreList[index] = new Score(Int32.Parse(tokens[1]), tokens[0]);
					
					index++;
				}
				sr.Close();
			}
			catch
			{
				scoreList[0] = new Score(5, "AAA");
				scoreList[1] = new Score(4, "BBB");
				scoreList[2] = new Score(3, "CCC");
				scoreList[3] = new Score(2, "DDD");
				scoreList[4] = new Score(1, "EEE");
			}
		}
		
		//Background Music
		private static void MusicPlay(string music)
		{
			try
			{
				bgmp.Dispose();
			}
			catch{}
			
			Bgm bgm = new Bgm(music);
			bgmp = bgm.CreatePlayer();
			bgmp.Loop = true;
			bgmp.Play();
		}
		
		//Red Bad Guy creation. 
		private static void CreateSmallBaddys()
		{
			texture = new Texture2D("/Application/Assets/FighterShip.png", false);
			for (int i = 0; i<3; i++)
				enemyList.Add(new FighterShip(graphics, texture, playership, enemyList, 10, 3, r.Next(1000,1500),r.Next(-500,0)));
		}
		
		//On Close
		private static void EndGame()
		{
			WriteScores();
			DoRunGame = false;
		}
		
//*****************************************************************************
//*****************************************************************************
//                            New Game Initialize 
//*****************************************************************************
//*****************************************************************************
		private static void InitializeNewGame()
		{
			//Load things
			Texture2D bgTex = new Texture2D("/Application/Assets/Background.png",false);
			background = new BackGround(graphics, bgTex);
			playership = new PlayerShip(graphics);
			powerstation = new PowerStation(graphics);
			enemyList = new List<Enemy> ();
			weaponPlayerList = new List<Weapon>();
			weaponScaterList = new List<Weapon>();
			projectileList = new List<Projectile>();
			projectileListE = new List<Projectile>();
			hud = new HUD(graphics);
			r = new Random();
			badguyTimer = 0;
			
			//LoadingSounds
			MusicPlay("/Application/Assets/MainPlay.mp3");
						
			//WeaponSounds
			Sound soundEffect;
			soundEffect= new Sound("/Application/Assets/Bullet.wav");
			bulletSound = soundEffect.CreatePlayer();
			soundEffect= new Sound("/Application/Assets/Missle.wav");
			missleSound = soundEffect.CreatePlayer();
			soundEffect= new Sound("/Application/Assets/DropBomb.wav");
			dropBombSound = soundEffect.CreatePlayer();
			
			//enemy creation
			texture = new Texture2D("/Application/Assets/MotherShip.png", false);
			enemyList.Add(new MotherShip(graphics, texture, powerstation, enemyList, 1000, 1, -200, 50));
			CreateSmallBaddys();
			texture = new Texture2D("/Application/Assets/Guardian.png", false);
			enemyList.Add(new Guardian(graphics, texture, playership, enemyList, 250, 2,0,r.Next(600,1000)));
			
			//Weapon Creation
			gunTexFile = "/Application/Assets/gun.png";
			texture = new Texture2D(gunTexFile, false); 
			weaponScaterList.Add(new Guns(graphics, texture, new Vector3(200, 300, 0), 0f, 1));
			missleTexFile = "/Application/Assets/MissleGun.png";
			texture = new Texture2D(missleTexFile, false); 
			weaponScaterList.Add(new Guns(graphics, texture, new Vector3(400, 200, 0), 0f, 2));
			DropOffTexFile = "/Application/Assets/DropBox.png";
			texture = new Texture2D(DropOffTexFile, false); 
			weaponScaterList.Add(new Guns(graphics, texture, new Vector3(600, 100, 0), 0f, 3));
			
			currentState = GameState.Playing;
		}
		
//*****************************************************************************
//*****************************************************************************
//                            Playing State Update
//*****************************************************************************
//*****************************************************************************
		private static void UpdatePlay(GamePadData gamePadData)
		{
			var pressedButtons = gamePadData.ButtonsDown;
			
			//player movement
			playership.Update(gamePadData);
			powerstation.Update();
			
			//enemy updates (This may be expanded to go to update in enemy and then it will choose move and other stuff later)
			foreach (Enemy e in enemyList)
			{
				e.Update(timeDelta);
				
				bool doattack = e.IsAttacking;
				if (doattack)
				{
					Texture2D tex = new Texture2D(e.ProjTexture,false);
					projectileListE.Add(new Projectile(graphics, tex, e, 10, 10));
				}
			}
			
			//Weapon Update
			foreach (Weapon w in weaponPlayerList)
				w.Update();
			
			//Handles (Collision, Damage, Cleanup, and Pickup)
			ShipCollision();
			BulletDamage();
			CleanupStuff();
			HandlePickupWeapon(gamePadData);
			
			//switch weapons. I think I can do better than this. It will work for now, but keep me in mind for cleanup time.
			if ((pressedButtons & GamePadButtons.R) == GamePadButtons.R) 
			{
				if(playership.WeaponIndex +1 > weaponPlayerList.Count-1)
				{
					playership.WeaponIndex = 0;
				}
				else
				{
					playership.WeaponIndex++;
				}
			}
			
			//reload weapon
			if ((pressedButtons & GamePadButtons.Square) == GamePadButtons.Square) 
			{
				if (weaponPlayerList.Count>0)
				{
					weaponPlayerList[playership.WeaponIndex].Reload();
				}
			}
			
			//shoot weapon
			if ((pressedButtons & GamePadButtons.Circle) == GamePadButtons.Circle) 
			{
				if (weaponPlayerList.Count>0)
				{				
					bool doShoot = weaponPlayerList[playership.WeaponIndex].Shoot();
					if(doShoot)
					{
						if(weaponPlayerList[playership.WeaponIndex].UniqueID == 1)
						{
							texture = new Texture2D("/Application/Assets/bullet.png", false);
							projectileList.Add(new Projectile(graphics,texture,playership,10,weaponPlayerList[playership.WeaponIndex].Damage));
							bulletSound.Play();
						}
						if(weaponPlayerList[playership.WeaponIndex].UniqueID == 2)
						{
							texture = new Texture2D("/Application/Assets/BoomStick.png", false);
							projectileList.Add(new Projectile(graphics,texture,playership,5,weaponPlayerList[playership.WeaponIndex].Damage));
							missleSound.Play();
						}
						if(weaponPlayerList[playership.WeaponIndex].UniqueID == 3)
						{
							texture = new Texture2D("/Application/Assets/DropOff.png", false);
							projectileList.Add(new Projectile(graphics,texture,playership,0,weaponPlayerList[playership.WeaponIndex].Damage));
							dropBombSound.Play();
						}
					}
				}
			}
			
			//Update projectile
			foreach (Projectile p in projectileList)
				p.Update();
			foreach (Projectile p in projectileListE)
				p.Update();
			
			//Update HUD
		 	hud.Update("Score: "+playership.Points);
			
			//Check for Paused
			if ((pressedButtons & GamePadButtons.Start) == GamePadButtons.Start) 
			{
				InitializePaused();
			}
			
			//Create more baddies
			badguyTimer += timeDelta;
			if(badguyTimer >= 10000)
			{
				CreateSmallBaddys();
				badguyTimer =0;
			}
			
			//Check for GameOver
			GameOver();
		}
		
//*****************************************************************************
//*****************************************************************************
//                            Playing State Render
//*****************************************************************************
//*****************************************************************************
		public static void RenderPlay ()
		{
			//Render things
			background.Render();
			powerstation.Render();
			
			foreach (Weapon w in weaponScaterList)
				w.Render();
			foreach (Enemy e in enemyList)
				e.Render();
			foreach (Projectile p in projectileList)
				p.Render();
			foreach (Projectile p in projectileListE)
				p.Render();
			
			playership.Render();
			hud.Render();
			
			if(weaponPlayerList.Count>0)
				weaponPlayerList[playership.WeaponIndex].Render();
		}
		
//*****************************************************************************
//*****************************************************************************
//                            Menu
//*****************************************************************************
//*****************************************************************************	
		public static void InitializeMenu()
		{
			//Load things
			Texture2D bgTex = new Texture2D("/Application/Assets/Menu.png",false);
			background = new BackGround(graphics, bgTex);
			MusicPlay("/Application/Assets/Menu.mp3");
			
			//set State
			currentState = GameState.Menu;
		}

		public static void UpdateMenu (GamePadData gamePadData)
		{
			var pressedButtons = gamePadData.ButtonsDown;
			
			if ((pressedButtons & GamePadButtons.Square) == GamePadButtons.Square) 
			{
				InitializeInstructions();
			}
			if ((pressedButtons & GamePadButtons.Triangle) == GamePadButtons.Triangle) 
			{
				InitializeCredits();
			}
			if ((pressedButtons & GamePadButtons.Circle) == GamePadButtons.Circle) 
			{
				InitializeHighScore();
			}
			if ((pressedButtons & GamePadButtons.Select) == GamePadButtons.Select) 
			{
				EndGame();
			}
			if ((pressedButtons & GamePadButtons.Cross) == GamePadButtons.Cross) 
			{
				InitializeNewGame();
			}
		}

		public static void RenderMenu ()
		{
			background.Render();
		}
		
//*****************************************************************************
//*****************************************************************************
//                            Instructions
//*****************************************************************************
//*****************************************************************************	
		public static void InitializeInstructions()
		{
			//Load things
			Texture2D bgTex = new Texture2D("/Application/Assets/Instructions.png",false);
			background = new BackGround(graphics, bgTex);
			
			//set state
			currentState = GameState.Instructions;
		}
		
		public static void UpdateInstructions(GamePadData gamePadData)
		{
			var pressedButtons = gamePadData.ButtonsDown;
			if ((pressedButtons & GamePadButtons.Start) == GamePadButtons.Start) 
			{
				InitializeMenu();
			}
		}

		public static void RenderInstructions ()
		{
			background.Render();
		}
		
//*****************************************************************************
//*****************************************************************************
//                            Credits
//*****************************************************************************
//*****************************************************************************	
		public static void InitializeCredits()
		{
			//Load things
			Texture2D bgTex = new Texture2D("/Application/Assets/Credits.png",false);
			background = new BackGround(graphics, bgTex);
			
			//set state
			currentState = GameState.Credits;
		}
		
		public static void UpdateCredits(GamePadData gamePadData)
		{
			var pressedButtons = gamePadData.ButtonsDown;
			if ((pressedButtons & GamePadButtons.Start) == GamePadButtons.Start) 
			{
				InitializeMenu();
			}
		}

		public static void RenderCredits()
		{
			background.Render();
		}
		
//*****************************************************************************
//*****************************************************************************
//                              Paused
//*****************************************************************************
//*****************************************************************************	
		public static void InitializePaused()
		{
			//Load things
			Texture2D bgTex = new Texture2D("/Application/Assets/Paused.png",false);
			backgroundPaused = new BackGround(graphics, bgTex);
			
			//set state
			currentState = GameState.Paused;
		}
		
		public static void UpdatePaused(GamePadData gamePadData)
		{
			var pressedButtons = gamePadData.ButtonsDown;
			if ((pressedButtons & GamePadButtons.Start) == GamePadButtons.Start) 
			{
				currentState = GameState.Playing;
			}
			if ((pressedButtons & GamePadButtons.Select) == GamePadButtons.Select) 
			{
				InitializeMenu();
			}
		}

		public static void RenderPaused()
		{
			backgroundPaused.Render();
		}		
		
//*****************************************************************************
//*****************************************************************************
//                              GameOver
//*****************************************************************************
//*****************************************************************************	
		public static void InitializeGameOver()
		{
			//Load things
			Texture2D bgTex = new Texture2D("/Application/Assets/GameOver.png",false);
			background = new BackGround(graphics, bgTex);
			topScoreHUD = new TopScoreHUD(graphics, scoreList, playership.Points);
			MusicPlay("/Application/Assets/GameOver.mp3");
			
			//set state
			currentState = GameState.GameOver;
		}
		
		public static void UpdateGameOver(GamePadData gamePadData)
		{
			var pressedButtons = gamePadData.ButtonsDown;
			
			topScoreHUD.Update(gamePadData);
			
			if ((pressedButtons & GamePadButtons.Start) == GamePadButtons.Start) 
			{
				scoreList[4].PlayerInitials = topScoreHUD.PlayersInitials;
				scoreList[4].PlayerScore = topScoreHUD.PlayersScore;
				Array.Sort(scoreList);
				Array.Reverse(scoreList);
				InitializeMenu();
			}
		}

		public static void RenderGameOver()
		{
			background.Render();
			topScoreHUD.Render();
		}			
		
//*****************************************************************************
//*****************************************************************************
//                              HighScore
//*****************************************************************************
//*****************************************************************************	
		public static void InitializeHighScore()
		{
			//Load things
			Texture2D bgTex = new Texture2D("/Application/Assets/HighScore.png",false);
			background = new BackGround(graphics, bgTex);
			topScoreHUD = new TopScoreHUD(graphics, scoreList,0);
			MusicPlay("/Application/Assets/HighScore.mp3");
			
			//set state
			currentState = GameState.HighScore;
		}
		
		public static void UpdateHighScore(GamePadData gamePadData)
		{
			var pressedButtons = gamePadData.ButtonsDown;
			if ((pressedButtons & GamePadButtons.Start) == GamePadButtons.Start) 
			{
				InitializeMenu();
			}
			topScoreHUD.Update(gamePadData);
		}

		public static void RenderHighScore()
		{
			background.Render();
			topScoreHUD.Render();
		}	
	}
}
