﻿/*
    Copyright (C) 2018 de4dot@gmail.com

    This file is part of Iced.

    Iced is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Iced is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with Iced.  If not, see <https://www.gnu.org/licenses/>.
*/

#if !NO_DECODER64 && !NO_DECODER
using System;
using System.Diagnostics;

namespace Iced.Intel.DecoderInternal.OpCodeHandlers64 {
	sealed class OpCodeHandler_VEX2 : OpCodeHandlerModRM {
		public override void Decode(Decoder decoder, ref Instruction instruction) => decoder.VEX2(ref instruction);
	}

	sealed class OpCodeHandler_VEX3 : OpCodeHandlerModRM {
		public override void Decode(Decoder decoder, ref Instruction instruction) => decoder.VEX3(ref instruction);
	}

	sealed class OpCodeHandler_XOP : OpCodeHandlerModRM {
		readonly OpCodeHandler handler_reg0;

		public OpCodeHandler_XOP(OpCodeHandler handler_reg0) => this.handler_reg0 = handler_reg0 ?? throw new ArgumentNullException(nameof(handler_reg0));

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			if ((decoder.state.modrm & 0x1F) < 8)
				handler_reg0.Decode(decoder, ref instruction);
			else
				decoder.XOP(ref instruction);
		}
	}

	sealed class OpCodeHandler_VEX_VHEv : OpCodeHandlerModRM {
		readonly Register baseReg;
		readonly Code codeW0;
		readonly Code codeW1;

		public OpCodeHandler_VEX_VHEv(Register baseReg, Code codeW0, Code codeW1) {
			this.baseReg = baseReg;
			this.codeW0 = codeW0;
			this.codeW1 = codeW1;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((state.flags & StateFlags.W) != 0)
				instruction.InternalCode = codeW1;
			else
				instruction.InternalCode = codeW0;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + baseReg;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp1Kind = OpKind.Register;
			instruction.InternalOp1Register = (int)state.vvvv + baseReg;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp2Kind = OpKind.Register;
				instruction.InternalOp2Register = (int)(state.rm + state.extraBaseRegisterBase) + ((state.flags & StateFlags.W) != 0 ? Register.RAX : Register.EAX);
			}
			else {
				instruction.InternalOp2Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
		}
	}

	sealed class OpCodeHandler_VEX_VHEvIb : OpCodeHandlerModRM {
		readonly Register baseReg;
		readonly Code codeW0;
		readonly Code codeW1;

		public OpCodeHandler_VEX_VHEvIb(Register baseReg, Code codeW0, Code codeW1) {
			this.baseReg = baseReg;
			this.codeW0 = codeW0;
			this.codeW1 = codeW1;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((state.flags & StateFlags.W) != 0)
				instruction.InternalCode = codeW1;
			else
				instruction.InternalCode = codeW0;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + baseReg;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp1Kind = OpKind.Register;
			instruction.InternalOp1Register = (int)state.vvvv + baseReg;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp2Kind = OpKind.Register;
				instruction.InternalOp2Register = (int)(state.rm + state.extraBaseRegisterBase) + ((state.flags & StateFlags.W) != 0 ? Register.RAX : Register.EAX);
			}
			else {
				instruction.InternalOp2Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
			instruction.InternalOp3Kind = OpKind.Immediate8;
			instruction.InternalImmediate8 = decoder.ReadByte();
		}
	}

	sealed class OpCodeHandler_VEX_VW : OpCodeHandlerModRM {
		readonly Register baseReg1;
		readonly Register baseReg2;
		readonly Code code;

		public OpCodeHandler_VEX_VW(Register baseReg, Code code) {
			baseReg1 = baseReg;
			baseReg2 = baseReg;
			this.code = code;
		}

		public OpCodeHandler_VEX_VW(Register baseReg1, Register baseReg2, Code code) {
			this.baseReg1 = baseReg1;
			this.baseReg2 = baseReg2;
			this.code = code;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			instruction.InternalCode = code;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + baseReg1;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp1Kind = OpKind.Register;
				instruction.InternalOp1Register = (int)(state.rm + state.extraBaseRegisterBase) + baseReg2;
			}
			else {
				instruction.InternalOp1Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
		}
	}

	sealed class OpCodeHandler_VEX_VX_Ev : OpCodeHandlerModRM {
		readonly Code code32;
		readonly Code code64;

		public OpCodeHandler_VEX_VX_Ev(Code code32, Code code64) {
			this.code32 = code32;
			this.code64 = code64;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			Register gpr;
			if ((state.flags & StateFlags.W) != 0) {
				instruction.InternalCode = code64;
				gpr = Register.RAX;
			}
			else {
				instruction.InternalCode = code32;
				gpr = Register.EAX;
			}
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + Register.XMM0;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp1Kind = OpKind.Register;
				instruction.InternalOp1Register = (int)(state.rm + state.extraBaseRegisterBase) + gpr;
			}
			else {
				instruction.InternalOp1Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
		}
	}

	sealed class OpCodeHandler_VEX_Ev_VX : OpCodeHandlerModRM {
		readonly Code code32;
		readonly Code code64;

		public OpCodeHandler_VEX_Ev_VX(Code code32, Code code64) {
			this.code32 = code32;
			this.code64 = code64;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			Register gpr;
			if ((state.flags & StateFlags.W) != 0) {
				instruction.InternalCode = code64;
				gpr = Register.RAX;
			}
			else {
				instruction.InternalCode = code32;
				gpr = Register.EAX;
			}
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp0Kind = OpKind.Register;
				instruction.InternalOp0Register = (int)(state.rm + state.extraBaseRegisterBase) + gpr;
			}
			else {
				instruction.InternalOp0Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp1Kind = OpKind.Register;
			instruction.InternalOp1Register = (int)(state.reg + state.extraRegisterBase) + Register.XMM0;
		}
	}

	sealed class OpCodeHandler_VEX_WV : OpCodeHandlerModRM {
		readonly Register baseReg1;
		readonly Register baseReg2;
		readonly Code code;

		public OpCodeHandler_VEX_WV(Register baseReg, Code code) {
			baseReg1 = baseReg;
			baseReg2 = baseReg;
			this.code = code;
		}

		public OpCodeHandler_VEX_WV(Register baseReg1, Register baseReg2, Code code) {
			this.baseReg1 = baseReg1;
			this.baseReg2 = baseReg2;
			this.code = code;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			instruction.InternalCode = code;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp0Kind = OpKind.Register;
				instruction.InternalOp0Register = (int)(state.rm + state.extraBaseRegisterBase) + baseReg2;
			}
			else {
				instruction.InternalOp0Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp1Kind = OpKind.Register;
			instruction.InternalOp1Register = (int)(state.reg + state.extraRegisterBase) + baseReg1;
		}
	}

	sealed class OpCodeHandler_VEX_VM : OpCodeHandlerModRM {
		readonly Register baseReg;
		readonly Code code;

		public OpCodeHandler_VEX_VM(Register baseReg, Code code) {
			this.baseReg = baseReg;
			this.code = code;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			instruction.InternalCode = code;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + baseReg;
			if (state.mod == 3)
				decoder.SetInvalidInstruction();
			else {
				instruction.InternalOp1Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
		}
	}

	sealed class OpCodeHandler_VEX_MV : OpCodeHandlerModRM {
		readonly Register baseReg;
		readonly Code code;

		public OpCodeHandler_VEX_MV(Register baseReg, Code code) {
			this.baseReg = baseReg;
			this.code = code;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			instruction.InternalCode = code;
			if (state.mod == 3)
				decoder.SetInvalidInstruction();
			else {
				instruction.InternalOp0Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp1Kind = OpKind.Register;
			instruction.InternalOp1Register = (int)(state.reg + state.extraRegisterBase) + baseReg;
		}
	}

	sealed class OpCodeHandler_VEX_M : OpCodeHandlerModRM {
		readonly Code code;

		public OpCodeHandler_VEX_M(Code code) {
			this.code = code;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			instruction.InternalCode = code;
			if (state.mod == 3)
				decoder.SetInvalidInstruction();
			else {
				instruction.InternalOp0Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
		}
	}

	sealed class OpCodeHandler_VEX_RdRq : OpCodeHandlerModRM {
		readonly Code code32;
		readonly Code code64;

		public OpCodeHandler_VEX_RdRq(Code code32, Code code64) {
			this.code32 = code32;
			this.code64 = code64;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			if ((state.flags & StateFlags.W) != 0) {
				instruction.InternalCode = code64;
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp0Kind = OpKind.Register;
				instruction.InternalOp0Register = (int)(state.rm + state.extraBaseRegisterBase) + Register.RAX;
			}
			else {
				instruction.InternalCode = code32;
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp0Kind = OpKind.Register;
				instruction.InternalOp0Register = (int)(state.rm + state.extraBaseRegisterBase) + Register.EAX;
			}
			if (state.mod != 3)
				decoder.SetInvalidInstruction();
		}
	}

	sealed class OpCodeHandler_VEX_rDI_VX_RX : OpCodeHandlerModRM {
		readonly Register baseReg;
		readonly Code code;

		public OpCodeHandler_VEX_rDI_VX_RX(Register baseReg, Code code) {
			this.baseReg = baseReg;
			this.code = code;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			instruction.InternalCode = code;
			Debug.Assert(state.addressSize == OpSize.Size32 || state.addressSize == OpSize.Size64);
			if (state.addressSize == OpSize.Size64)
				instruction.InternalOp0Kind = OpKind.MemorySegRDI;
			else
				instruction.InternalOp0Kind = OpKind.MemorySegEDI;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp1Kind = OpKind.Register;
			instruction.InternalOp1Register = (int)(state.reg + state.extraRegisterBase) + baseReg;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp2Kind = OpKind.Register;
				instruction.InternalOp2Register = (int)(state.rm + state.extraBaseRegisterBase) + baseReg;
			}
			else
				decoder.SetInvalidInstruction();
		}
	}

	sealed class OpCodeHandler_VEX_VWIb : OpCodeHandlerModRM {
		readonly Register baseReg1;
		readonly Register baseReg2;
		readonly Code codeW0;
		readonly Code codeW1;

		public OpCodeHandler_VEX_VWIb(Register baseReg, Code code) {
			baseReg1 = baseReg;
			baseReg2 = baseReg;
			codeW0 = code;
			codeW1 = code;
		}

		public OpCodeHandler_VEX_VWIb(Register baseReg, Code codeW0, Code codeW1) {
			baseReg1 = baseReg;
			baseReg2 = baseReg;
			this.codeW0 = codeW0;
			this.codeW1 = codeW1;
		}

		public OpCodeHandler_VEX_VWIb(Register baseReg1, Register baseReg2, Code code) {
			this.baseReg1 = baseReg1;
			this.baseReg2 = baseReg2;
			codeW0 = code;
			codeW1 = code;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			if ((state.flags & StateFlags.W) != 0)
				instruction.InternalCode = codeW1;
			else
				instruction.InternalCode = codeW0;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + baseReg1;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp1Kind = OpKind.Register;
				instruction.InternalOp1Register = (int)(state.rm + state.extraBaseRegisterBase) + baseReg2;
			}
			else {
				instruction.InternalOp1Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
			instruction.InternalOp2Kind = OpKind.Immediate8;
			instruction.InternalImmediate8 = decoder.ReadByte();
		}
	}

	sealed class OpCodeHandler_VEX_WVIb : OpCodeHandlerModRM {
		readonly Register baseReg1;
		readonly Register baseReg2;
		readonly Code code;

		public OpCodeHandler_VEX_WVIb(Register baseReg, Code code) {
			baseReg1 = baseReg;
			baseReg2 = baseReg;
			this.code = code;
		}

		public OpCodeHandler_VEX_WVIb(Register baseReg1, Register baseReg2, Code code) {
			this.baseReg1 = baseReg1;
			this.baseReg2 = baseReg2;
			this.code = code;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			instruction.InternalCode = code;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp0Kind = OpKind.Register;
				instruction.InternalOp0Register = (int)(state.rm + state.extraBaseRegisterBase) + baseReg1;
			}
			else {
				instruction.InternalOp0Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp1Kind = OpKind.Register;
			instruction.InternalOp1Register = (int)(state.reg + state.extraRegisterBase) + baseReg2;
			instruction.InternalOp2Kind = OpKind.Immediate8;
			instruction.InternalImmediate8 = decoder.ReadByte();
		}
	}

	sealed class OpCodeHandler_VEX_Ed_V_Ib : OpCodeHandlerModRM {
		readonly Register baseReg;
		readonly Code code32;
		readonly Code code64;

		public OpCodeHandler_VEX_Ed_V_Ib(Register baseReg, Code code32, Code code64) {
			this.baseReg = baseReg;
			this.code32 = code32;
			this.code64 = code64;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			Register gpr;
			if ((state.flags & StateFlags.W) != 0) {
				instruction.InternalCode = code64;
				gpr = Register.RAX;
			}
			else {
				instruction.InternalCode = code32;
				gpr = Register.EAX;
			}
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp0Kind = OpKind.Register;
				instruction.InternalOp0Register = (int)(state.rm + state.extraBaseRegisterBase) + gpr;
			}
			else {
				instruction.InternalOp0Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp1Kind = OpKind.Register;
			instruction.InternalOp1Register = (int)(state.reg + state.extraRegisterBase) + baseReg;
			instruction.InternalOp2Kind = OpKind.Immediate8;
			instruction.InternalImmediate8 = decoder.ReadByte();
		}
	}

	sealed class OpCodeHandler_VEX_VHW : OpCodeHandlerModRM {
		readonly Register baseReg1;
		readonly Register baseReg2;
		readonly Register baseReg3;
		readonly Code codeR;
		readonly Code codeM;

		public OpCodeHandler_VEX_VHW(Register baseReg, Code codeR, Code codeM) {
			baseReg1 = baseReg;
			baseReg2 = baseReg;
			baseReg3 = baseReg;
			this.codeR = codeR;
			this.codeM = codeM;
		}

		public OpCodeHandler_VEX_VHW(Register baseReg, Code code) {
			baseReg1 = baseReg;
			baseReg2 = baseReg;
			baseReg3 = baseReg;
			codeR = code;
			codeM = code;
		}

		public OpCodeHandler_VEX_VHW(Register baseReg1, Register baseReg2, Register baseReg3, Code code) {
			this.baseReg1 = baseReg1;
			this.baseReg2 = baseReg2;
			this.baseReg3 = baseReg3;
			codeR = code;
			codeM = code;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + baseReg1;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp1Kind = OpKind.Register;
			instruction.InternalOp1Register = (int)state.vvvv + baseReg2;
			if (state.mod == 3) {
				instruction.InternalCode = codeR;
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp2Kind = OpKind.Register;
				instruction.InternalOp2Register = (int)(state.rm + state.extraBaseRegisterBase) + baseReg3;
			}
			else {
				instruction.InternalCode = codeM;
				instruction.InternalOp2Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
		}
	}

	sealed class OpCodeHandler_VEX_VWH : OpCodeHandlerModRM {
		readonly Register baseReg;
		readonly Code code;

		public OpCodeHandler_VEX_VWH(Register baseReg, Code code) {
			this.baseReg = baseReg;
			this.code = code;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			instruction.InternalCode = code;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + baseReg;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp1Kind = OpKind.Register;
				instruction.InternalOp1Register = (int)(state.rm + state.extraBaseRegisterBase) + baseReg;
			}
			else {
				instruction.InternalOp1Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp2Kind = OpKind.Register;
			instruction.InternalOp2Register = (int)state.vvvv + baseReg;
		}
	}

	sealed class OpCodeHandler_VEX_WHV : OpCodeHandlerModRM {
		readonly Register baseReg;
		readonly Code codeR;
		readonly Code codeM;

		public OpCodeHandler_VEX_WHV(Register baseReg, Code codeR, Code codeM) {
			this.baseReg = baseReg;
			this.codeR = codeR;
			this.codeM = codeM;
		}

		public OpCodeHandler_VEX_WHV(Register baseReg, Code code) {
			this.baseReg = baseReg;
			codeR = code;
			codeM = code;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if (state.mod == 3) {
				instruction.InternalCode = codeR;
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp0Kind = OpKind.Register;
				instruction.InternalOp0Register = (int)(state.rm + state.extraBaseRegisterBase) + baseReg;
			}
			else {
				instruction.InternalCode = codeM;
				instruction.InternalOp0Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp1Kind = OpKind.Register;
			instruction.InternalOp1Register = (int)state.vvvv + baseReg;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp2Kind = OpKind.Register;
			instruction.InternalOp2Register = (int)(state.reg + state.extraRegisterBase) + baseReg;
		}
	}

	sealed class OpCodeHandler_VEX_VHM : OpCodeHandlerModRM {
		readonly Register baseReg;
		readonly Code code;

		public OpCodeHandler_VEX_VHM(Register baseReg, Code code) {
			this.baseReg = baseReg;
			this.code = code;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			instruction.InternalCode = code;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + baseReg;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp1Kind = OpKind.Register;
			instruction.InternalOp1Register = (int)state.vvvv + baseReg;
			if (state.mod == 3)
				decoder.SetInvalidInstruction();
			else {
				instruction.InternalOp2Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
		}
	}

	sealed class OpCodeHandler_VEX_MHV : OpCodeHandlerModRM {
		readonly Register baseReg;
		readonly Code code;

		public OpCodeHandler_VEX_MHV(Register baseReg, Code code) {
			this.baseReg = baseReg;
			this.code = code;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			instruction.InternalCode = code;
			if (state.mod == 3)
				decoder.SetInvalidInstruction();
			else {
				instruction.InternalOp0Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp1Kind = OpKind.Register;
			instruction.InternalOp1Register = (int)state.vvvv + baseReg;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp2Kind = OpKind.Register;
			instruction.InternalOp2Register = (int)(state.reg + state.extraRegisterBase) + baseReg;
		}
	}

	sealed class OpCodeHandler_VEX_VHWIb : OpCodeHandlerModRM {
		readonly Register baseReg1;
		readonly Register baseReg2;
		readonly Register baseReg3;
		readonly Code code;

		public OpCodeHandler_VEX_VHWIb(Register baseReg, Code code) {
			baseReg1 = baseReg;
			baseReg2 = baseReg;
			baseReg3 = baseReg;
			this.code = code;
		}

		public OpCodeHandler_VEX_VHWIb(Register baseReg1, Register baseReg2, Register baseReg3, Code code) {
			this.baseReg1 = baseReg1;
			this.baseReg2 = baseReg2;
			this.baseReg3 = baseReg3;
			this.code = code;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			instruction.InternalCode = code;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + baseReg1;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp1Kind = OpKind.Register;
			instruction.InternalOp1Register = (int)state.vvvv + baseReg2;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp2Kind = OpKind.Register;
				instruction.InternalOp2Register = (int)(state.rm + state.extraBaseRegisterBase) + baseReg3;
			}
			else {
				instruction.InternalOp2Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
			instruction.InternalOp3Kind = OpKind.Immediate8;
			instruction.InternalImmediate8 = decoder.ReadByte();
		}
	}

	sealed class OpCodeHandler_VEX_HRIb : OpCodeHandlerModRM {
		readonly Register baseReg;
		readonly Code code;

		public OpCodeHandler_VEX_HRIb(Register baseReg, Code code) {
			this.baseReg = baseReg;
			this.code = code;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			instruction.InternalCode = code;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)state.vvvv + baseReg;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp1Kind = OpKind.Register;
				instruction.InternalOp1Register = (int)(state.rm + state.extraBaseRegisterBase) + baseReg;
			}
			else
				decoder.SetInvalidInstruction();
			instruction.InternalOp2Kind = OpKind.Immediate8;
			instruction.InternalImmediate8 = decoder.ReadByte();
		}
	}

	sealed class OpCodeHandler_VEX_VHWIs4 : OpCodeHandlerModRM {
		readonly Register baseReg;
		readonly Code code;

		public OpCodeHandler_VEX_VHWIs4(Register baseReg, Code code) {
			this.baseReg = baseReg;
			this.code = code;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			instruction.InternalCode = code;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + baseReg;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp1Kind = OpKind.Register;
			instruction.InternalOp1Register = (int)state.vvvv + baseReg;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp2Kind = OpKind.Register;
				instruction.InternalOp2Register = (int)(state.rm + state.extraBaseRegisterBase) + baseReg;
			}
			else {
				instruction.InternalOp2Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp3Kind = OpKind.Register;
			instruction.InternalOp3Register = (int)(decoder.ReadByte() >> 4) + baseReg;
		}
	}

	sealed class OpCodeHandler_VEX_VHIs4W : OpCodeHandlerModRM {
		readonly Register baseReg;
		readonly Code code;

		public OpCodeHandler_VEX_VHIs4W(Register baseReg, Code code) {
			this.baseReg = baseReg;
			this.code = code;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			instruction.InternalCode = code;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + baseReg;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp1Kind = OpKind.Register;
			instruction.InternalOp1Register = (int)state.vvvv + baseReg;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp3Kind = OpKind.Register;
				instruction.InternalOp3Register = (int)(state.rm + state.extraBaseRegisterBase) + baseReg;
			}
			else {
				instruction.InternalOp3Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp2Kind = OpKind.Register;
			instruction.InternalOp2Register = (int)(decoder.ReadByte() >> 4) + baseReg;
		}
	}

	sealed class OpCodeHandler_VEX_VHWIs5 : OpCodeHandlerModRM {
		readonly Register baseReg;
		readonly Code code;

		public OpCodeHandler_VEX_VHWIs5(Register baseReg, Code code) {
			this.baseReg = baseReg;
			this.code = code;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			instruction.InternalCode = code;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + baseReg;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp1Kind = OpKind.Register;
			instruction.InternalOp1Register = (int)state.vvvv + baseReg;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp2Kind = OpKind.Register;
				instruction.InternalOp2Register = (int)(state.rm + state.extraBaseRegisterBase) + baseReg;
			}
			else {
				instruction.InternalOp2Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
			uint ib = decoder.ReadByte();
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp3Kind = OpKind.Register;
			instruction.InternalOp3Register = (int)(ib >> 4) + baseReg;
			Debug.Assert(instruction.Op4Kind == OpKind.Immediate8);// It's hard coded
			instruction.InternalImmediate8 = ib & 3;
		}
	}

	sealed class OpCodeHandler_VEX_VHIs5W : OpCodeHandlerModRM {
		readonly Register baseReg;
		readonly Code code;

		public OpCodeHandler_VEX_VHIs5W(Register baseReg, Code code) {
			this.baseReg = baseReg;
			this.code = code;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			instruction.InternalCode = code;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + baseReg;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp1Kind = OpKind.Register;
			instruction.InternalOp1Register = (int)state.vvvv + baseReg;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp3Kind = OpKind.Register;
				instruction.InternalOp3Register = (int)(state.rm + state.extraBaseRegisterBase) + baseReg;
			}
			else {
				instruction.InternalOp3Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
			uint ib = decoder.ReadByte();
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp2Kind = OpKind.Register;
			instruction.InternalOp2Register = (int)(ib >> 4) + baseReg;
			Debug.Assert(instruction.Op4Kind == OpKind.Immediate8);// It's hard coded
			instruction.InternalImmediate8 = ib & 3;
		}
	}

	sealed class OpCodeHandler_VEX_VK_HK_RK : OpCodeHandlerModRM {
		readonly Code code;

		public OpCodeHandler_VEX_VK_HK_RK(Code code) => this.code = code;

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv > 7) {
				decoder.SetInvalidInstruction();
				return;
			}
			instruction.InternalCode = code;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)state.reg + Register.K0;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp1Kind = OpKind.Register;
			instruction.InternalOp1Register = (int)state.vvvv + Register.K0;// vvvv is valid, see above
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp2Kind = OpKind.Register;
				instruction.InternalOp2Register = (int)state.rm + Register.K0;
			}
			else
				decoder.SetInvalidInstruction();
		}
	}

	sealed class OpCodeHandler_VEX_VK_RK : OpCodeHandlerModRM {
		readonly Code code;

		public OpCodeHandler_VEX_VK_RK(Code code) => this.code = code;

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			instruction.InternalCode = code;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)state.reg + Register.K0;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp1Kind = OpKind.Register;
				instruction.InternalOp1Register = (int)state.rm + Register.K0;
			}
			else
				decoder.SetInvalidInstruction();
		}
	}

	sealed class OpCodeHandler_VEX_VK_RK_Ib : OpCodeHandlerModRM {
		readonly Code code;

		public OpCodeHandler_VEX_VK_RK_Ib(Code code) => this.code = code;

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			instruction.InternalCode = code;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)state.reg + Register.K0;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp1Kind = OpKind.Register;
				instruction.InternalOp1Register = (int)state.rm + Register.K0;
			}
			else
				decoder.SetInvalidInstruction();
			instruction.InternalOp2Kind = OpKind.Immediate8;
			instruction.InternalImmediate8 = decoder.ReadByte();
		}
	}

	sealed class OpCodeHandler_VEX_VK_WK : OpCodeHandlerModRM {
		readonly Code code;

		public OpCodeHandler_VEX_VK_WK(Code code) {
			this.code = code;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			instruction.InternalCode = code;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)state.reg + Register.K0;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp1Kind = OpKind.Register;
				instruction.InternalOp1Register = (int)state.rm + Register.K0;
			}
			else {
				instruction.InternalOp1Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
		}
	}

	sealed class OpCodeHandler_VEX_MK_VK : OpCodeHandlerModRM {
		readonly Code code;

		public OpCodeHandler_VEX_MK_VK(Code code) {
			this.code = code;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			instruction.InternalCode = code;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp0Kind = OpKind.Register;
				instruction.InternalOp0Register = (int)state.rm + Register.K0;
			}
			else {
				instruction.InternalOp0Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp1Kind = OpKind.Register;
			instruction.InternalOp1Register = (int)state.reg + Register.K0;
		}
	}

	sealed class OpCodeHandler_VEX_VK_R : OpCodeHandlerModRM {
		readonly Code code;
		readonly Register gpr;

		public OpCodeHandler_VEX_VK_R(Code code, Register gpr) {
			this.code = code;
			this.gpr = gpr;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			instruction.InternalCode = code;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)state.reg + Register.K0;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp1Kind = OpKind.Register;
				instruction.InternalOp1Register = (int)(state.rm + state.extraBaseRegisterBase) + gpr;
			}
			else
				decoder.SetInvalidInstruction();
		}
	}

	sealed class OpCodeHandler_VEX_G_VK : OpCodeHandlerModRM {
		readonly Code code;
		readonly Register gpr;

		public OpCodeHandler_VEX_G_VK(Code code, Register gpr) {
			this.code = code;
			this.gpr = gpr;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			instruction.InternalCode = code;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + gpr;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp1Kind = OpKind.Register;
				instruction.InternalOp1Register = (int)state.rm + Register.K0;
			}
			else
				decoder.SetInvalidInstruction();
		}
	}

	sealed class OpCodeHandler_VEX_Gv_W : OpCodeHandlerModRM {
		readonly Register baseReg;
		readonly Code codeW0;
		readonly Code codeW1;

		public OpCodeHandler_VEX_Gv_W(Register baseReg, Code codeW0, Code codeW1) {
			this.baseReg = baseReg;
			this.codeW0 = codeW0;
			this.codeW1 = codeW1;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			if ((state.flags & StateFlags.W) != 0) {
				instruction.InternalCode = codeW1;
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp0Kind = OpKind.Register;
				instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + Register.RAX;
			}
			else {
				instruction.InternalCode = codeW0;
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp0Kind = OpKind.Register;
				instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + Register.EAX;
			}
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp1Kind = OpKind.Register;
				instruction.InternalOp1Register = (int)(state.rm + state.extraBaseRegisterBase) + baseReg;
			}
			else {
				instruction.InternalOp1Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
		}
	}

	sealed class OpCodeHandler_VEX_Gv_RX : OpCodeHandlerModRM {
		readonly Register baseReg;
		readonly Code code32;
		readonly Code code64;

		public OpCodeHandler_VEX_Gv_RX(Register baseReg, Code code32, Code code64) {
			this.baseReg = baseReg;
			this.code32 = code32;
			this.code64 = code64;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			if ((state.flags & StateFlags.W) != 0)
				instruction.InternalCode = code64;
			else
				instruction.InternalCode = code32;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + ((state.flags & StateFlags.W) != 0 ? Register.RAX : Register.EAX);
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp1Kind = OpKind.Register;
				instruction.InternalOp1Register = (int)(state.rm + state.extraBaseRegisterBase) + baseReg;
			}
			else
				decoder.SetInvalidInstruction();
		}
	}

	sealed class OpCodeHandler_VEX_Gv_GPR_Ib : OpCodeHandlerModRM {
		readonly Register baseReg;
		readonly Code code32;
		readonly Code code64;

		public OpCodeHandler_VEX_Gv_GPR_Ib(Register baseReg, Code code32, Code code64) {
			this.baseReg = baseReg;
			this.code32 = code32;
			this.code64 = code64;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			Register gpr;
			if ((state.flags & StateFlags.W) != 0) {
				instruction.InternalCode = code64;
				gpr = Register.RAX;
			}
			else {
				instruction.InternalCode = code32;
				gpr = Register.EAX;
			}
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + gpr;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp1Kind = OpKind.Register;
				instruction.InternalOp1Register = (int)(state.rm + state.extraBaseRegisterBase) + baseReg;
			}
			else
				decoder.SetInvalidInstruction();
			instruction.InternalOp2Kind = OpKind.Immediate8;
			instruction.InternalImmediate8 = decoder.ReadByte();
		}
	}

	sealed class OpCodeHandler_VEX_VX_VSIB_HX : OpCodeHandlerModRM {
		readonly Register baseReg1;
		readonly Register vsibIndex;
		readonly Register baseReg3;
		readonly Code code;

		public OpCodeHandler_VEX_VX_VSIB_HX(Register baseReg1, Register vsibIndex, Register baseReg3, Code code) {
			this.baseReg1 = baseReg1;
			this.vsibIndex = vsibIndex;
			this.baseReg3 = baseReg3;
			this.code = code;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			instruction.InternalCode = code;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + baseReg1;
			if (state.mod == 3)
				decoder.SetInvalidInstruction();
			else {
				instruction.InternalOp1Kind = OpKind.Memory;
				decoder.ReadOpMem_VSIB_m64(ref instruction, vsibIndex);
			}
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp2Kind = OpKind.Register;
			instruction.InternalOp2Register = (int)state.vvvv + baseReg3;
		}
	}

	sealed class OpCodeHandler_VEX_Gv_Gv_Ev : OpCodeHandlerModRM {
		readonly Code code32;
		readonly Code code64;

		public OpCodeHandler_VEX_Gv_Gv_Ev(Code code32, Code code64) {
			this.code32 = code32;
			this.code64 = code64;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			Register gpr;
			if ((state.flags & StateFlags.W) != 0) {
				instruction.InternalCode = code64;
				gpr = Register.RAX;
			}
			else {
				instruction.InternalCode = code32;
				gpr = Register.EAX;
			}
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + gpr;
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp1Kind = OpKind.Register;
			instruction.InternalOp1Register = (int)state.vvvv + gpr;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp2Kind = OpKind.Register;
				instruction.InternalOp2Register = (int)(state.rm + state.extraBaseRegisterBase) + gpr;
			}
			else {
				instruction.InternalOp2Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
		}
	}

	sealed class OpCodeHandler_VEX_Gv_Ev_Gv : OpCodeHandlerModRM {
		readonly Code code32;
		readonly Code code64;

		public OpCodeHandler_VEX_Gv_Ev_Gv(Code code32, Code code64) {
			this.code32 = code32;
			this.code64 = code64;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			Register gpr;
			if ((state.flags & StateFlags.W) != 0) {
				instruction.InternalCode = code64;
				gpr = Register.RAX;
			}
			else {
				instruction.InternalCode = code32;
				gpr = Register.EAX;
			}
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + gpr;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp1Kind = OpKind.Register;
				instruction.InternalOp1Register = (int)(state.rm + state.extraBaseRegisterBase) + gpr;
			}
			else {
				instruction.InternalOp1Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp2Kind = OpKind.Register;
			instruction.InternalOp2Register = (int)state.vvvv + gpr;
		}
	}

	sealed class OpCodeHandler_VEX_Hv_Ev : OpCodeHandlerModRM {
		readonly Code code32;
		readonly Code code64;

		public OpCodeHandler_VEX_Hv_Ev(Code code32, Code code64) {
			this.code32 = code32;
			this.code64 = code64;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			Register gpr;
			if ((state.flags & StateFlags.W) != 0) {
				instruction.InternalCode = code64;
				gpr = Register.RAX;
			}
			else {
				instruction.InternalCode = code32;
				gpr = Register.EAX;
			}
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)state.vvvv + gpr;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp1Kind = OpKind.Register;
				instruction.InternalOp1Register = (int)(state.rm + state.extraBaseRegisterBase) + gpr;
			}
			else {
				instruction.InternalOp1Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
		}
	}

	sealed class OpCodeHandler_VEX_Hv_Ed_Id : OpCodeHandlerModRM {
		readonly Code code32;
		readonly Code code64;

		public OpCodeHandler_VEX_Hv_Ed_Id(Code code32, Code code64) {
			this.code32 = code32;
			this.code64 = code64;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((state.flags & StateFlags.W) != 0) {
				instruction.InternalCode = code64;
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp0Kind = OpKind.Register;
				instruction.InternalOp0Register = (int)state.vvvv + Register.RAX;
			}
			else {
				instruction.InternalCode = code32;
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp0Kind = OpKind.Register;
				instruction.InternalOp0Register = (int)state.vvvv + Register.EAX;
			}
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp1Kind = OpKind.Register;
				instruction.InternalOp1Register = (int)(state.rm + state.extraBaseRegisterBase) + Register.EAX;
			}
			else {
				instruction.InternalOp1Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
			instruction.InternalOp2Kind = OpKind.Immediate32;
			instruction.Immediate32 = decoder.ReadUInt32();
		}
	}

	sealed class OpCodeHandler_VEX_GvM_VX_Ib : OpCodeHandlerModRM {
		readonly Register baseReg;
		readonly Code code32;
		readonly Code code64;

		public OpCodeHandler_VEX_GvM_VX_Ib(Register baseReg, Code code32, Code code64) {
			this.baseReg = baseReg;
			this.code32 = code32;
			this.code64 = code64;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			Register gpr;
			if ((state.flags & StateFlags.W) != 0) {
				instruction.InternalCode = code64;
				gpr = Register.RAX;
			}
			else {
				instruction.InternalCode = code32;
				gpr = Register.EAX;
			}
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp0Kind = OpKind.Register;
				instruction.InternalOp0Register = (int)(state.rm + state.extraBaseRegisterBase) + gpr;
			}
			else {
				instruction.InternalOp0Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp1Kind = OpKind.Register;
			instruction.InternalOp1Register = (int)(state.reg + state.extraRegisterBase) + baseReg;
			instruction.InternalOp2Kind = OpKind.Immediate8;
			instruction.InternalImmediate8 = decoder.ReadByte();
		}
	}

	sealed class OpCodeHandler_VEX_Gv_Ev_Ib : OpCodeHandlerModRM {
		readonly Code code32;
		readonly Code code64;

		public OpCodeHandler_VEX_Gv_Ev_Ib(Code code32, Code code64) {
			this.code32 = code32;
			this.code64 = code64;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			Register gpr;
			if ((state.flags & StateFlags.W) != 0) {
				instruction.InternalCode = code64;
				gpr = Register.RAX;
			}
			else {
				instruction.InternalCode = code32;
				gpr = Register.EAX;
			}
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + gpr;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp1Kind = OpKind.Register;
				instruction.InternalOp1Register = (int)(state.rm + state.extraBaseRegisterBase) + gpr;
			}
			else {
				instruction.InternalOp1Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
			instruction.InternalOp2Kind = OpKind.Immediate8;
			instruction.InternalImmediate8 = decoder.ReadByte();
		}
	}

	sealed class OpCodeHandler_VEX_Gv_Ev_Id : OpCodeHandlerModRM {
		readonly Code code32;
		readonly Code code64;

		public OpCodeHandler_VEX_Gv_Ev_Id(Code code32, Code code64) {
			this.code32 = code32;
			this.code64 = code64;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction) {
			ref var state = ref decoder.state;
			Debug.Assert(state.Encoding == EncodingKind.VEX || state.Encoding == EncodingKind.XOP);
			if ((int)state.vvvv != 0) {
				decoder.SetInvalidInstruction();
				return;
			}
			Register gpr;
			if ((state.flags & StateFlags.W) != 0) {
				instruction.InternalCode = code64;
				gpr = Register.RAX;
			}
			else {
				instruction.InternalCode = code32;
				gpr = Register.EAX;
			}
			Debug.Assert(OpKind.Register == 0);
			//instruction.InternalOp0Kind = OpKind.Register;
			instruction.InternalOp0Register = (int)(state.reg + state.extraRegisterBase) + gpr;
			if (state.mod == 3) {
				Debug.Assert(OpKind.Register == 0);
				//instruction.InternalOp1Kind = OpKind.Register;
				instruction.InternalOp1Register = (int)(state.rm + state.extraBaseRegisterBase) + gpr;
			}
			else {
				instruction.InternalOp1Kind = OpKind.Memory;
				decoder.ReadOpMem_m64(ref instruction);
			}
			instruction.InternalOp2Kind = OpKind.Immediate32;
			instruction.Immediate32 = decoder.ReadUInt32();
		}
	}
}
#endif
