﻿using System;

namespace BiasMachine
{
	public static class FixedRandom
	{
		////////////////////////////////////////////////////////////////////////
		
		private static Random random;

		////////////////////////////////////////////////////////////////////////

		private static int seed;

		////////////////////////////////////////////////////////////////////////

		public static int Seed
		{
			get => seed;

			set
			{
				seed = value;
				random = new Random(value);
			}
		}

		////////////////////////////////////////////////////////////////////////

		static FixedRandom()
		{
			random = new Random();

			seed = random.Next();

			random = new Random(seed);
		}

		////////////////////////////////////////////////////////////////////////

		public static int Next()
		{
			return random.Next();
		}

		////////////////////////////////////////////////////////////////////////

		public static int Next(int max)
		{
			return random.Next(max);
		}

		////////////////////////////////////////////////////////////////////////

		public static int Next(int min, int max)
		{
			return random.Next(min, max);
		}

		////////////////////////////////////////////////////////////////////////

		public static double NextDouble()
		{
			return random.NextDouble();
		}

		////////////////////////////////////////////////////////////////////////

		public static double NextDouble(double max)
		{
			return max*random.NextDouble();
		}

		////////////////////////////////////////////////////////////////////////

		public static double NextDouble(double min, double max)
		{
			if(min >= max)
			{
				throw new ArgumentException();
			}

			return (max-min)*random.NextDouble() + min;
		}

		////////////////////////////////////////////////////////////////////////

		public static bool GetBool()
		{
			if(random.Next() % 2 == 0)
			{
				return true;
			}

			return false;
		}

		////////////////////////////////////////////////////////////////////////
	}
}
