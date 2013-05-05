using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cschip8
{
	public class Controller
	{
		public bool[] Keys = new bool[16];

		public void Update()
		{
			//if (Console.KeyAvailable) {
			//	var Key = Console.ReadKey(true);
			//	switch (Key.KeyChar) {
			//		case 'Q': Key = 0;
			//	}
			//}
		}

		public bool IsPressed(byte Key)
		{
			try {
				return Keys[Key];
			} finally {
				Keys[Key] = false;
			}
		}

		public int AnyPressed()
		{
			for (int n = 0; n < 16; n++) {
				if (Keys[n]) {
					Keys[n] = false;
					return n;
				}
			}
			return -1;
		}

		public void SetPressed(int Key, bool Pressed)
		{
			if (Key < 0 || Key >= 16) return;
			Keys[Key] = Pressed;
		}
	}
}
