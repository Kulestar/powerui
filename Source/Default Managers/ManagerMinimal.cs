namespace PowerUI{

	/// <summary>
	/// This class is a minimal PowerUI manager.
	/// Managers are totally optional - you can start PowerUI with just UI.Html="..".
	/// </summary>
	
	public class ManagerMinimal:UnityEngine.MonoBehaviour{
		
		public string Html;
		
		void Start(){
			
			// You only need to write to the UI and nothing more:
			UI.Html=Html;
			
		}
		
	}
	
}