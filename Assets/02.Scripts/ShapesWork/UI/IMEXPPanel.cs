using UnityEngine;

namespace Shapes {

	public class IMEXPPanel : ImmediateModePanel {

		[Range( 0, 1 )]
		public float fillAmount = 1;
		[GradientUsage( true )]
		public Gradient colorGradient;
		public string title = "Title";
		private float m_Percent;

		public void GameStart()
		{
			UIManager.Instance.onPlayerExpChangeEvent += ChangeExpValue;
		}

		        public override void OnDisable()
		        {
		            base.OnDisable();
		            if (UIManager.isApplicationQuitting)
		            {
		                return;
		            }
		            if (!UIManager.HasInstance)
		            {
		                return;
		            }
		            UIManager.Instance.onPlayerExpChangeEvent -= ChangeExpValue;
		        }		public override void DrawPanelShapes( Rect rect, ImCanvasContext ctx ) {
			if( colorGradient == null )
				return; // just in case it hasn't initialized

			// Draw black background:
			Draw.Rectangle( rect, 8f, Color.black );

			// Draw the colored bar:
			Rect fillRect = Inset( rect, 8 ); // inset the rect a little bit, to give it some margin
			fillRect.width *= fillAmount;
			Draw.Rectangle( fillRect, colorGradient.Evaluate( fillAmount ) );

			// Draw white border:
			Draw.RectangleBorder( rect, 4f, 8f, Color.white );
			
			for(int i = 0; i <= 10; i++)
			{
				m_Percent = i * 0.1f;
				Rect smallRect = PercentSize( rect, m_Percent );
				Draw.RectangleBorder( smallRect, 4f, 8f, Color.white );
			}

			// Draw the title
			Draw.FontSize = 240;
			Vector2 topLeft = new Vector2( rect.xMin + 6f, rect.yMax + 6f );
			Draw.Text( topLeft, title, TextAlign.BaselineLeft );
		}

		Rect Inset( Rect r, float amount ) {
			return new Rect( r.x + amount, r.y + amount, r.width - amount * 2, r.height - amount * 2 );
		}

		Rect PercentSize(Rect r, float percent)
		{
			return new Rect(r.x, r.y, r.width * percent, r.height);
		}
		
		private void ChangeExpValue(float ratio)
		{
			fillAmount = ratio;
		}
	}
}