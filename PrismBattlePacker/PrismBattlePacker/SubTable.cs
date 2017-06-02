﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismBattlePacker
{
	public static class SubTable
	{
		public static byte[] FlippedTable;
		public static byte[] Table =
		{
			0x52,0x5c,0x9c,0x13,0xea,0x69,0x2d,0xf3,0xef,0x85,0x44,0x2e,0x2c,
			0x58,0x5f,0x05,0x42,0xba,0x8a,0x86,0x28,0x76,0xc7,0xe8,0xaa,0x78,
			0x54,0xf9,0xde,0x7d,0x62,0x04,0xfa,0x35,0xdc,0x10,0x7c,0xa4,0xc2,
			0x7e,0xe0,0xc8,0x7b,0x8b,0x2b,0x20,0x1c,0x53,0xce,0x5e,0x29,0x21,
			0xf7,0x61,0xb5,0xa3,0xd6,0x0c,0xa7,0x7a,0x31,0xb9,0x84,0x19,0xf4,
			0x93,0x87,0xa5,0x49,0x8d,0x74,0x64,0x9b,0x47,0x0d,0x5d,0x1d,0x12,
			0x3c,0x3a,0xab,0x0e,0x41,0x09,0xd9,0x73,0x68,0xb0,0xb3,0x0a,0xc6,
			0x06,0xe1,0xb2,0xa9,0x96,0x99,0xd4,0xe2,0x9f,0x4d,0x83,0x8f,0x2f,
			0x6c,0x9e,0x5b,0x17,0xbb,0xd7,0x92,0xff,0x6f,0x39,0xa6,0x1e,0xfd,
			0xcb,0x0f,0x57,0x51,0xfe,0x5a,0x23,0x34,0x98,0xbf,0x4e,0xf8,0xc5,
			0x38,0xfb,0x4f,0xc0,0x1b,0xf6,0xc4,0x65,0x81,0x33,0x72,0x3b,0xdf,
			0x27,0xe5,0x6e,0xcf,0xe4,0xfc,0x07,0xdd,0x82,0xeb,0xee,0x4a,0x55,
			0xe3,0x88,0x4b,0xf5,0x01,0xd8,0xae,0x0b,0x14,0x75,0xc3,0xe9,0x4c,
			0xb7,0x36,0xbc,0xa2,0x79,0x89,0x80,0x6d,0x16,0xa1,0xf1,0xd0,0x60,
			0x66,0x30,0xb1,0xf0,0x1a,0x3d,0xa0,0xed,0x97,0xb4,0xcd,0x90,0xca,
			0x40,0x67,0x1f,0x9d,0x2a,0x91,0x08,0xdb,0xaf,0x02,0xec,0x22,0xd5,
			0xf2,0xda,0x50,0xd2,0xd1,0x24,0x59,0x71,0xbd,0x56,0x45,0xb6,0x46,
			0xac,0xb8,0x95,0x00,0x32,0xc9,0x48,0xcc,0xd3,0x8e,0xbe,0x9a,0x94,
			0x77,0x11,0x18,0x15,0xc1,0x26,0x70,0x8c,0xe7,0xa8,0x6a,0x3e,0x03,
			0xad,0x37,0xe6,0x7f,0x63,0x43,0x3f,0x6b,0x25,0x86,0x70,0xd8,0x3b,
			0xb6,0x05,0xd2,0xe5,0x25,0x37,0xb0,0x60,0x4c,0x4f,0x76,0xd5,0x50,
			0xd7,0x64,0x9d,0xf6,0x58,0x7c,0x22,0xc0,0xae,0x95,0x73,0x38,0xcb,
			0x82,0x2f,0xb9,0xfa,0xbf,0x3e,0x1c,0x51,0x6e,0x28,0x4b,0x7e,0x3d,
			0x8f,0xf7,0xa2,0x68,0x1f,0x09,0x79,0xba,0x72,0xc3,0x4e,0xd4,0xa0,
			0x1b,0x5c,0x53,0x9c,0xfd,0xf5,0x78,0xc2,0x2e,0x27,0x9f,0x91,0xe2,
			0xf4,0x9a,0x36,0x94,0x59,0xfb,0xfc,0xd3,0xa8,0x04,0xcc,0x57,0xf2,
			0x06,0x43,0x5b,0xf8,0x0a,0x96,0xbc,0xbb,0x8a,0xdc,0x5a,0xeb,0x47,
			0xec,0xc1,0xe6,0xb5,0x02,0xf3,0x5f,0xc9,0xa6,0x2b,0xdf,0xb7,0x7f,
			0x2a,0xa1,0xe1,0xed,0x7d,0x26,0xdb,0x54,0xde,0xab,0xf9,0x0c,0xcd,
			0x97,0x24,0x99,0xb8,0x1a,0x20,0xef,0x33,0xe3,0x80,0xb1,0x00,0x8b,
			0x89,0x07,0x29,0x0d,0x45,0xc5,0xe9,0xc4,0xbe,0x5d,0x16,0xcf,0xe7,
			0x67,0x83,0x6a,0x4a,0x01,0x98,0x21,0xa7,0xff,0x74,0xc6,0x7a,0x61,
			0xce,0xac,0x77,0x87,0xd9,0x11,0x39,0x69,0x6f,0x40,0x14,0x63,0x10,
			0x17,0x84,0x0b,0x2d,0x93,0xd0,0x65,0x35,0x49,0xaf,0x48,0xdd,0xa3,
			0xc7,0x2c,0x30,0x9b,0xf1,0x55,0x71,0x19,0xa4,0x75,0xea,0x3f,0x90,
			0x9e,0x1d,0x8e,0x62,0x08,0x41,0x46,0x88,0x32,0xe0,0xe8,0xda,0xaa,
			0xc8,0x31,0x15,0x12,0xbd,0x8d,0xf0,0xb2,0x92,0x7b,0x85,0x6b,0xb3,
			0xa5,0x52,0x03,0x4d,0x6d,0x18,0xd6,0x8c,0x56,0x0e,0x6c,0xad,0x66,
			0xa9,0xfe,0x23,0xb4,0x81,0x1e,0x3c,0xe4,0x44,0x5e,0x3a,0xd1,0x42,
			0x34,0xee,0x13,0x0f,0xca,0x01,0x74,0x0e,0xc2,0x81,0x6f,0x23,0x9c,
			0xda,0x5d,0x1c,0x80,0xb9,0xc4,0x4c,0x52,0xbc,0x42,0xd4,0xc6,0x9d,
			0x15,0xaf,0x8e,0xea,0x5b,0xdb,0x7f,0x7b,0x36,0x87,0x83,0xf3,0x0a,
			0x3b,0xe1,0x46,0xbf,0x06,0xd6,0x60,0x2f,0xc9,0x72,0x6a,0x2c,0x90,
			0x34,0xbe,0xa3,0xb6,0x28,0x16,0xcb,0xfd,0x89,0xa7,0xce,0x6c,0xe8,
			0xac,0x08,0x8c,0x5a,0xe2,0x09,0x26,0xfe,0xbb,0x02,0x62,0xd7,0xb0,
			0xb1,0x93,0x20,0x58,0x8f,0x55,0x2d,0xbd,0x25,0x57,0x7e,0xf5,0x27,
			0xc5,0x8b,0xd0,0x6d,0x31,0x30,0x48,0x85,0x19,0x5f,0x97,0xaa,0x1e,
			0x49,0x17,0xdd,0x6b,0x1d,0xa5,0xeb,0x64,0x33,0xcf,0xc1,0x5c,0xe6,
			0xb4,0xfa,0x7c,0x0f,0xdc,0x61,0x67,0xcd,0x70,0x4f,0x1a,0x91,0xc3,
			0x38,0x56,0x84,0xde,0x63,0x03,0x29,0x7d,0x43,0x71,0xf9,0x1f,0xa4,
			0x35,0xba,0xb3,0xe4,0x51,0xad,0x05,0xd8,0xa9,0xe9,0x4b,0x3f,0x77,
			0x96,0x1b,0x95,0xa2,0x66,0xcc,0xc0,0xee,0x37,0xe0,0x76,0xec,0xf6,
			0x22,0x47,0x0d,0xa0,0xd1,0x13,0xef,0x24,0xfc,0x54,0xd5,0xe5,0x69,
			0x9a,0xe7,0xd2,0xa1,0x12,0x9f,0x86,0x79,0x2e,0x88,0x0b,0x68,0xa6,
			0x9b,0x6e,0xb5,0xd9,0xa8,0xff,0x9e,0x50,0xfb,0x4a,0xca,0x2b,0xf8,
			0xf4,0x3c,0x32,0x4e,0xc7,0xb8,0x3d,0x0c,0x53,0xed,0x92,0x2a,0xb7,
			0x04,0x94,0xe3,0x75,0x14,0x82,0x45,0xf2,0x98,0x10,0xae,0xdf,0x73,
			0xc8,0xf7,0xd3,0x11,0x7a,0x4d,0x07,0x18,0x78,0x3e,0x3a,0x00,0x99,
			0x59,0x5e,0x41,0xf1,0xab,0x8a,0xb2,0xf0,0x65,0x39,0x21,0x8d,0x44,
			0x40,0x3c,0x93,0x62,0x2d,0x14,0xce,0xd5,0xe2,0xb3,0x39,0x55,0x6a,
			0x77,0x05,0x7e,0x70,0xf4,0x98,0x76,0x1a,0xa2,0xa7,0xb9,0xf7,0x40,
			0xbd,0xcb,0x84,0x82,0x61,0xe3,0x6d,0xf3,0x6e,0x27,0x87,0x3b,0xe8,
			0xe9,0xea,0x6b,0xef,0x16,0xaa,0xe5,0xab,0x78,0xa6,0x10,0x60,0x6c,
			0x9d,0x2e,0x7d,0x7a,0x0a,0x34,0x0f,0x32,0xcf,0xeb,0x9c,0xb4,0x35,
			0x64,0x74,0x0b,0xcd,0xb0,0xc1,0x58,0xfd,0xbf,0xc0,0x21,0x4b,0xd8,
			0xff,0xe7,0xb6,0xc4,0x94,0x95,0xd7,0x15,0x00,0xa5,0x28,0x63,0x2b,
			0xf6,0xbc,0x42,0x9b,0x69,0x5e,0x3e,0xae,0xcc,0xc3,0xe1,0x48,0x83,
			0xaf,0x3f,0x0e,0x8a,0x50,0x6f,0x7c,0x56,0x24,0xb2,0x02,0x5b,0xf9,
			0x08,0xb7,0x65,0x12,0x04,0x81,0xb8,0xdc,0xd1,0xee,0xbb,0x47,0x4a,
			0x54,0xf5,0x8f,0x06,0x18,0x7f,0x73,0xd3,0x36,0xb1,0x2f,0x1b,0xa1,
			0x8d,0xdf,0xde,0x9e,0x51,0x5a,0xc9,0x59,0xd6,0xfb,0x1e,0x8c,0xca,
			0x4c,0xec,0xc7,0x68,0x23,0x88,0x29,0x90,0xf2,0x09,0x2c,0x45,0x25,
			0x13,0xa3,0x79,0xda,0x3d,0x96,0xdd,0x20,0x1d,0x89,0x9a,0xa4,0x01,
			0xd0,0x1f,0xa8,0xed,0x5d,0x67,0x49,0x75,0x46,0x8b,0x9f,0x91,0x37,
			0x03,0xc5,0xd2,0x53,0x0c,0x5f,0x44,0x33,0xf0,0xe6,0x86,0x4e,0xdb,
			0x71,0x31,0x57,0x43,0x38,0xc8,0x72,0xad,0xa9,0x11,0x19,0x7b,0x8e,
			0xb5,0x1c,0x22,0x26,0x80,0xc2,0x4d,0xd9,0xf1,0x97,0x66,0xf8,0x30,
			0x2a,0xfc,0xfe,0xba,0x07,0x41,0x5c,0x92,0x4f,0xbe,0xfa,0x85,0x52,
			0xe0,0x3a,0xac,0xe4,0xa0,0xd4,0x99,0x17,0xc6,0x0d,0xe8,0x7d,0x06,
			0x40,0x53,0x7e,0xba,0x0b,0x52,0xaa,0x71,0x88,0x30,0x21,0x50,0x75,
			0xc6,0x38,0xef,0xd3,0x24,0x33,0xed,0x0d,0x9e,0xb7,0x94,0x6f,0x5d,
			0xd1,0x92,0x3e,0xa9,0xc0,0xac,0x96,0xfd,0xa8,0x4a,0x1b,0xb6,0x97,
			0x73,0x5a,0x9d,0x2e,0x74,0xa0,0x84,0x6e,0x3d,0x4e,0x5b,0x9a,0xbc,
			0xfe,0xf8,0x9f,0x86,0x2b,0xf6,0x6d,0xe3,0xbd,0x10,0xc9,0x0f,0xe7,
			0xd2,0x91,0x20,0x6c,0xa2,0xd8,0xaf,0x0e,0x5c,0xfb,0x7b,0x34,0x4d,
			0x11,0x31,0x85,0x77,0x90,0xf9,0xe0,0xcf,0x8a,0xea,0x25,0x1f,0x09,
			0x2c,0x60,0xff,0x12,0xe4,0x8f,0xf1,0x17,0xc7,0xb1,0xa5,0xc1,0x67,
			0x2d,0x49,0xf3,0x8e,0xfc,0x57,0x7a,0xc2,0x2f,0xda,0x04,0xdf,0x18,
			0x80,0x63,0x68,0x9c,0xd0,0x89,0x19,0xa6,0x64,0x1c,0xd9,0xd5,0xe9,
			0x1a,0xec,0x56,0xa3,0xc5,0x98,0xde,0xb8,0x3a,0xa1,0xf5,0xf4,0x16,
			0x54,0x3f,0xd7,0x95,0x79,0x46,0x36,0x2a,0x47,0x5e,0x42,0x6b,0x82,
			0xb0,0xee,0x70,0xab,0xb2,0xeb,0xcb,0x48,0x51,0xd6,0xae,0x8c,0x22,
			0x72,0x4b,0xa4,0x02,0xdd,0x78,0xa7,0xb9,0x66,0x8d,0xfa,0x8b,0xf2,
			0x1e,0xdb,0x7c,0x43,0x62,0xcd,0x41,0xdc,0x55,0xe6,0x69,0x08,0x03,
			0xe1,0xbb,0x39,0x99,0x32,0x45,0x61,0x87,0xf7,0x01,0x6a,0xb4,0x3c,
			0xad,0xb3,0xe2,0x4c,0x35,0x05,0x0c,0x83,0xd4,0x93,0x4f,0x00,0x28,
			0x81,0x37,0xce,0xca,0x29,0xbf,0x5f,0x14,0x58,0x27,0x59,0xe5,0xf0,
			0xcc,0x7f,0x3b,0x13,0x76,0x65,0x44,0x0a,0xb5,0xc3,0x23,0x1d,0xc8,
			0xc4,0xbe,0x9b,0x07,0x15,0x26,0x52,0x91,0x32,0x1f,0x5a,0x2e,0xb3,
			0xc6,0x84,0xdc,0x83,0xa9,0x31,0x3e,0xb5,0x85,0x54,0x9e,0x23,0xe0,
			0x14,0xf3,0x44,0x37,0xf9,0xb8,0x6b,0x4f,0x99,0x9f,0xa0,0x17,0x86,
			0xed,0xd2,0x63,0x58,0xc2,0x30,0x57,0xca,0xfe,0x8d,0x11,0x22,0xeb,
			0x00,0x0b,0xa6,0x1c,0x05,0x82,0x2b,0xbd,0x68,0xe3,0x8c,0xf4,0xbe,
			0x29,0xf2,0xab,0x5e,0x74,0x0e,0xaf,0x40,0x0d,0xbc,0xf1,0x24,0xe4,
			0x2d,0xd7,0x73,0x47,0x69,0xa8,0x18,0xb9,0x8a,0xe9,0xfc,0xcf,0xcc,
			0x89,0x3a,0x35,0x59,0x33,0x50,0xc0,0x98,0xce,0xcd,0x28,0x92,0xa7,
			0x55,0xa5,0x5f,0xb4,0xe6,0xd6,0xe1,0x8b,0x36,0xc9,0xd0,0x41,0x5d,
			0x90,0x4e,0xfd,0x78,0x80,0x6e,0xd5,0x67,0x3f,0xf5,0x1a,0xb7,0x13,
			0x43,0x7f,0x75,0x8e,0xcb,0x7e,0xf6,0x95,0xd3,0x4d,0xec,0xfa,0xda,
			0x01,0xee,0x38,0x65,0xb1,0x27,0xac,0x45,0x3d,0x19,0xe5,0xa2,0xdf,
			0xe2,0x7c,0xd4,0x87,0xdb,0x6a,0xff,0x0c,0xa1,0x70,0x0a,0x7b,0x64,
			0x2a,0x34,0x20,0x94,0x60,0x3b,0x88,0xef,0x77,0xf0,0x25,0xd1,0x56,
			0x72,0x21,0xb6,0x39,0xb2,0x46,0x12,0xc5,0xc8,0xf7,0x1d,0x0f,0x96,
			0xc4,0x79,0x48,0xf8,0x6d,0x9c,0x1b,0x2f,0xa4,0x53,0x03,0x61,0x10,
			0xb0,0xe8,0x06,0x8f,0x15,0xa3,0xba,0xc7,0x51,0x62,0x49,0xe7,0x26,
			0xc1,0xbf,0x08,0x16,0xae,0xdd,0x42,0x9b,0x4a,0x3c,0xaa,0xd8,0x09,
			0x93,0xbb,0x02,0x9a,0x4b,0x4c,0x5c,0x7d,0xad,0xfb,0xd9,0x9d,0x81,
			0xea,0x71,0x76,0x07,0x97,0xde,0x04,0x2c,0xc3,0x5b,0x66,0x6c,0x7a,
			0x1e,0x6f,0x6f
		};

		public static void FlipTable()
		{
			FlippedTable = new byte[6 * 256];

			for (var i = 0; i < 6; i++)
			{
				for (var j = 0; j < 256; j++)
				{
					var target = i * 256 + j;
					var val = Table[target];
					FlippedTable[i * 256 + val] = (byte)j;
				}
			}
		}
	}
}
