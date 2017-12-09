//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Dom;
using System.IO;
using Zlib;


namespace PowerUI.Compression{
	
	/// <summary>
	/// An interface for the Zlib compression algorithm. Use Compression.Get("zlib") instead (it's a global instance).
	/// </summary>
	
	public class ZlibCompressor : Compressor{
		
		/// <summary>The names of the compressor.</summary>
		public override string[] GetNames(){
			return new string[]{"zlib","deflate"};
		}
		
		/// <summary>Decompresses 'output_size' original bytes from source into target.
		/// Essentially target should be at least 'output_size' in length.</summary>
		public override byte[] Decompress(Stream source,byte[] target,int offset,ref int output_size){
			
			// Need to create our buffer?
			if (target == null) {
				
				if(output_size<0){
					throw new NotSupportedException("Please provide the # of bytes to decompress.");
				}
				
				target=new byte[output_size];
				
			}else if(output_size<0){
				output_size=target.Length;
			}
			
			// Setup a deflate stream:
			DeflateStream deflater=new DeflateStream(source,CompressionMode.Decompress);
			
			// Read into target:
			deflater.Read(target,offset,output_size);
			
			return target;
			
		}
		
	}
	
}