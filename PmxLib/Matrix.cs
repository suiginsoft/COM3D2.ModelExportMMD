namespace PmxLib
{
	public struct Matrix
	{
		public float M11;

		public float M12;

		public float M13;

		public float M14;

		public float M21;

		public float M22;

		public float M23;

		public float M24;

		public float M31;

		public float M32;

		public float M33;

		public float M34;

		public float M41;

		public float M42;

		public float M43;

		public float M44;

		public static Matrix Identity
		{
			get
			{
				Matrix result = default(Matrix);
				result.M11 = 1f;
				result.M22 = 1f;
				result.M33 = 1f;
				result.M44 = 1f;
				return result;
			}
		}

		public float[] ToArray()
		{
			return new float[16]
			{
				M11,
				M12,
				M13,
				M14,
				M21,
				M22,
				M23,
				M24,
				M31,
				M32,
				M33,
				M34,
				M41,
				M42,
				M43,
				M44
			};
		}
	}
}
