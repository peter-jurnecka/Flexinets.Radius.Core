﻿using System;
using System.Buffers.Binary;

namespace Flexinets.Radius.Core
{
    public class VendorSpecificAttribute
    {
        public byte Length;
        public uint VendorId;
        public byte VendorCode;        
        public byte[] Value;


        /// <summary>
        /// Create a vsa from bytes
        /// </summary>
        /// <param name="contentBytes"></param>
        public VendorSpecificAttribute(byte[] contentBytes)
        {
            //var vendorId = new byte[4];
            //Buffer.BlockCopy(contentBytes, 0, vendorId, 0, 4);
            //Array.Reverse(vendorId);
            //this.VendorId = BitConverter.ToUInt32(vendorId, 0);

            //var vendorType = new byte[1];
            //Buffer.BlockCopy(contentBytes, 4, vendorType, 0, 1);
            //this.VendorCode = vendorType[0]!;

            //var vendorLength = new byte[1];
            //Buffer.BlockCopy(contentBytes, 5, vendorLength, 0, 1);
            //this.Length = vendorLength[0];

            //var value = new byte[contentBytes.Length - 6];
            //Buffer.BlockCopy(contentBytes, 6, value, 0, contentBytes.Length - 6);
            //this.Value = value;


            ReadOnlySpan<byte> contentSpan = contentBytes;

            VendorId = BinaryPrimitives.ReadUInt32BigEndian(contentSpan.Slice(0, 4));

            VendorCode = contentSpan[4];
            Length = contentSpan[5];
            Value = contentSpan.Slice(6).ToArray();
        }
    }
}
