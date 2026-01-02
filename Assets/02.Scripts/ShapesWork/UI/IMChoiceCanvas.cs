using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering.Universal;

namespace Shapes {

	public class IMChoiceCanvas : ImmediateModeCanvas {
		public float cornerRadius = 16f;
		public float thickness = 8f;
		[ColorUsage(true, true)]
		public Color roundColor = Color.white;
		
		[ColorUsage(true, true)]
		public Color insideColor = Color.white;

		public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
		public float amount;
		
		private Rect m_Rect;
		
		// this is called automatically by the base class, in an existing Draw.Command context
		public override void DrawCanvasShapes( ImCanvasContext ctx ) {
			// The Rect input above is the full region of the UI,
			// usually the region of the entire screen, in UI coordinates

			m_Rect = Inset(ctx.canvasRect, amount);

			using (Draw.Command(Camera.main, renderPassEvent))
			{
				Draw.ZTest = UnityEngine.Rendering.CompareFunction.Always;
				Draw.Rectangle(ctx.canvasRect, insideColor);
				// Draw a rounded border around the whole screen:
				Draw.RectangleBorder( m_Rect, thickness, cornerRadius, roundColor );
			}
			

			// Draws all ImmediateModePanel child objects.
			// in this case, they are health/stamina/magic bars:
			base.DrawPanels();
		}
		
		Rect Inset( Rect r, float amount ) {
			return new Rect( r.x + amount, r.y + amount, r.width - amount * 2, r.height - amount * 2 );
		}
	}

}