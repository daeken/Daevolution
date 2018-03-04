using System;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SkiaSharp;
using static LivingCanvas.Globals;

namespace LivingCanvas {
	internal class LCWindow : GameWindow {
		internal LCWindow((int, int) dimensions) : base(
			dimensions.Item1, dimensions.Item2, GraphicsMode.Default, "LivingCanvas", 
			GameWindowFlags.Default, DisplayDevice.Default, 1, 0, GraphicsContextFlags.Default
		) {}

		new GRContext Context;
		GRBackendRenderTargetDesc RenderTarget;

		internal new event EventHandler<SKCanvas> RenderFrame;

		internal Vec2 CoordOffset;
		internal float CoordMultiplier;

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			var glInterface = GRGlInterface.CreateNativeGlInterface();
			Context = GRContext.Create(GRBackend.OpenGL, glInterface);

			GL.GetInteger(GetPName.FramebufferBinding, out var fb);
			GL.GetInteger(GetPName.StencilBits, out var stencilBits);
			GL.GetInteger(GetPName.Samples, out var samples);
			
			GL.GetRenderbufferParameter(RenderbufferTarget.Renderbuffer, RenderbufferParameterName.RenderbufferWidth, out var bw);
			GL.GetRenderbufferParameter(RenderbufferTarget.Renderbuffer, RenderbufferParameterName.RenderbufferHeight, out var bh);

			RenderTarget = new GRBackendRenderTargetDesc {
				Width = bw, 
				Height = bh, 
				Config = GRPixelConfig.Bgra8888, 
				Origin = GRSurfaceOrigin.TopLeft, 
				SampleCount = samples, 
				StencilBits = stencilBits, 
				RenderTargetHandle = (IntPtr) fb
			};
			
			OnResize(null);
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			RenderTarget.Width = Width;
			RenderTarget.Height = Height;
			using(var surface = SKSurface.Create(Context, RenderTarget)) {
				var canvas = surface.Canvas;
				canvas.Flush();
				RenderFrame?.Invoke(this, canvas);
				canvas.Flush();
			}
			Context.Flush();
			SwapBuffers();
		}

		protected override void OnResize(EventArgs e) {
			if(e != null)
				base.OnResize(e);

			var ratio = (double) Width / Height;
			var targetRatio = 1280.0 / 720;
			if(Math.Abs(ratio - targetRatio) < 0.01) {
				CoordOffset = vec2();
				CoordMultiplier = (float) (Width / 1280.0);
			} else if(ratio > targetRatio) { // Wider
				var hsub = Height * targetRatio;
				CoordOffset = vec2((Width - hsub) / 2, 0);
				CoordMultiplier = (float) (hsub / 1280.0);
			} else if(ratio < targetRatio) { // Taller
				var vsub = Width / targetRatio;
				CoordOffset = vec2(0, (Height - vsub) / 2);
				CoordMultiplier = (float) (vsub / 720.0);
			}
		}
	}
	
	public class Canvas {
		public event EventHandler<float> Frame;
		public event EventHandler<(float Time, float X, float Y)> Click;

		readonly LCWindow Win;

		SKCanvas Ctx;
		SKColor? FillColor, StrokeColor;
		int StrokeThickness;
		SKPaint FillPaint, StrokePaint;

		void UpdatePaint(Action cb) {
			cb();
			FillPaint = FillColor != null ? new SKPaint { IsAntialias = true, Color = FillColor.Value, IsStroke = false } : null;
			StrokePaint = StrokeColor != null && StrokeThickness > 0 ? new SKPaint { IsAntialias = true, Color = StrokeColor.Value, IsStroke = true, StrokeWidth = StrokeThickness } : null;
		}
		
		public Canvas((int, int)? dimensions = null) {
			var sw = Stopwatch.StartNew();
			float CurTime() => sw.ElapsedMilliseconds / 1000f;
			
			Win = new LCWindow(dimensions ?? (800, 600));
			Win.RenderFrame += (_, canvas) => {
				Ctx = canvas;
				canvas.Clear(SKColors.GreenYellow);
				Frame?.Invoke(this, CurTime());
			};
			Win.MouseDown += (_, e) => Click?.Invoke(this, (CurTime(), e.X, e.Y));
		}

		public void Run() {
			Win.Run(60);
		}

		public void Clear(SKColor color) => Ctx.Clear(color);
		public void Fill(SKColor color) => UpdatePaint(() => FillColor = color);
		public void Stroke(SKColor color) => UpdatePaint(() => StrokeColor = color);
		public void NoFill() => UpdatePaint(() => FillColor = null);
		public void NoStroke() => UpdatePaint(() => StrokeColor = null);
		public void StrokeWidth(float width) => UpdatePaint(() => StrokeThickness = (int) width);

		Vec2 ReCoord(Vec2 coord) => coord * Win.CoordMultiplier + Win.CoordOffset;
		float ReSize(float size) => size * Win.CoordMultiplier;
		Vec2 ReSize(Vec2 size) => size * Win.CoordMultiplier;

		public void Rectangle(Vec2 p, Vec2 s) {
			p = ReCoord(p);
			s = ReSize(s);
			if(FillPaint != null)
				Ctx.DrawRect(p.X, p.Y, s.X, s.Y, FillPaint);
			if(StrokePaint != null)
				Ctx.DrawRect(p.X, p.Y, s.X, s.Y, StrokePaint);
		}
		public void Rectangle(float ax, float ay, float sx, float sy) => Rectangle(vec2(ax, ay), vec2(sx, sy));
		public void Rectangle(double ax, double ay, double sx, double sy) => Rectangle(vec2(ax, ay), vec2(sx, sy));

		public void Circle(Vec2 p, float r) {
			p = ReCoord(p);
			r = ReSize(r);
			if(FillPaint != null)
				Ctx.DrawCircle(p.X, p.Y, r, FillPaint);
			if(StrokePaint != null)
				Ctx.DrawCircle(p.X, p.Y, r, StrokePaint);
		}
		public void Circle(float ax, float ay, float r) => Circle(vec2(ax, ay), r);
	}
}