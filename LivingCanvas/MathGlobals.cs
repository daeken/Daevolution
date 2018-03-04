using System;
using static System.Math;

namespace LivingCanvas {
	public partial class Globals {
		public static Vec2 vec2() => new Vec2();
		public static Vec2 vec2(float v) => new Vec2(v);
		public static Vec2 vec2(double v) => new Vec2((float) v);
		public static Vec2 vec2(float x, float y) => new Vec2(x, y);
		public static Vec2 vec2(double x, double y) => new Vec2(x, y);

		public static Vec3 vec3() => new Vec3();
		public static Vec3 vec3(float v) => new Vec3(v);
		public static Vec3 vec3(double v) => new Vec3((float) v);
		public static Vec3 vec3(float x, float y, float z) => new Vec3(x, y, z);
		public static Vec3 vec3(double x, double y, double z) => new Vec3(x, y, z);

		public static Vec4 vec4() => new Vec4();
		public static Vec4 vec4(float v) => new Vec4(v);
		public static Vec4 vec4(double v) => new Vec4((float) v);
		public static Vec4 vec4(float x, float y, float z, float w) => new Vec4(x, y, z, w);
		public static Vec4 vec4(double x, double y, double z, double w) => new Vec4(x, y, z, w);

		public static float clamp(float x, float min, float max) => Min(Max(x, min), max);
		public static float fract(float x) => (float) (x - Math.Floor(x));

		public static Vec3 floor(Vec3 x) => vec3(Math.Floor(x.X), Math.Floor(x.Y), Math.Floor(x.Z));

		public static Vec3 min(Vec3 a, Vec3 b) => vec3(Min(a.X, b.X), Min(a.Y, b.Y), Min(a.Z, b.Z));
		public static Vec3 max(Vec3 a, Vec3 b) => vec3(Max(a.X, b.X), Max(a.Y, b.Y), Max(a.Z, b.Z));

		public static float lerp(float a, float b, float x) => (b - a) * x + a;
		public static Vec3 lerp(Vec3 a, Vec3 b, float x) => (b - a) * x + a;

		public static float sin(float v) => (float) Sin(v);
		public static float cos(float v) => (float) Cos(v);
		public static float tan(float v) => (float) Tan(v);
		public static float abs(float v) => Abs(v);

		public static float sin(double v) => (float) Sin(v);
		public static float cos(double v) => (float) Cos(v);
		public static float tan(double v) => (float) Tan(v);
		public static float abs(double v) => (float) Abs(v);
	}
}