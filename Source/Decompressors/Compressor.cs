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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Dom;


namespace PowerUI.Compression{
	
	/// <summary>
	/// An interface for a compression algorithm. Use e.g. Compression.Get("zlib") instead.
	/// </summary>
	
	[Values.Preserve]
	public class Compressor{
		
		/// <summary>The names of the compressor.</summary>
		public virtual string[] GetNames(){
			return null;
		}
		
		protected void SizeRequired(){
			throw new NotSupportedException("This compression algorithm requires a specific size.");
		}
		
		/// <summary>When an algorithm supports it, this figures out the right amount of bytes to decompress.</summary>
		public byte[] Decompress(Stream source){
			
			int size=-1;
			return Decompress(source,null,0,ref size);
			
		}
		
		/// <summary>When an algorithm supports it, this figures out the right amount of bytes to decompress.</summary>
		public byte[] Decompress(Stream source,int byteCount){
			
			int size=byteCount;
			byte[] target=size<0?null : new byte[size];
			
			return Decompress(source,target,0,ref size);
			
		}
		
		/// <summary>When an algorithm supports it, this figures out the right amount of bytes to decompress.</summary>
		public void Decompress(Stream source,byte[] target){
			
			int size=-1;
			Decompress(source,target,0,ref size);
			
		}
		
		/// <summary>When an algorithm supports it, this figures out the right amount of bytes to decompress.</summary>
		public void Decompress(Stream source,byte[] target,int offset,int size){
			
			Decompress(source,target,offset,ref size);
			
		}
		
		/// <summary>Decompresses 'output_size' original bytes from source into target.
		/// Essentially target should be at least 'output_size' in length.</summary>
		public virtual byte[] Decompress(Stream source,byte[] target,int offset,ref int output_size){
			return null;
		}
		
	}
	
}