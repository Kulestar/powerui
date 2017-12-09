using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Diagnostics;
using System.IO;


namespace PowerUI {
	
	/// <summary>
	/// Binding for Node.js which ships with Unity.
	/// </summary>
	public class NodeJS : Dom.EventTarget {
		
		/// <summary>Relative path to Node (from Unity).</summary>
		public readonly static string RelativePath = "Data/Tools/nodejs/";
		/// <summary>Relative path to npm (from Unity).</summary>
		public readonly static string RelativeNodePath = "node";
		/// <summary>Relative path to npm (from Unity).</summary>
		public readonly static string RelativeNpmPath = "npm.cmd";
		
		public readonly static string DefaultPackageJson = @"{
			""scripts"": {
			  ""gulp"": ""gulp""
			}
		}";
		
		/// <summary>Discovered Node basepath.</summary>
		private static string _NodePath;
		private static string _WorkingDirectory;
		
		/// <summary>Searches the Unity installation directory to find Node.</summary>
		public static string NodeBasePath{
			get{
				if(_NodePath == null){
					// The path to Unity:
					var unityPath = EditorApplication.applicationPath;
					
					// Get the dir:
					var unityBasePath = Path.GetDirectoryName(unityPath).Replace("\\", "/");
					
					string nodePath;
					
					if(unityBasePath.EndsWith("/")){
						nodePath = unityBasePath + RelativePath;
					}else{
						nodePath = unityBasePath + "/" + RelativePath;
					}
					
					_NodePath = nodePath;
				}
				return _NodePath;
			}
		}
		
		/// <summary>Searches the Unity installation directory to find node.</summary>
		public static string NodePath{
			get{
				return NodeBasePath + RelativeNodePath;
			}
		}
		
		/// <summary>Searches the Unity installation directory to find Npm.</summary>
		public static string NpmPath{
			get{
				return NodeBasePath + RelativeNpmPath;
			}
		}
		
		public static string WorkingDirectory{
			get{
				if(_WorkingDirectory == null){
					_WorkingDirectory = Directory.GetCurrentDirectory() + "/NodeJS/";
					
					if(!Directory.Exists(_WorkingDirectory)){
						Directory.CreateDirectory(_WorkingDirectory + "Source");
						File.WriteAllText(_WorkingDirectory + "package.json", DefaultPackageJson);
					}
				}
				return _WorkingDirectory;
			}
		}
		
		public static string NodeModulesDirectory{
			get{
				return WorkingDirectory + "node_modules/";
			}
		}
		
		public NodeJS(){}
		
		public Process execute(string jsFile){
			if(jsFile.Contains(" ")){
				jsFile = "\"" + jsFile+"\"";
			}
			return start(jsFile, NodePath);
		}
		
		public bool exists(string package){
			return Directory.Exists(NodeModulesDirectory+package);
		}
		
		public Process run(string package, string args){
			return start("run " + package+" -- "+ args, NpmPath);
		}
		
		public Process install(string packageAndArgs){
			return start("install "+packageAndArgs, NpmPath);
		}
		
		private Process start(string args, string exe){
			
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.FileName = exe;
			startInfo.CreateNoWindow = true;
			startInfo.UseShellExecute = false;
			startInfo.RedirectStandardOutput = true;
			// node_modules must be outside Assets directory:
			startInfo.WorkingDirectory = WorkingDirectory;
			startInfo.Arguments = args;
			
			Process process = Process.Start(startInfo);
			process.EnableRaisingEvents = true;
			process.Exited += delegate(object sender, System.EventArgs evtArgs){
				string output = process.StandardOutput.ReadToEnd();
				var e = new NodeEvent("exit");
				e.stdOutput = output;
				dispatchEvent(e);
			};
			return process;
		}
		
	}
	
	
	
	public class NodeEvent : Dom.Event{
		
		public string stdOutput;
		
		
		public NodeEvent(string type):base(type){}
		
	}
	
}