using System;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shapes {

	public class IMTimer : ImmediateModePanel {
		public static IMTimer Instance { get; private set; }
		[ColorUsage( true, true )]
		public Color color;
		public string timer = "timer";
		public int fontSize = 240;
		public TMP_FontAsset fontAsset;

		public float ElapsedTime { get; private set; }

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Destroy(gameObject);
			}
		}

		private void Update()
		{
			ElapsedTime += Time.deltaTime;
		}

		public void ResetTimer()
		{
			ElapsedTime = 0;
		}

		public override void DrawPanelShapes( Rect rect, ImCanvasContext ctx ) {
			// Draw black background:
			Draw.Rectangle( rect, 8f, color );
			

			// Draw white border:
			Draw.RectangleBorder( rect, 8f, 8f, Color.white );
			

			// Draw the title
			timer = ElapsedTime.ToString("00.00");
			Draw.FontSize = fontSize;
			Vector2 center = new Vector2(0, 0);
			Draw.Font = fontAsset;
			Draw.Text( center, timer, TextAlign.Center );
		}
	}
}