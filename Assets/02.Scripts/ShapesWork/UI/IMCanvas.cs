using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering.Universal;

namespace Shapes {

	public class IMCanvas : ImmediateModeCanvas {
		public float cornerRadius = 16f;
		public float thickness = 8f;
		[ColorUsage(true, true)]
		public Color roundColor = Color.white;
		
		// this is called automatically by the base class, in an existing Draw.Command context
		public override void DrawCanvasShapes( ImCanvasContext ctx ) {
			// The Rect input above is the full region of the UI,
			// usually the region of the entire screen, in UI coordinates
			
			// Draw a rounded border around the whole screen:
			Draw.RectangleBorder( ctx.canvasRect, thickness, cornerRadius, roundColor );
			// Draws all ImmediateModePanel child objects.
			// in this case, they are health/stamina/magic bars:
			base.DrawPanels();
		}
	}

}