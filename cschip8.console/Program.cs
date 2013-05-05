using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cschip8.console
{
	class Program
	{
		static void Main(string[] args)
		{
			var EmulatorState = new EmulatorState();
			var Interpreter = new Interpreter(EmulatorState);
			//EmulatorState.Memory.WriteBytes(0x200, File.ReadAllBytes("../../../games/HIDDEN"));
			EmulatorState.Memory.WriteBytes(0x200, File.ReadAllBytes("../../../games/TETRIS"));
			//EmulatorState.Memory.WriteBytes(0x200, File.ReadAllBytes("../../../games/BRIX"));
			EmulatorState.Registers.PC = 0x200;
			int m = 0;
			while (true)
			{
				Interpreter.ExecuteFrame();
				if ((m++ % 5) == 0) EmulatorState.Display.ConsoleOutput();
				Thread.Sleep(20);
			}
		}
	}
}
