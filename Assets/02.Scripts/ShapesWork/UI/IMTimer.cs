using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shapes {

	public class IMTimer : ImmediateModePanel {
		
		[ColorUsage( true, true )]
		public Color color;
		public string title = "Title";
		public int fontSize = 240;
		public TMP_FontAsset fontAsset;

		public override void DrawPanelShapes( Rect rect, ImCanvasContext ctx ) {
			// Draw black background:
			Draw.Rectangle( rect, 8f, color );
			

			// Draw white border:
			Draw.RectangleBorder( rect, 8f, 8f, Color.white );
			

			// Draw the title
			Draw.FontSize = fontSize;
			Vector2 center = new Vector2(0, 0);
			Draw.Font = fontAsset;
			Draw.Text( center, title, TextAlign.Center );
		}
	}
}