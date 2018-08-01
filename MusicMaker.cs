using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Diagnostics;
using System.IO;
using System;
using Terraria.Utilities;

namespace MusicMaker
{

	class MusicMaker : Mod
	{
		
		public MusicMaker()
		{
		}

		public static int[] noteStart = new int[10];
		public static int[] curTileX = new int[10];
		public static int[] curTileY = new int[10];
		public static bool[] isBass = new bool[10];
		public static int notesActive = 0;
		public static int curActive = 0;
		public static int tempo = 0;
		public static Stopwatch timer;
		public static bool musicPlaying = false;


		public override void PreSaveAndQuit(){
			musicPlaying = false;
			notesActive = 0;
			timer = null;
		}

		public override void HandlePacket (BinaryReader	reader,int whoAmI){
			byte msgType = reader.ReadByte();
			switch (msgType)
			{
				case 0:
					tempo = reader.ReadInt32();
					byte bass = reader.ReadByte();
					int i = reader.ReadInt32();
					int j = reader.ReadInt32();
					if(notesActive >= 10)
						return;
					for(int x=0;x<notesActive;x++){
						if(noteStart[x] == (i-1) && curTileY[x] == (j-1))
							return;
					}
					curTileX[notesActive] = i-1;
					noteStart[notesActive] = i-1;
					curTileY[notesActive] = j-1;
					isBass[notesActive] = (bass == 1);
					notesActive++;
					timer = new Stopwatch();
					timer.Start();
					break;
			}
		}

		//I decided to update the music notes whenever the UI updates are processed
		public override void UpdateUI (GameTime gameTime){
			if(notesActive == 0)
				return;
			//This variable works fine for single player,
			//but does not work with multiplayer due to packet delay
			musicPlaying = true;
			if(timer.ElapsedMilliseconds < 84)
				return;
			if(tempo == 120 && timer.ElapsedMilliseconds < 125)
				return;
			if(tempo == 80 && timer.ElapsedMilliseconds < 188)
				return;
			
			curActive = 0;
			bool noneActive = true;
			for(int x=0;x<notesActive;x++){
				curTileX[x] += 2;
				int tile = Main.tile[curTileX[x], curTileY[x]]==null?-1:Main.tile[curTileX[x], curTileY[x]].type;
				bool nextnote = true;
				string note = null;
				if(tile == TileType("Note_A"))
					note = "A";
				else if(tile == TileType("Note_AS"))
					note = "AS";
				else if(tile == TileType("Note_B"))
					note = "B";
				else if(tile == TileType("Note_C"))
					note = "C";
				else if(tile == TileType("Note_CS"))
					note = "CS";
				else if(tile == TileType("Note_D"))
					note = "D";
				else if(tile == TileType("Note_DS"))
					note = "DS";
				else if(tile == TileType("Note_E"))
					note = "E";
				else if(tile == TileType("Note_F"))
					note = "F";
				else if(tile == TileType("Note_FS"))
					note = "FS";
				else if(tile == TileType("Note_G"))
					note = "G";
				else if(tile == TileType("Note_GS"))
					note = "GS";
				else if(tile == TileType("Note_K"))
					note = "K";
				else if(tile == TileType("Note_S"))
					note = "S";
				else if(tile == TileType("Note_LT"))
					note = "LT";
				else if(tile == TileType("Note_MT"))
					note = "MT";
				else if(tile == TileType("Note_HT"))
					note = "HT";
				else if(tile == TileType("Note_OH"))
					note = "OH";
				else if(tile == TileType("Note_CH"))
					note = "CH";
				else if(tile != TileType("Note_P"))
					nextnote = false;

				//Main.NewText("Note: "+note+" - Timer: "+Math.Round((double)timer.ElapsedMilliseconds), 255, 255, 0);

				if(nextnote){
					if(tile != TileType("Note_P")){
						if(isBass[x])
							note = "B"+note;
						Main.PlaySound(GetLegacySoundSlot(SoundType.Custom, "Sounds/"+note)); 				
					}
					Vector2 vector = new Vector2(curTileX[x]*16,curTileY[x]*16);
					for(int y=0;y<10;y++)
						Dust.NewDust(vector,0,0,DustType("NoteHit"));
					noneActive = false;
				}
	
				curActive++;
				if(curActive >= notesActive)
					curActive = 0;
			}
			if(noneActive){
				musicPlaying = false;
				notesActive = 0;
				timer.Stop();
			}
			else
				timer = Stopwatch.StartNew();
			
		}


		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.Bell, 1);
			recipe.AddIngredient(ItemID.MusicBox, 1);
			recipe.AddTile(TileID.CrystalBall);
			recipe.SetResult(ItemType("DrumBox"), 1);
			recipe.AddRecipe();

			recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.Harp, 1);
			recipe.AddIngredient(ItemID.MusicBox, 1);
			recipe.AddTile(TileID.CrystalBall);
			recipe.SetResult(ItemType("TrebleBox"), 1);
			recipe.AddRecipe();

			recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.PlatinumWatch, 1);
			recipe.AddTile(TileID.CrystalBall);
			recipe.SetResult(ItemType("Music_Play_180"), 1);
			recipe.AddRecipe();

			recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.GoldWatch, 1);
			recipe.AddTile(TileID.CrystalBall);
			recipe.SetResult(ItemType("Music_Play_180"), 1);
			recipe.AddRecipe();

			recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.TungstenWatch, 1);
			recipe.AddTile(TileID.CrystalBall);
			recipe.SetResult(ItemType("Music_Play_120"), 1);
			recipe.AddRecipe();

			recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.SilverWatch, 1);
			recipe.AddTile(TileID.CrystalBall);
			recipe.SetResult(ItemType("Music_Play_120"), 1);
			recipe.AddRecipe();

			recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.CopperWatch, 1);
			recipe.AddTile(TileID.CrystalBall);
			recipe.SetResult(ItemType("Music_Play_80"), 1);
			recipe.AddRecipe();

			recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.TinWatch, 1);
			recipe.AddTile(TileID.CrystalBall);
			recipe.SetResult(ItemType("Music_Play_80"), 1);
			recipe.AddRecipe();

			recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.PlatinumWatch, 1);
			recipe.AddTile(TileID.CrystalBall);
			recipe.SetResult(ItemType("Music_Play_B180"), 1);
			recipe.AddRecipe();

			recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.GoldWatch, 1);
			recipe.AddTile(TileID.CrystalBall);
			recipe.SetResult(ItemType("Music_Play_B180"), 1);
			recipe.AddRecipe();

			recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.TungstenWatch, 1);
			recipe.AddTile(TileID.CrystalBall);
			recipe.SetResult(ItemType("Music_Play_B120"), 1);
			recipe.AddRecipe();

			recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.SilverWatch, 1);
			recipe.AddTile(TileID.CrystalBall);
			recipe.SetResult(ItemType("Music_Play_B120"), 1);
			recipe.AddRecipe();

			recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.CopperWatch, 1);
			recipe.AddTile(TileID.CrystalBall);
			recipe.SetResult(ItemType("Music_Play_B80"), 1);
			recipe.AddRecipe();

			recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.TinWatch, 1);
			recipe.AddTile(TileID.CrystalBall);
			recipe.SetResult(ItemType("Music_Play_B80"), 1);
			recipe.AddRecipe();
		}

	}
}
