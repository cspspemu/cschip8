using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cschip8
{
	public class Display
	{
		public const int Width = 64;
		public const int Height = 32;
		public bool[,] Pixels = new bool[Width, Height];

		public bool InBounds(int x, int y)
		{
			return (x >= 0 && y >= 0 && x < Width && y < Height);
		}

		public void Set(int x, int y, bool Value)
		{
			//if (!InBounds(x, y)) return;
			Pixels[x % Width, y % Height] = Value;
		}

		public bool Get(int x, int y)
		{
			//if (!InBounds(x, y)) return false;
			return Pixels[x % Width, y % Height];
		}

		public void Draw(Registers Registers, Memory Memory, ushort Address, int px, int py, int height)
		{
			//Console.WriteLine("----------");
			//for (int y = 0; y < height; y++) {
			//	var Row = Memory.Read8((ushort)(Address + y));
			//	for (int x = 0; x < 8; x++) {
			//		var New = ((Row >> (x)) & 1) != 0;
			//		Console.Write(New ? "*" : ".");
			//	}
			//	Console.WriteLine();
			//}

			Registers.V[15] = 0;
			for (int y = 0; y < height; y++) {
				var Row = Memory.Read8((ushort)(Address + y));
				for (int x = 0; x < 8; x++) {
					var New = ((Row >> (7 - x)) & 1) != 0;
					var Old = Get(px + x, py + y);
					if (Old && New) Registers.V[15] = 1;
					Set(px + x, py + y, Old ^ New);
				}
			}
		}

		public void Clear()
		{
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					Set(x, y, false);
				}
			}
		}

		public void ConsoleOutput()
		{
			//return;

			Console.SetWindowSize(Width + 1, Height);
			Console.SetBufferSize(Width + 1, Height);
			for (int y = 0; y < Height; y++)
			{
				var Line = "";
				for (int x = 0; x < Width; x++)
				{
					Line += Get(x, y) ? "X" : ".";
				}
				Console.SetCursorPosition(0, y);
				Console.Write(Line);
				//Console.WriteLine();
			}
		}
	}
}
