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
using Brotli;


namespace PowerUI.Compression{
	
	/// <summary>
	/// An interface for the Brotli compression algorithm. Use Compression.Get("brotli") instead (it's a global instance).
	/// </summary>
	
	public class BrotliCompressor : Compressor{
		
		/// <summary>The names of the compressor.</summary>
		public override string[] GetNames(){
			return new string[]{"brotli","br"};
		}
		
		/// <summary>Decompresses 'output_size' original bytes from source into target.
		/// Essentially target should be at least 'output_size' in length.</summary>
		public override byte[] Decompress(Stream source,byte[] target,int offset,ref int output_size){
			
			// If target or a size is defined then we must not generate any other buffers.
			bool explicitSize=(target!=null || output_size!=-1);
			
			if (target == null) {
				output_size = Brotli.Decoder.BrotliDecompressedSize(source);
				target = new byte[output_size];
			}else if(output_size==-1){
				output_size=target.Length;
			}
			
			OutputStream output=new OutputStream(target,explicitSize);
			output.pos_=offset;
			
			Brotli.Decoder.BrotliDecompress(source, output);

			if (output.pos_ < output_size) {
				
				// Wrote less than expected.
				if(explicitSize){
					output_size=output.pos_;
					return output.buffer;
				}
				
				byte[] ob2=new byte[output.pos_];
				
				System.Array.Copy(output.buffer,0,ob2,0,output.pos_);
				
				return ob2;
			}
			
			output_size=output.pos_;
			return output.buffer;
			
		}
		
	}
	
}