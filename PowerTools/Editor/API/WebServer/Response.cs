using PowerUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Json;


namespace PowerTools{
	
	/// <summary>
	/// A response stream which can be reused.
	/// </summary>
	public class Response{
		
		public StringBuilder Builder=new StringBuilder();
		
		public void Add(string str){
			Builder.Append(str);
		}
		
		public void Add(char c){
			Builder.Append(c);
		}
		
		/// <summary>Outputs a jsonlist for the given set of rows.
		/// Jsonlist is particularly powerful as it supports parsing extremely large files in parallel.</summary>
		public void List<T>(IEnumerable<T> rows,string[] fields){
			List<T>(rows,fields,null);
		}
		
		/// <summary>Outputs a jsonlist for the given set of rows.
		/// Jsonlist is particularly powerful as it supports parsing extremely large files in parallel.</summary>
		public void List<T>(IEnumerable<T> rows,string[] fields,Action<T> rowFunction){
			
			// Get field meta:
			System.Reflection.FieldInfo[] fieldSet=new System.Reflection.FieldInfo[fields.Length];
			
			// For each one..
			for(int i=0;i<fields.Length;i++){
				
				// Get the field:
				fieldSet[i] = typeof(T).GetField(fields[i]);
				
			}
			
			// Fields:
			Builder.Append("{\"fields\":{");
			
			for(int i=0;i<fields.Length;i++){
				
				if(i!=0){
					Builder.Append(',');
				}
				
				Builder.Append("\""+fields[i]+"\":"+i);
				
			}
			
			// Field names:
			Builder.Append("},\"fieldNames\":[");
			
			for(int i=0;i<fields.Length;i++){
				
				if(i!=0){
					Builder.Append(",");
				}
				
				Builder.Append("\""+fields[i]+"\"");
				
			}
			
			// Rows header:
			Builder.Append("],\"rows\":[[");
			
			int size=Builder.Length;
			
			// Start outputting the rows now!
			List<int> checkpoints=new List<int>();
			bool first=true;
			
			foreach(T entity in rows){
				
				if(first){
					first=false;
					checkpoints.Add(size-1);
					size=1;
				}else{
					
					size++;
					
					if(size>1000000){
						// 1MB. Add a checkpoint and reset size.
						checkpoints.Add(size+1);
						size=1;
						Builder.Append("],[");
					}else{
						Builder.Append(',');
					}
					
				}
				
				// Call the row method if there is one:
				if(rowFunction!=null){
					rowFunction(entity);
				}
				
				int sizeTrack=Builder.Length;
				
				Builder.Append('[');
				
				for(int i=0;i<fieldSet.Length;i++){
					
					if(i!=0){
						Builder.Append(',');
					}
					
					// Read the value:
					object value=fieldSet[i].GetValue(entity);
					
					if(value==null){
						Builder.Append("null");
					}else if(value is string){
						Builder.Append("\""+(value as string).Replace("\"","\\\"")+"\"");
					}else{
						Builder.Append(value.ToString());
					}
					
				}
				
				Builder.Append(']');
				
				// Increase by row size:
				size+=Builder.Length-sizeTrack;
				
			}
			
			// This has a length of 18, but we use 17 as we want the first bracket to be included in the referenced value.
			Builder.Append("]],\"checkpoints\":[");
			
			// We want to know where the checkpoints start at for our 'cp' location value.
			int checkpointsAt=size+17;
			int total=0;
			
			// for each checkpoint..
			for(int i=0;i<checkpoints.Count;i++){
				
				if(i!=0){
					Builder.Append(',');
				}
				
				// Add it to the total:
				total+=checkpoints[i];
				
				Builder.Append(total.ToString());
				
			}
			
			Builder.Append("],\"locations\":{\"f\":10,\"cp\":"+checkpointsAt+"}}");
			
		}
			
		public void Add(JSObject json){
			Builder.Append(json.ToJSONString());
		}
		
		public void Append(string str){
			Builder.Append(str);
		}
		
		public void Append(char c){
			Builder.Append(c);
		}
		
		public void Append(JSObject json){
			Builder.Append(json.ToJSONString());
		}
		
		public void Echo(string str){
			Builder.Append(str);
		}
		
		public void Echo(char c){
			Builder.Append(c);
		}
		
		public void Echo(JSObject json){
			Builder.Append(json.ToJSONString());
		}
		
		public void Write(string str){
			Builder.Append(str);
		}
		
		public void Write(char c){
			Builder.Append(c);
		}
		
		public void Write(JSObject json){
			Builder.Append(json.ToJSONString());
		}
		
		public void Clear(){
			Builder.Length=0;
		}
		
		public override string ToString(){
			return Builder.ToString();
		}
		
	}
	
}