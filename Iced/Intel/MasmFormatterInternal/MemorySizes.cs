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

#if !NO_MASM_FORMATTER && !NO_FORMATTER
using System;

namespace Iced.Intel.MasmFormatterInternal {
	static class MemorySizes {
		static readonly string[] byte_ptr = new string[] { "byte", "ptr" };
		static readonly string[] word_ptr = new string[] { "word", "ptr" };
		internal static readonly string[] dword_ptr = new string[] { "dword", "ptr" };
		internal static readonly string[] qword_ptr = new string[] { "qword", "ptr" };
		internal static readonly string[] mmword_ptr = new string[] { "mmword", "ptr" };
		internal static readonly string[] xmmword_ptr = new string[] { "xmmword", "ptr" };
		static readonly string[] ymmword_ptr = new string[] { "ymmword", "ptr" };
		static readonly string[] zmmword_ptr = new string[] { "zmmword", "ptr" };
		static readonly string[] fword_ptr = new string[] { "fword", "ptr" };
		static readonly string[] tbyte_ptr = new string[] { "tbyte", "ptr" };
		internal static readonly string[] oword_ptr = new string[] { "oword", "ptr" };
		static readonly string[] dword_bcst = new string[] { "dword", "bcst" };
		static readonly string[] qword_bcst = new string[] { "qword", "bcst" };

		public static readonly (MemorySize memorySize, bool isBroadcast, int size, string[] names)[] AllMemorySizes = new(MemorySize memorySize, bool isBroadcast, int size, string[] names)[DecoderConstants.NumberOfMemorySizes] {
			(MemorySize.Unknown, false, 0, Array.Empty<string>()),
			(MemorySize.UInt8, false, 1, byte_ptr),
			(MemorySize.UInt16, false, 2, word_ptr),
			(MemorySize.UInt32, false, 4, dword_ptr),
			(MemorySize.UInt52, false, 8, qword_ptr),
			(MemorySize.UInt64, false, 8, qword_ptr),
			(MemorySize.UInt128, false, 16, xmmword_ptr),
			(MemorySize.UInt256, false, 32, ymmword_ptr),
			(MemorySize.UInt512, false, 64, zmmword_ptr),
			(MemorySize.Int8, false, 1, byte_ptr),
			(MemorySize.Int16, false, 2, word_ptr),
			(MemorySize.Int32, false, 4, dword_ptr),
			(MemorySize.Int64, false, 8, qword_ptr),
			(MemorySize.Int128, false, 16, xmmword_ptr),
			(MemorySize.Int256, false, 32, ymmword_ptr),
			(MemorySize.Int512, false, 64, zmmword_ptr),
			(MemorySize.SegPtr16, false, 4, dword_ptr),
			(MemorySize.SegPtr32, false, 6, fword_ptr),
			(MemorySize.SegPtr64, false, 10, tbyte_ptr),
			(MemorySize.WordOffset, false, 2, word_ptr),
			(MemorySize.DwordOffset, false, 4, dword_ptr),
			(MemorySize.QwordOffset, false, 8, qword_ptr),
			(MemorySize.Bound16_WordWord, false, 4, dword_ptr),
			(MemorySize.Bound32_DwordDword, false, 8, qword_ptr),
			(MemorySize.Bnd32, false, 8, qword_ptr),
			(MemorySize.Bnd64, false, 16, oword_ptr),
			(MemorySize.Fword5, false, 5, fword_ptr),
			(MemorySize.Fword6, false, 6, fword_ptr),
			(MemorySize.Fword10, false, 10, fword_ptr),
			(MemorySize.Float16, false, 2, word_ptr),
			(MemorySize.Float32, false, 4, dword_ptr),
			(MemorySize.Float64, false, 8, qword_ptr),
			(MemorySize.Float80, false, 10, tbyte_ptr),
			(MemorySize.Float128, false, 16, xmmword_ptr),
			(MemorySize.FpuEnv14, false, 14, "fpuenv14 ptr".Split(' ')),
			(MemorySize.FpuEnv28, false, 28, "fpuenv28 ptr".Split(' ')),
			(MemorySize.FpuState94, false, 94, "fpustate94 ptr".Split(' ')),
			(MemorySize.FpuState108, false, 108, "fpustate108 ptr".Split(' ')),
			(MemorySize.Fxsave_512Byte, false, 512, Array.Empty<string>()),
			(MemorySize.Fxsave64_512Byte, false, 512, Array.Empty<string>()),
			(MemorySize.Xsave, false, 0, Array.Empty<string>()),
			(MemorySize.Xsave64, false, 0, Array.Empty<string>()),
			(MemorySize.Bcd, false, 10, tbyte_ptr),
			(MemorySize.Packed16_UInt8, false, 2, word_ptr),
			(MemorySize.Packed16_Int8, false, 2, word_ptr),
			(MemorySize.Packed32_UInt8, false, 4, dword_ptr),
			(MemorySize.Packed32_Int8, false, 4, dword_ptr),
			(MemorySize.Packed32_UInt16, false, 4, dword_ptr),
			(MemorySize.Packed32_Int16, false, 4, dword_ptr),
			(MemorySize.Packed64_UInt8, false, 8, qword_ptr),
			(MemorySize.Packed64_Int8, false, 8, qword_ptr),
			(MemorySize.Packed64_UInt16, false, 8, qword_ptr),
			(MemorySize.Packed64_Int16, false, 8, qword_ptr),
			(MemorySize.Packed64_UInt32, false, 8, qword_ptr),
			(MemorySize.Packed64_Int32, false, 8, qword_ptr),
			(MemorySize.Packed64_Float16, false, 8, qword_ptr),
			(MemorySize.Packed64_Float32, false, 8, qword_ptr),
			(MemorySize.Packed128_UInt8, false, 16, xmmword_ptr),
			(MemorySize.Packed128_Int8, false, 16, xmmword_ptr),
			(MemorySize.Packed128_UInt16, false, 16, xmmword_ptr),
			(MemorySize.Packed128_Int16, false, 16, xmmword_ptr),
			(MemorySize.Packed128_UInt32, false, 16, xmmword_ptr),
			(MemorySize.Packed128_Int32, false, 16, xmmword_ptr),
			(MemorySize.Packed128_UInt52, false, 16, xmmword_ptr),
			(MemorySize.Packed128_UInt64, false, 16, xmmword_ptr),
			(MemorySize.Packed128_Int64, false, 16, xmmword_ptr),
			(MemorySize.Packed128_Float16, false, 16, xmmword_ptr),
			(MemorySize.Packed128_Float32, false, 16, xmmword_ptr),
			(MemorySize.Packed128_Float64, false, 16, xmmword_ptr),
			(MemorySize.Packed256_UInt8, false, 32, ymmword_ptr),
			(MemorySize.Packed256_Int8, false, 32, ymmword_ptr),
			(MemorySize.Packed256_UInt16, false, 32, ymmword_ptr),
			(MemorySize.Packed256_Int16, false, 32, ymmword_ptr),
			(MemorySize.Packed256_UInt32, false, 32, ymmword_ptr),
			(MemorySize.Packed256_Int32, false, 32, ymmword_ptr),
			(MemorySize.Packed256_UInt52, false, 32, ymmword_ptr),
			(MemorySize.Packed256_UInt64, false, 32, ymmword_ptr),
			(MemorySize.Packed256_Int64, false, 32, ymmword_ptr),
			(MemorySize.Packed256_UInt128, false, 32, ymmword_ptr),
			(MemorySize.Packed256_Int128, false, 32, ymmword_ptr),
			(MemorySize.Packed256_Float16, false, 32, ymmword_ptr),
			(MemorySize.Packed256_Float32, false, 32, ymmword_ptr),
			(MemorySize.Packed256_Float64, false, 32, ymmword_ptr),
			(MemorySize.Packed256_Float128, false, 32, ymmword_ptr),
			(MemorySize.Packed512_UInt8, false, 64, zmmword_ptr),
			(MemorySize.Packed512_Int8, false, 64, zmmword_ptr),
			(MemorySize.Packed512_UInt16, false, 64, zmmword_ptr),
			(MemorySize.Packed512_Int16, false, 64, zmmword_ptr),
			(MemorySize.Packed512_UInt32, false, 64, zmmword_ptr),
			(MemorySize.Packed512_Int32, false, 64, zmmword_ptr),
			(MemorySize.Packed512_UInt52, false, 64, zmmword_ptr),
			(MemorySize.Packed512_UInt64, false, 64, zmmword_ptr),
			(MemorySize.Packed512_Int64, false, 64, zmmword_ptr),
			(MemorySize.Packed512_UInt128, false, 64, zmmword_ptr),
			(MemorySize.Packed512_Float32, false, 64, zmmword_ptr),
			(MemorySize.Packed512_Float64, false, 64, zmmword_ptr),
			(MemorySize.Broadcast64_UInt32, true, 4, dword_bcst),
			(MemorySize.Broadcast64_Int32, true, 4, dword_bcst),
			(MemorySize.Broadcast64_Float32, true, 4, dword_bcst),
			(MemorySize.Broadcast128_UInt32, true, 4, dword_bcst),
			(MemorySize.Broadcast128_Int32, true, 4, dword_bcst),
			(MemorySize.Broadcast128_UInt52, true, 8, qword_bcst),
			(MemorySize.Broadcast128_UInt64, true, 8, qword_bcst),
			(MemorySize.Broadcast128_Int64, true, 8, qword_bcst),
			(MemorySize.Broadcast128_Float32, true, 4, dword_bcst),
			(MemorySize.Broadcast128_Float64, true, 8, qword_bcst),
			(MemorySize.Broadcast256_UInt32, true, 4, dword_bcst),
			(MemorySize.Broadcast256_Int32, true, 4, dword_bcst),
			(MemorySize.Broadcast256_UInt52, true, 8, qword_bcst),
			(MemorySize.Broadcast256_UInt64, true, 8, qword_bcst),
			(MemorySize.Broadcast256_Int64, true, 8, qword_bcst),
			(MemorySize.Broadcast256_Float32, true, 4, dword_bcst),
			(MemorySize.Broadcast256_Float64, true, 8, qword_bcst),
			(MemorySize.Broadcast512_UInt32, true, 4, dword_bcst),
			(MemorySize.Broadcast512_Int32, true, 4, dword_bcst),
			(MemorySize.Broadcast512_UInt52, true, 8, qword_bcst),
			(MemorySize.Broadcast512_UInt64, true, 8, qword_bcst),
			(MemorySize.Broadcast512_Int64, true, 8, qword_bcst),
			(MemorySize.Broadcast512_Float32, true, 4, dword_bcst),
			(MemorySize.Broadcast512_Float64, true, 8, qword_bcst),
		};
	}
}
#endif
