using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Dom;
using Json;
using PowerUI.Http;


namespace PowerUI{
	
	/// <summary>
	/// Handles disk caching of cookies and other content.
	/// </summary>
	public static class Cache{
		
		/// <summary>The index of domain information.</summary>
		private static Dictionary<string,DomainData> Index;
		
		
		/// <summary>Loads the index data for the given domain.</summary>
		public static DomainData GetDomain(string domain){
			
			// Tidy it:
			domain=domain.Trim().ToLower();
			
			if(Index==null){
				
				Index=new Dictionary<string,DomainData>();
				
			}
			
			DomainData result;
			if(!Index.TryGetValue(domain,out result)){
				
				// Create it (always created so we don't spam PlayerPrefs):
				result=new DomainData(domain);
				
				// WARNING: The following throws threading issues.
				// Often occurs when reloading a document with a http/s URL in it.
				
				// Try getting it from PlayerPrefs:
				string domainData=PlayerPrefs.GetString(domain+"-domain-data");
				result.LoadFromJson(domainData);
				
				// Put into result:
				Index[domain]=result;
				
			}
			
			return result;
		}
		
	}
	
	/// <summary>
	/// Holds e.g. cookies and the index of cached files for a given domain name.
	/// Stored permanently when possible.
	/// </summary>
	public class DomainData{
		
		/// <summary>The domain that this data is for.</summary>
		public string Host;
		/// <summary>Cookies for this domain.</summary>
		public CookieJar Cookies;
		/// <summary>The content of this domain.</summary>
		public CachedContentSet Content;
		/// <summary>True if this data is empty.</summary>
		public bool IsEmpty{
			get{
				return (Cookies==null || Cookies.IsEmpty) && (Content==null || Content.IsEmpty);
			}
		}
		
		
		public DomainData(string domain){
			Host=domain;
		}
		
		/// <summary>Loads the domain data from the given JSON string.</summary>
		public void LoadFromJson(string data){
			
			if(string.IsNullOrEmpty(data)){
				return;
			}
			
			// Load the JSON:
			JSObject json=JSON.Parse(data);
			
			if(json==null){
				return;
			}
			
			// Setup content and cookies:
			JSObject cookies=json["cookies"];
			
			if(cookies!=null){
				
				// Setup:
				Cookies=new CookieJar(this);
				Cookies.LoadFromJson(cookies);
				
			}
			
			// Content:
			JSObject content=json["files"];
			
			if(content!=null){
				
				// Setup:
				Content=new CachedContentSet(this);
				Content.LoadFromJson(content);
				
			}
			
		}
		
		/// <summary>Writes out this domain data to player prefs.</summary>
		public void Save(){
			
			string json=ToJSONString();
			string key=Host+"-domain-data";
			
			if(json==null){
				PlayerPrefs.DeleteKey(key);
			}else{
				// Set it:
				PlayerPrefs.SetString(key,json);
			}
			
		}
		
		/// <summary>Gets this cache info as a JSON string.
		/// Returns null if it's empty.</summary>
		public string ToJSONString(){
			
			// Get JSON:
			JSObject json=ToJson();
			
			if(json==null){
				return null;
			}
			
			// Write it out:
			return json.ToJSONString();
			
		}
		
		/// <summary>Gets this cache info as a JSON object.
		/// Returns null if it's empty.</summary>
		public JSObject ToJson(){
			
			if(IsEmpty){
				return null;
			}
			
			JSObject obj=new JSObject();
			
			obj["host"]=new JSValue(Host);
			
			if(Cookies!=null && !Cookies.IsEmpty){
				obj[Cookies.JsonIndex]=Cookies.ToJson();
			}
			
			if(Content!=null && !Content.IsEmpty){
				obj[Content.JsonIndex]=Content.ToJson();
			}
			
			return obj;
			
		}
		
	}
	
	/// <summary>
	/// Part of domain data such as a cookie jar.
	/// </summary>
	public class DomainEntry{
		
		/// <summary>The domain that this entry is part of.</summary>
		public DomainData Domain;
		
		
		public string Host{
			get{
				return Domain.Host;
			}
		}
		
		public DomainEntry(DomainData domain){
			Domain=domain;
		}
		
		/// <summary>Saves this data now.</summary>
		public void Save(){
			Domain.Save();
		}
		
		/// <summary>The index used in the JSON data.</summary>
		public virtual string JsonIndex{
			get{
				return "";
			}
		}
		
		/// <summary>Converts this domain data into a JSON object. Null if it's empty.</summary>
		public virtual JSObject ToJson(){
			return null;
		}
		
		/// <summary>Loads this data from JSON.</summary>
		public virtual void LoadFromJson(JSObject obj){
		}
		
		/// <summary>True if this entry has no useful information inside it.</summary>
		public virtual bool IsEmpty{
			get{
				return true;
			}
		}
		
	}
	
	/// <summary>
	/// All the cached files in a domain.
	/// </summary>
	public class CachedContentSet : DomainEntry{
		
		/// <summary>The set of entries. Relative URL mapped to the metadata.</summary>
		public Dictionary<string,CachedContent> Entries=new Dictionary<string,CachedContent>();
		
		
		public override bool IsEmpty{
			get{
				return Entries.Count==0;
			}
		}
		
		public CachedContentSet(DomainData data):base(data){}
		
		
		/// <summary>Adds a cached file to the index.</summary>
		public void Add(string url,CachedContent content){
			
			Entries[url]=content;
			
		}
		
		/// <summary>Loads this cached data from the given JSON object.</summary>
		public override void LoadFromJson(Json.JSObject obj){
			
			// It should be an array:
			Json.JSArray arr=obj as Json.JSArray;
			
			if(arr==null){
				return;
			}
			
			// For each one..
			foreach(KeyValuePair<string,JSObject> kvp in arr.Values){
				
				// Add it:
				Add(kvp.Key,new CachedContent(this,kvp.Key,kvp.Value));
				
			}
			
		}
		
		/// <summary>Writes out this cache data as a JSON object.</summary>
		public override Json.JSObject ToJson(){
			
			// Create array:
			Json.JSArray arr=new Json.JSArray();
			
			// Add each one:
			foreach(KeyValuePair<string,CachedContent> kvp in Entries){
				
				// Insert it:
				arr[kvp.Key]=kvp.Value.ToJson();	
				
			}
			
			return arr;
			
		}
		
	}
	
	/// <summary>
	/// A particular cached file.
	/// </summary>
	public class CachedContent{
		
		/// <summary>A counter for this session.</summary>
		public static int Counter=1;
		
		/// <summary>The relative path.</summary>
		public string Path;
		/// <summary>True if an expiry date was set (if in use, we don't bother with a check for a 304).</summary>
		private bool HasExpiry_;
		/// <summary>The etag.</summary>
		private string ETag_;
		/// <summary>The expiry of this entry (UTC datetime).</summary>
		private DateTime Expiry_;
		/// <summary>The time it was saved at.</summary>
		private DateTime SavedAt_;
		/// <summary>Cached path.</summary>
		private string PathToData_;
		
		/// <summary>True if an expiry date was set (if in use, we don't bother with a check for a 304).</summary>
		public bool HasExpiry{
			get{
				return HasExpiry_;
			}
		}
		
		/// <summary>The etag.</summary>
		public string ETag{
			get{
				return ETag_;
			}
			set{
				ETag_=value;
				Changed=true;
			}
		}
		
		/// <summary>The expiry of this entry (UTC datetime).</summary>
		public DateTime Expiry{
			get{
				return Expiry_;
			}
			set{
				Expiry_=value;
				Changed=true;
			}
		}
		
		/// <summary>The time it was saved at.</summary>
		public DateTime SavedAt{
			get{
				return SavedAt_;
			}
			set{
				SavedAt_=value;
				Changed=true;
			}
		}
		
		/// <summary>Cached path.</summary>
		public string PathToData{
			get{
				return PathToData_;
			}
			set{
				PathToData_=value;
				Changed=true;
			}
		}
		
		/// <summary>True if this requires saving.</summary>
		public bool Changed;
		/// <summary>The set this belongs to.</summary>
		public CachedContentSet Set;
		
		
		/// <summary>Has this cookie expired?</summary>
		public bool Expired{
			get{
				if(HasExpiry && Expiry < DateTime.UtcNow){
					return true;
				}
				
				return false;
				
			}
		}
		
		/// <summary>The cached data.</summary>
		public byte[] Data{
			get{
				#if UNITY_WEBPLAYER || UNITY_WEBGL
				return null;
				#else
				return System.IO.File.ReadAllBytes(PathToData);
				#endif
			}
		}
		
		
		public CachedContent(CachedContentSet set,string path){
			SavedAt=DateTime.Now;
			Set=set;
			Path=path;
			Changed=true;
		}
		
		public CachedContent(CachedContentSet set,string path,JSObject obj){
			Set=set;
			Path=path;
			LoadFromJson(obj);
		}
		
		/// <summary>Writes the given data to the file.</summary>
		public void Write(byte[] data){
			
			if(PathToData_==null){
				
				// Allocate a temp path now:
				PathToData=Application.temporaryCachePath+"/"+DateTime.UtcNow.Ticks+"-"+Counter+".bin";
				Counter++;
				
				// Flush the index:
				Save();
				
			}
			
			#if UNITY_WEBPLAYER || UNITY_WEBGL
			// Write out here
			#else
			// Write it out now:
			System.IO.File.WriteAllBytes(PathToData_, data);
			#endif
		}
		
		/// <summary>Writes out this index entry.</summary>
		public void Save(){
			if(Changed){
				Changed=false;
				Set.Save();
			}
		}
		
		/// <summary>Sets an expiry time.</summary>
		public void SetExpiry(string value){
			
			value=value.Replace("-"," ");
			
			if(DateTime.TryParseExact(
				value,
				Cookie.DateTimePattern,
				System.Globalization.CultureInfo.InvariantCulture,
				System.Globalization.DateTimeStyles.None,
				out Expiry_
			)){
				
				HasExpiry_=true;
				
			}else{
				HasExpiry_=false;
				Dom.Log.Add("Warning: Attempted to cache content with a non-standard expiry date '"+value+"'");
			}
			
			Changed=true;
			
		}
		
		/// <summary>Loads this data from a JSON object.</summary>
		public void LoadFromJson(JSObject obj){
			
			ETag_=obj.String("etag");
			PathToData_=obj.String("data");
			
			// Saved at:
			long ticks;
			long.TryParse(obj.String("date"),out ticks);
			SavedAt_=new DateTime(ticks);
			
			// Expiry date:
			string expires=obj.String("expires");
			HasExpiry_=(expires!=null);
			
			if(HasExpiry_){
				long.TryParse(expires,out ticks);
				Expiry_=new DateTime(ticks);
			}
			
		}
		
		/// <summary>Gets this data as a suitable JSON object.</summary>
		public JSObject ToJson(){
			
			JSObject obj=new JSObject();
			obj["etag"]=new JSValue(ETag_);
			
			obj["date"]=new JSLiteral(SavedAt_.ToString());
			
			if(HasExpiry_){
				obj["expires"]=new JSLiteral(Expiry_.Ticks.ToString());
			}
			
			obj["data"]=new JSValue(PathToData_);
			
			return obj;
			
		}
		
	}
	
}