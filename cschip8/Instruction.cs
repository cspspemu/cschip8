using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cschip8
{
	public struct Instruction
	{
		public ushort Value;

		public byte InstructionType { get { return (byte)((Value >> 12) & 0xF); } }
		public byte X { get { return (byte)((Value >> 8) & 0xF); } }
		public byte Y { get { return (byte)((Value >> 4) & 0xF); } }
		public byte N { get { return (byte)((Value >> 0) & 0xF); } }
		public byte NN { get { return (byte)((Value >> 0) & 0xFF); } }
		public ushort NNN { get { return (ushort)((Value >> 0) & 0xFFF); } }

		static public implicit operator Instruction(ushort Value)
		{
			return new Instruction() { Value = Value };
		}
	}
}
