using System;
using LivingCanvas;
using static System.Console;
using static System.Math;
using static LivingCanvas.Globals;

namespace Daevolution {
	class Program {
		static void Main(string[] args) {
			var cvs = new Canvas((1280, 720));
			cvs.Frame += (sender, time) => {
				cvs.Clear(vec3(0.15));
				cvs.StrokeWidth(1);
				var tstep = 0.0001;
				for(var i = 0; i < 20000; ++i) {
					var t = time - tstep * i;
					cvs.Stroke(vec4(abs(sin(t * 3)), 1, abs(sin(t)), 0.125 + (sin(t) + 1) * .125));
					cvs.Circle(640 + sin(t * 3) * 350 + cos(t * 1.5 + sin(t * 7) * cos(time / 3.5) * 5) * 50, 360 + sin(t * 5 + sin(time / 7.9) * 5) * 250, 40 + sin(t * 4) * cos(t * 3.7) * 30);
				}
			};
			cvs.Click += (sender, a) => {
				var (time, x, y) = a;
				WriteLine($"Click ({x}, {y}) at {time}");
			};
			cvs.Run();
		}
	}
}