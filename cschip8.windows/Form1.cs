using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cschip8.windows
{
	public partial class Form1 : Form
	{
		EmulatorState EmulatorState;
		Interpreter Interpreter;

		public Form1()
		{
			InitializeComponent();

			EmulatorState = new EmulatorState();
			Interpreter = new Interpreter(EmulatorState);

			var Timer = new System.Windows.Forms.Timer();
			Timer.Interval = 20;
			Timer.Tick += Timer_Tick;
			Timer.Start();

			EmulatorState.Memory.WriteBytes(0x200, File.ReadAllBytes("../../../games/HIDDEN"));
			//EmulatorState.Memory.WriteBytes(0x200, File.ReadAllBytes("../../../games/TETRIS"));
			//EmulatorState.Memory.WriteBytes(0x200, File.ReadAllBytes("../../../games/BRIX"));
			EmulatorState.Registers.PC = 0x200;

		}

		void Timer_Tick(object sender, EventArgs e)
		{
			Interpreter.ExecuteFrame();

			//EmulatorState.Display.ConsoleOutput();
			var Bitmap = new Bitmap(Display.Width, Display.Height);
			var Set = Color.White;
			var Unset = Color.Black;
			for (int y = 0; y < Display.Height; y++) {
				for (int x = 0; x < Display.Width; x++)
				{
					Bitmap.SetPixel(x, y, EmulatorState.Display.Get(x, y) ? Set : Unset);
				}
			}
			var Graphics = panel1.CreateGraphics();
			Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
			Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
			Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			Graphics.DrawImage(Bitmap, new Rectangle(0, 0, panel1.Width, panel1.Height));
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void Form1_Load(object sender, EventArgs e)
		{
			Focus();
		}

		public void Render()
		{
		}

		public int ConvertKeyCodeToKeyIndex(Keys Key)
		{
			switch (Key)
			{
				case Keys.D1: return 0x1;
				case Keys.D2: return 0x2;
				case Keys.D3: return 0x3;
				case Keys.D4: return 0xC;

				case Keys.Q: return 0x4;
				case Keys.W: return 0x5;
				case Keys.E: return 0x6;
				case Keys.R: return 0xD;

				case Keys.A: return 0x7;
				case Keys.S: return 0x8;
				case Keys.D: return 0x9;
				case Keys.F: return 0xE;

				case Keys.Z: return 0xA;
				case Keys.X: return 0x0;
				case Keys.C: return 0xB;
				case Keys.V: return 0xF;
			}

			return -1;
		}

		private void Form1_KeyDown(object sender, KeyEventArgs e)
		{
			EmulatorState.Controller.SetPressed(ConvertKeyCodeToKeyIndex(e.KeyCode), true);
		}

		private void Form1_KeyUp(object sender, KeyEventArgs e)
		{
			EmulatorState.Controller.SetPressed(ConvertKeyCodeToKeyIndex(e.KeyCode), false);
		}
	}
}
