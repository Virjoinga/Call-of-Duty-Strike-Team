using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Snowbolt
{
	public class PerlinNoise
	{
		private struct Hasher
		{
			private uint m_value;

			public Hasher(uint _value)
			{
				m_value = 2166136261u;
				Set(_value);
			}

			public void Set(uint _value)
			{
				Set((int)_value);
			}

			public void Set(float _value)
			{
				Set((int)_value);
			}

			public void Set(int _value)
			{
				SetByte((uint)_value & 0xFFu);
				SetByte((uint)(_value >> 8) & 0xFFu);
				SetByte((uint)(_value >> 16) & 0xFFu);
				SetByte((uint)(_value >> 24) & 0xFFu);
			}

			public void SetByte(uint _value)
			{
				m_value = (m_value * 16777619) ^ _value;
			}

			public uint Get()
			{
				return m_value;
			}
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct ReinterpretHelper
		{
			[FieldOffset(0)]
			public uint m_valueUInt;

			[FieldOffset(0)]
			public int m_valueInt;

			[FieldOffset(0)]
			public float m_valueFloat;
		}

		private int m_seed;

		private int m_numOctaves;

		private uint[] m_octaveSeed;

		private float[] m_octaveInScale;

		private float[] m_octaveOutScale;

		private float m_norm;

		public int NumOctaves
		{
			get
			{
				return m_numOctaves;
			}
		}

		public PerlinNoise(int _seed, int _numOctaves)
		{
			m_seed = _seed;
			m_numOctaves = _numOctaves;
			System.Random random = new System.Random(m_seed);
			m_octaveSeed = new uint[m_numOctaves];
			for (int i = 0; i < m_octaveSeed.Length; i++)
			{
				m_octaveSeed[i] = (uint)random.Next();
			}
			float[] array = new float[m_numOctaves];
			float[] array2 = new float[m_numOctaves];
			for (int j = 0; j < m_numOctaves; j++)
			{
				array[j] = Mathf.Pow(2f, j);
				array2[j] = Mathf.Pow(2f, 0f - (float)j);
			}
			SetInScales(array);
			SetOutScales(array2);
		}

		public void SetInScales(float[] _inScale)
		{
			m_octaveInScale = _inScale;
		}

		public void SetOutScales(float[] _outScale)
		{
			m_octaveOutScale = _outScale;
			m_norm = 0f;
			for (int i = 0; i < m_numOctaves; i++)
			{
				m_norm += m_octaveOutScale[i];
			}
			m_norm = 1f / m_norm;
		}

		public float OctaveF(float _x, uint _octaveSeed)
		{
			Hasher hasher = new Hasher(_octaveSeed);
			hasher.Set(_x);
			uint num = hasher.Get();
			num = ((num >> 3) & 0x7FFFFFu) | 0x3F800000u;
			ReinterpretHelper reinterpretHelper = default(ReinterpretHelper);
			reinterpretHelper.m_valueUInt = num;
			return reinterpretHelper.m_valueFloat - 1f;
		}

		public float OctaveF(float _x, float _y, uint _octaveSeed)
		{
			Hasher hasher = new Hasher(_octaveSeed);
			hasher.Set(_x);
			hasher.Set(_y);
			uint num = hasher.Get();
			num = ((num >> 3) & 0x7FFFFFu) | 0x3F800000u;
			ReinterpretHelper reinterpretHelper = default(ReinterpretHelper);
			reinterpretHelper.m_valueUInt = num;
			return reinterpretHelper.m_valueFloat - 1f;
		}

		public float OctaveF(float _x, float _y, float _z, uint _octaveSeed)
		{
			Hasher hasher = new Hasher(_octaveSeed);
			hasher.Set(_x);
			hasher.Set(_y);
			hasher.Set(_z);
			uint num = hasher.Get();
			num = ((num >> 3) & 0x7FFFFFu) | 0x3F800000u;
			ReinterpretHelper reinterpretHelper = default(ReinterpretHelper);
			reinterpretHelper.m_valueUInt = num;
			return reinterpretHelper.m_valueFloat - 1f;
		}

		public float SmoothedOctaveF(float _x, uint _octaveSeed, float _octaveInScale)
		{
			float num = _x * _octaveInScale;
			float num2 = Mathf.Floor(num);
			float x = num2 + 1f;
			float t = num - num2;
			return Mathf.Lerp(OctaveF(num2, _octaveSeed), OctaveF(x, _octaveSeed), t);
		}

		public float SmoothedOctaveF(float _x, float _y, uint _octaveSeed, float _octaveInScale)
		{
			float num = _x * _octaveInScale;
			float num2 = Mathf.Floor(num);
			float x = num2 + 1f;
			float t = num - num2;
			float num3 = _y * _octaveInScale;
			float num4 = Mathf.Floor(num3);
			float y = num4 + 1f;
			float t2 = num3 - num4;
			float from = Mathf.Lerp(OctaveF(num2, num4, _octaveSeed), OctaveF(x, num4, _octaveSeed), t);
			float to = Mathf.Lerp(OctaveF(num2, y, _octaveSeed), OctaveF(x, y, _octaveSeed), t);
			return Mathf.Lerp(from, to, t2);
		}

		public float SmoothedOctaveF_Cosine(float _x, float _y, uint _octaveSeed, float _octaveInScale)
		{
			float num = _x * _octaveInScale;
			float num2 = Mathf.Floor(num);
			float x = num2 + 1f;
			float lerp = num - num2;
			float num3 = _y * _octaveInScale;
			float num4 = Mathf.Floor(num3);
			float y = num4 + 1f;
			float lerp2 = num3 - num4;
			float a = MathUtils.CosineLerp(OctaveF(num2, num4, _octaveSeed), OctaveF(x, num4, _octaveSeed), lerp);
			float b = MathUtils.CosineLerp(OctaveF(num2, y, _octaveSeed), OctaveF(x, y, _octaveSeed), lerp);
			return MathUtils.CosineLerp(a, b, lerp2);
		}

		public float SmoothedOctaveF(float _x, float _y, float _z, uint _octaveSeed, float _octaveInScale)
		{
			float num = _x * _octaveInScale;
			float num2 = Mathf.Floor(num);
			float x = num2 + 1f;
			float t = num - num2;
			float num3 = _y * _octaveInScale;
			float num4 = Mathf.Floor(num3);
			float y = num4 + 1f;
			float t2 = num3 - num4;
			float num5 = _z * _octaveInScale;
			float num6 = Mathf.Floor(num5);
			float z = num6 + 1f;
			float t3 = num5 - num6;
			float from = Mathf.Lerp(OctaveF(num2, num4, num6, _octaveSeed), OctaveF(x, num4, num6, _octaveSeed), t);
			float to = Mathf.Lerp(OctaveF(num2, y, num6, _octaveSeed), OctaveF(x, y, num6, _octaveSeed), t);
			float from2 = Mathf.Lerp(from, to, t2);
			float from3 = Mathf.Lerp(OctaveF(num2, num4, z, _octaveSeed), OctaveF(x, num4, z, _octaveSeed), t);
			float to2 = Mathf.Lerp(OctaveF(num2, y, z, _octaveSeed), OctaveF(x, y, z, _octaveSeed), t);
			float to3 = Mathf.Lerp(from3, to2, t2);
			return Mathf.Lerp(from2, to3, t3);
		}

		public float SmoothedOctaveF_Cosine(float _x, float _y, float _z, uint _octaveSeed, float _octaveInScale)
		{
			float num = _x * _octaveInScale;
			float num2 = Mathf.Floor(num);
			float x = num2 + 1f;
			float lerp = num - num2;
			float num3 = _y * _octaveInScale;
			float num4 = Mathf.Floor(num3);
			float y = num4 + 1f;
			float lerp2 = num3 - num4;
			float num5 = _z * _octaveInScale;
			float num6 = Mathf.Floor(num5);
			float z = num6 + 1f;
			float lerp3 = num5 - num6;
			float a = MathUtils.CosineLerp(OctaveF(num2, num4, num6, _octaveSeed), OctaveF(x, num4, num6, _octaveSeed), lerp);
			float b = MathUtils.CosineLerp(OctaveF(num2, y, num6, _octaveSeed), OctaveF(x, y, num6, _octaveSeed), lerp);
			float a2 = MathUtils.CosineLerp(a, b, lerp2);
			float a3 = MathUtils.CosineLerp(OctaveF(num2, num4, z, _octaveSeed), OctaveF(x, num4, z, _octaveSeed), lerp);
			float b2 = MathUtils.CosineLerp(OctaveF(num2, y, z, _octaveSeed), OctaveF(x, y, z, _octaveSeed), lerp);
			float b3 = MathUtils.CosineLerp(a3, b2, lerp2);
			return MathUtils.CosineLerp(a2, b3, lerp3);
		}

		public float GetValueF(float _x)
		{
			float num = 0f;
			for (int i = 0; i < m_numOctaves; i++)
			{
				float num2 = SmoothedOctaveF(_x, m_octaveSeed[i], m_octaveInScale[i]);
				num += num2 * m_octaveOutScale[i];
			}
			return num * m_norm;
		}

		public float GetValueF(float _x, float _y)
		{
			float num = 0f;
			for (int i = 0; i < m_numOctaves; i++)
			{
				float num2 = SmoothedOctaveF(_x, _y, m_octaveSeed[i], m_octaveInScale[i]);
				num += num2 * m_octaveOutScale[i];
			}
			return num * m_norm;
		}

		public float GetValueF_Cosine(float _x, float _y)
		{
			float num = 0f;
			for (int i = 0; i < m_numOctaves; i++)
			{
				float num2 = SmoothedOctaveF_Cosine(_x, _y, m_octaveSeed[i], m_octaveInScale[i]);
				num += num2 * m_octaveOutScale[i];
			}
			return num * m_norm;
		}

		public float GetValueF(float _x, float _y, float _z)
		{
			float num = 0f;
			for (int i = 0; i < m_numOctaves; i++)
			{
				float num2 = SmoothedOctaveF(_x, _y, _z, m_octaveSeed[i], m_octaveInScale[i]);
				num += num2 * m_octaveOutScale[i];
			}
			return num * m_norm;
		}

		public float GetValueF_Cosine(float _x, float _y, float _z)
		{
			float num = 0f;
			for (int i = 0; i < m_numOctaves; i++)
			{
				float num2 = SmoothedOctaveF_Cosine(_x, _y, _z, m_octaveSeed[i], m_octaveInScale[i]);
				num += num2 * m_octaveOutScale[i];
			}
			return num * m_norm;
		}

		public float GetValueF(float _x, float _y, float _z, float[] o_octaveValues)
		{
			float num = 0f;
			for (int i = 0; i < m_numOctaves; i++)
			{
				num += (o_octaveValues[i] = SmoothedOctaveF(_x, _y, _z, m_octaveSeed[i], m_octaveInScale[i])) * m_octaveOutScale[i];
			}
			return num * m_norm;
		}

		public float GetValueF_Cosine(float _x, float _y, float _z, float[] o_octaveValues)
		{
			float num = 0f;
			for (int i = 0; i < m_numOctaves; i++)
			{
				num += (o_octaveValues[i] = SmoothedOctaveF_Cosine(_x, _y, _z, m_octaveSeed[i], m_octaveInScale[i])) * m_octaveOutScale[i];
			}
			return num * m_norm;
		}

		public float GetTiledValueF(float _x, float _y, float _xMax, float _yMax)
		{
			float num = _x % _xMax;
			float num2 = _y % _yMax;
			if (num < 0f)
			{
				num = _xMax + num;
			}
			if (num2 < 0f)
			{
				num2 = _yMax + num2;
			}
			float num3 = GetValueF(num, num2) * (_xMax - num) * (_yMax - num2);
			num3 += GetValueF(num - _xMax, num2) * num * (_yMax - num2);
			num3 += GetValueF(num - _xMax, num2 - _yMax) * num * num2;
			num3 += GetValueF(num, num2 - _yMax) * (_xMax - num) * num2;
			return num3 / (_xMax * _yMax);
		}
	}
}
