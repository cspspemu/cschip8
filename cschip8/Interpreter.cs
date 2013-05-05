using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cschip8
{
	public class Interpreter
	{
		EmulatorState EmulatorState;
		Memory Memory;
		Registers Registers;
		Display Display;
		Controller Controller;

		public Interpreter(EmulatorState EmulatorState)
		{
			this.EmulatorState = EmulatorState;
			this.Memory = EmulatorState.Memory;
			this.Registers = EmulatorState.Registers;
			this.Display = EmulatorState.Display;
			this.Controller = EmulatorState.Controller;

			this.Memory.WriteBytes(0, Digits);
		}

		static private byte[] Digits = new byte[] {
			0xF0, 0x90, 0x90, 0x90, 0xF0,
			0x20, 0x60, 0x20, 0x20, 0x70,
			0xF0, 0x10, 0xF0, 0x80, 0xF0,
			0xF0, 0x10, 0xF0, 0x10, 0xF0,
			0x90, 0x90, 0xF0, 0x10, 0x10,
			0xF0, 0x80, 0xF0, 0x10, 0xF0,
			0xF0, 0x80, 0xF0, 0x90, 0xF0,
			0xF0, 0x10, 0x20, 0x40, 0x40,
			0xF0, 0x90, 0xF0, 0x90, 0xF0,
			0xF0, 0x90, 0xF0, 0x10, 0xF0,
			0xF0, 0x90, 0xF0, 0x90, 0x90,
			0xE0, 0x90, 0xE0, 0x90, 0xE0,
			0xF0, 0x80, 0x80, 0x80, 0xF0,
			0xE0, 0x90, 0x90, 0x90, 0xE0,
			0xF0, 0x80, 0xF0, 0x80, 0xF0,
			0xF0, 0x80, 0xF0, 0x80, 0x80,
		};

		public void ExecuteFrame()
		{
			EmulatorState.Update();
			for (int n = 0; n < 1000; n++) {
				ExecuteStep();
			}
		}

		public void ExecuteStep()
		{
			var Instruction = (Instruction)Memory.Read16(Registers.PC);
			//Console.WriteLine("{0:X4}: {1:X4} IT({2:X1}) Timer({3})", Registers.PC, Instruction.Value, Instruction.InstructionType, EmulatorState.DelayTimer.Value);

			Registers.PC += 2;

			switch (Instruction.InstructionType)
			{
				// Calls RCA 1802 program at address NNN.
				case 0x0:
					switch (Instruction.NNN)
					{
						// Clears the screen.
						case 0x0E0:
							Display.Clear();
						break;
						// Returns from a subroutine.
						case 0x0EE:
							Registers.PC = EmulatorState.CallStack.Pop();
						break;
						default:
							throw(new Exception(String.Format("Invalid operation 0x{0:X3}",Instruction.NNN)));
					}
				break;
				// Jumps to address NNN.
				case 0x1:
					Registers.PC = Instruction.NNN;
				break;
				// Calls subroutine at NNN.
				case 0x2:
					EmulatorState.CallStack.Push(Registers.PC);
					Registers.PC = Instruction.NNN;
				break;
				// Skips the next instruction if VX equals NN.
				case 0x3:
					if (Registers.V[Instruction.X] == Instruction.NN)
					{
						Registers.PC += 2;
					}
				break;
				// Skips the next instruction if VX doesn't equal NN.
				case 0x4:
					if (Registers.V[Instruction.X] != Instruction.NN)
					{
						Registers.PC += 2;
					}
				break;
				// Skips the next instruction if VX equals VY.
				case 0x5:
					if (Registers.V[Instruction.X] == Registers.V[Instruction.Y])
					{
						Registers.PC += 2;
					}
				break;
				// Sets VX to NN.
				case 0x6:
					Registers.V[Instruction.X] = Instruction.NN;
				break;
				// Adds NN to VX.
				case 0x7:
					Registers.V[Instruction.X] += Instruction.NN;
				break;
				// VX = VX op VY
				case 0x8:
					switch (Instruction.N) {
						// 8xy0 - LD Vx, Vy
						case 0x0:
							Registers.V[Instruction.X] = Registers.V[Instruction.Y];
						break;
						// 8xy1 - OR Vx, Vy
						case 0x1:
							Registers.V[Instruction.X] |= Registers.V[Instruction.Y];
						break;
						// 8xy2 - AND Vx, Vy
						case 0x2:
							Registers.V[Instruction.X] &= Registers.V[Instruction.Y];
						break;
						// 8xy3 - XOR Vx, Vy
						case 0x3:
							Registers.V[Instruction.X] ^= Registers.V[Instruction.Y];
						break;
						// 8xy4 - ADD Vx, Vy
						case 0x4:
							Registers.V[15] = (byte)(((Registers.V[Instruction.X] + Registers.V[Instruction.Y]) > 255) ? 1 : 0);
							Registers.V[Instruction.X] += Registers.V[Instruction.Y];
						break;
						// 8xy5 - SUB Vx, Vy
						case 0x5:
							Registers.V[15] = (byte)(((Registers.V[Instruction.X] >  Registers.V[Instruction.Y])) ? 1 : 0);
							Registers.V[Instruction.X] -= Registers.V[Instruction.Y];
						break;
						// 8xy6 - SHR Vx {, Vy}
						case 0x6:
						// 8xy7 - SUBN Vx, Vy
						case 0x7:
						// 8xyE - SHL Vx {, Vy}
						case 0xE:
							throw(new NotImplementedException("Not implemented"));
						break;
						default:
							throw(new Exception("Invalid operation"));
					}
				break;
				// Skips the next instruction if VX doesn't equal VY.
				case 0x9:
					if (Registers.V[Instruction.X] != Registers.V[Instruction.Y])
					{
						Registers.PC += 2;
					}
				break;
				// Sets I to the address NNN.
				case 0xA:
					Registers.I = Instruction.NNN;
				break;
				// Jumps to the address NNN plus V0.
				case 0xB:
					Registers.PC = (ushort)(Instruction.NNN + Registers.V[0]);
				break;
				// Sets VX to a random number and NN.
				case 0xC:
					Registers.V[Instruction.X] = (byte)(EmulatorState.Random.Next() & Instruction.NN);
				break;
				// Draws a sprite at coordinate (VX, VY) that has a width of 8 pixels and a height of N pixels. Each row of 8 pixels is read as bit-coded (with the most significant bit of each byte displayed on the left) starting from memory location I; I value doesn't change after the execution of this instruction. As described above, VF is set to 1 if any screen pixels are flipped from set to unset when the sprite is drawn, and to 0 if that doesn't happen.
				case 0xD:
					//Console.WriteLine("Draw ({0}, {1})!", Registers.V[Instruction.X], Registers.V[Instruction.Y]);
					Display.Draw(Registers, Memory, Registers.I, Registers.V[Instruction.X], Registers.V[Instruction.Y], Instruction.N);
					//Display.ConsoleOutput();
				break;
				case 0xE:
					switch (Instruction.NN) {
						// Skips the next instruction if the key stored in VX is pressed.
						case 0x9E:
							if (Controller.IsPressed(Registers.V[Instruction.X])) Registers.PC += 2;
							//throw (new NotImplementedException("Not implemented 0xE"));
						break;
						// Skips the next instruction if the key stored in VX isn't pressed.
						case 0xA1:
							if (!Controller.IsPressed(Registers.V[Instruction.X])) Registers.PC += 2;
							//throw (new NotImplementedException("Not implemented 0xE"));
						break;
						default:
							throw(new NotImplementedException("Not implemented 0xE"));
					}
				break;
				case 0xF:
					switch (Instruction.NN) {
						// LD Vx, DT
						// Set Vx = delay timer value
						// The value of DT is placed into Vx.
						// Sets VX to the value of the delay timer.
						case 0x07:
							Registers.V[Instruction.X] = EmulatorState.DelayTimer.Value;
						break;
						// Wait for a key press, store the value of the key in Vx.
						// All execution stops until a key is pressed, then the value of that key is stored in Vx
						// A key press is awaited, and then stored in VX.
						case 0x0A:
							{
								int KeyPressed = Controller.AnyPressed();
								if (KeyPressed != -1) {
									Registers.V[Instruction.X] = (byte)KeyPressed;
								} else {
									Registers.PC -= 2;
								}
							}
						break;
						// Sets the delay timer to VX.
						case 0x15:
							EmulatorState.DelayTimer.Value = Registers.V[Instruction.X];
						break;
						// Sets the sound timer to VX.
						case 0x18:
							EmulatorState.SoundTimer.Value = Registers.V[Instruction.X];
						break;
						// Adds VX to I.
						// VF is set to 1 when range overflow (I+VX>0xFFF), and 0 when there isn't. This is undocumented feature of the Chip-8 and used by Spacefight 2019! game.
						case 0x1E:
							// TODO: Note
							Registers.I += Registers.V[Instruction.X];
						break;
						// Sets I to the location of the sprite for the character in VX. Characters 0-F (in hexadecimal) are represented by a 4x5 font.
						case 0x29:
							Registers.I = (ushort)(Registers.V[Instruction.X] * 5);
						break;
						// Stores the Binary-coded decimal representation of VX, with the most significant of three digits at the address in I, the middle digit at I plus 1, and the least significant digit at I plus 2. (In other words, take the decimal representation of VX, place the hundreds digit in memory at location in I, the tens digit at location I+1, and the ones digit at location I+2.)
						case 0x33:
							//Memory.WriteBytes(Registers.I, Digits[Registers.V[Instruction.X]]);
							//throw (new NotImplementedException("Not implemented"));
							Memory.Write8((ushort)(Registers.I + 0), (byte)((Registers.V[Instruction.X] / 100) % 10));
							Memory.Write8((ushort)(Registers.I + 1), (byte)((Registers.V[Instruction.X] / 10) % 10));
							Memory.Write8((ushort)(Registers.I + 2), (byte)((Registers.V[Instruction.X] / 1) % 10));
						break;
						// Stores V0 to VX in memory starting at address I.
						// On the original interpreter, when the operation is done, I=I+X+1.
						case 0x55:
							for (int n = 0; n < Instruction.X; n++) {
								Memory.Write8(Registers.I++, Registers.V[n]);
							}
						break;
						// Fills V0 to VX with values from memory starting at address I.
						// On the original interpreter, when the operation is done, I=I+X+1.
						case 0x65:
							for (int n = 0; n < Instruction.X; n++) {
								Registers.V[n] = Memory.Read8(Registers.I++);
							}
						break;
						default:
						throw (new Exception(String.Format("Invalid 0xF:0x{0:X2}", Instruction.NN)));
							break;
					}
				break;
			}
		}
	}
}
