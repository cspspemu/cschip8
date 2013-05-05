using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cschip8
{
	public class Memory
	{
		private byte[] Data = new byte[0x1000];

		public byte Read8(ushort Address) {
			return Data[Address];
		}

		public ushort Read16(ushort Address)
		{
			return (ushort)(((uint)Read8((ushort)(Address + 0)) << 8) | ((uint)Read8((ushort)(Address + 1)) << 0));
		}

		public void Write8(ushort Address, byte Value)
		{
			Data[Address] = Value;
		}

		public void WriteBytes(ushort Address, byte[] Values)
		{
			foreach (var Value in Values) Write8(Address++, Value);
		}
	}
}
