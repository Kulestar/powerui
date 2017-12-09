#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4
	#define PRE_UNITY3_5
#endif

#if PRE_UNITY3_5 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
	#define PRE_UNITY5
#endif


using PowerUI;
using UnityEngine;


namespace PowerUI{

	/// <summary>
	/// This class is a minimal live html manager. Just add it to a gameobject in your scene.
	/// Note that there should only be one PowerUI manager.
	/// Managers are totally optional - you can start PowerUI with just UI.Html="..".
	/// </summary>

	public class ManagerLiveText:MonoBehaviour{
		
		#if !PRE_UNITY5 
			[TextArea(10,10)]
		#endif
		/// <summary>Write your HTML using the inspector.</summary> 
		public string Html="My HTML";
		/// <summary>A copy of HTML used to check if it's changed.</summary>
		private string CachedHtml="";
		
		
		void Update(){
			
			// Changed?
			if(CachedHtml!=Html){
				
				// Yep! Write it out:
				CachedHtml=Html;
				UI.Html=Html;
				
			}
		
		}
		
	}
	
}