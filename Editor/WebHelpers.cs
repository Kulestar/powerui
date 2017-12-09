using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

/// <summary>
/// A web view in an editor window using a bit of an Asset Store hack.
/// With thanks to WebEditorWindow!
/// </summary>
public static class WebHelpers{
	
	/// <summary>Opens a webview into the given editor window. Null if it fails.</summary>
	public static object Open(EditorWindow window,string url) {
		
		if(!url.Contains("://")){
			
			// Make it a file URL:
			url = "file://"+System.IO.Path.GetFullPath(url);
			
		}
		
		
		// Get the host UI view:
		var thisWindowGuiView = typeof(EditorWindow).GetField("m_Parent", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(window);
		
		// Get a webview type and instance it:
		Type webViewType = GetTypeFromAllAssemblies("WebView");
		
		if(webViewType==null){
			return null;
		}
		
		var webView = ScriptableObject.CreateInstance(webViewType);
		
		int w = (int)window.position.width;
		int h = (int)window.position.height - 22;
		
		// Load the URL now:
		webViewType.GetMethod("InitWebView").Invoke(webView, new object[]{thisWindowGuiView, 0, 22, w, h, false});
		webViewType.GetMethod("LoadURL").Invoke(webView, new object[]{url});
		
		return webView;
	}
	
	/// <summary>Resizes a webview.</summary>
	public static void Resize(object webView,float width,float height){
		
		if(webView==null){
			return;
		}
		
		// Call the resize method:
		var resize=webView.GetType().GetMethod("Resize");
		
		if(resize!=null){
			
			resize.Invoke(webView, new object[]{(int)width, (int)height});
			
		}else{
			
			// SetSizeAndPosition:
			resize=webView.GetType().GetMethod("SetSizeAndPosition");
			
			if(resize!=null){
				
				resize.Invoke(webView, new object[]{0,22,(int)width, (int)height});
				
			}
			
		}
		
	}
	
	/// <summary>Searches all available assemblies for the given type name.</summary>
	private static Type GetTypeFromAllAssemblies(string typeName) {
		
		Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
		foreach(Assembly assembly in assemblies) {
			Type[] types = assembly.GetTypes();
			foreach(Type type in types) {
				if(type.Name.Equals(typeName, StringComparison.CurrentCultureIgnoreCase) || type.Name.Contains('+' + typeName)) //+ check for inline classes
					return type;
			}
		}
		return null;
	}
}


