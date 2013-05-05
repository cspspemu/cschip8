using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cschip8
{
	public class EmulatorState
	{
		public Random Random = new Random();
		public Timer DelayTimer = new Timer("Delay");
		public Timer SoundTimer = new Timer("Sound");
		public Controller Controller = new Controller();
		public Memory Memory = new Memory();
		public Registers Registers = new Registers();
		public Display Display = new Display();
		public Stack<ushort> CallStack = new Stack<ushort>();

		public void Update()
		{
			DelayTimer.Update();
			SoundTimer.Update();
			Controller.Update();
		}
	}
}
