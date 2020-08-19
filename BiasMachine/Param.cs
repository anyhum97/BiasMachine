using System;
using System.Globalization;

namespace BiasMachine
{
	////////////////////////////////////////////////////////////////////////
	
	public interface IMutation
	{
		void Mutation();
	}

	////////////////////////////////////////////////////////////////////////

	public class Param : IMutation
	{
		////////////////////////////////////////////////////////////////////////
		
		public double Value { get; protected set; }

		////////////////////////////////////////////////////////////////////////

		private double mutationRate;
		public double MutationRate
		{
			get => mutationRate;

			set
			{
				if(value > MaxMutationRate)
				{
					mutationRate = MaxMutationRate;
					return;
				}

				if(value < MinMutationRate)
				{
					mutationRate = MinMutationRate;
					return;
				}
				
				mutationRate = value;
			}
		}

		////////////////////////////////////////////////////////////////////////

		private double mutationFactor;
		public double MutationFactor
		{
			get => mutationFactor;

			set
			{
				if(value > MaxMutationFactor)
				{
					mutationFactor = MaxMutationFactor;
					return;
				}

				if(value < MinMutationFactor)
				{
					mutationFactor = MinMutationFactor;
					return;
				}
				
				mutationFactor = value;
			}
		}

		////////////////////////////////////////////////////////////////////////

		private double increaseFactor;
		public double IncreaseFactor
		{
			get => increaseFactor;

			set
			{
				if(value > MaxIncreaseFactor)
				{
					increaseFactor = MaxIncreaseFactor;
					return;
				}

				if(value < MinIncreaseFactor)
				{
					increaseFactor = MinIncreaseFactor;
					return;
				}
				
				increaseFactor = value;
			}
		}

		////////////////////////////////////////////////////////////////////////

		private static double defaultMutationRate;
		public static double DefaultMutationRate
		{
			get => defaultMutationRate;

			set
			{
				if(value > MaxMutationRate)
				{
					defaultMutationRate = MaxMutationRate;
					return;
				}

				if(value < MinMutationRate)
				{
					defaultMutationRate = MinMutationRate;
					return;
				}
				
				defaultMutationRate = value;
			}
		}

		////////////////////////////////////////////////////////////////////////

		private static double defaultMutationFactor;
		public static double DefaultMutationFactor
		{
			get => defaultMutationFactor;

			set
			{
				if(value > MaxMutationFactor)
				{
					defaultMutationFactor = MaxMutationFactor;
					return;
				}

				if(value < MinMutationFactor)
				{
					defaultMutationFactor = MinMutationFactor;
					return;
				}
				
				defaultMutationFactor = value;
			}
		}
		
		////////////////////////////////////////////////////////////////////////

		private static double defaultIncreaseFactor;
		public static double DefaultIncreaseFactor
		{
			get => defaultIncreaseFactor;

			set
			{
				if(value > MaxIncreaseFactor)
				{
					defaultIncreaseFactor = MaxIncreaseFactor;
					return;
				}

				if(value < MinIncreaseFactor)
				{
					defaultIncreaseFactor = MinIncreaseFactor;
					return;
				}
				
				defaultIncreaseFactor = value;
			}
		}

		////////////////////////////////////////////////////////////////////////

		protected bool Increase { get; set; }

		////////////////////////////////////////////////////////////////////////

		public const double MinMutationRate = 0.001;
		public const double MaxMutationRate = 1.000;

		public const double MinMutationFactor = 0.001;
		public const double MaxMutationFactor = 1.000;
		
		public const double MinIncreaseFactor = 0.001;
		public const double MaxIncreaseFactor = 1.000;

		private const double DefaultMinValue = -1.0;
		private const double DefaultMaxValue = 1.0;

		public const double Bias = 0.01;
		
		////////////////////////////////////////////////////////////////////////

		static Param()
		{
			DefaultMutationRate = 0.200;
			DefaultMutationFactor = 0.200;
			DefaultIncreaseFactor = 0.200;
		}

		////////////////////////////////////////////////////////////////////////

		public Param()
		{
			ResetValue();
			
			MutationRate = DefaultMutationRate;
			MutationFactor = DefaultMutationFactor;
			IncreaseFactor = DefaultIncreaseFactor;

			Increase = FixedRandom.GetBool();
		}

		////////////////////////////////////////////////////////////////////////

		public Param(double value)
		{
			Value = value;
			
			MutationRate = DefaultMutationRate;
			MutationFactor = DefaultMutationFactor;
			IncreaseFactor = DefaultIncreaseFactor;

			Increase = FixedRandom.GetBool();
		}

		////////////////////////////////////////////////////////////////////////

		public Param(double mutationRate, double mutationFactor)
		{
			ResetValue();
			
			MutationRate = mutationRate;
			MutationFactor = mutationFactor;
			IncreaseFactor = DefaultIncreaseFactor;

			Increase = FixedRandom.GetBool();
		}

		////////////////////////////////////////////////////////////////////////

		public Param(double value, double mutationRate, double mutationFactor)
		{
			Value = value;
			
			MutationRate = mutationRate;
			MutationFactor = mutationFactor;
			IncreaseFactor = DefaultIncreaseFactor;

			Increase = FixedRandom.GetBool();
		}

		////////////////////////////////////////////////////////////////////////

		public static implicit operator double(Param instance)
		{
			return instance.Value;
		}

		////////////////////////////////////////////////////////////////////////

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0:F3}", Value);
		}

		////////////////////////////////////////////////////////////////////////

		public virtual void SetValue(double value)
		{
			Value = value;
		}

		////////////////////////////////////////////////////////////////////////

		public virtual void Mutation()
		{
			if(FixedRandom.NextDouble() <= IncreaseFactor)
			{
				Increase = FixedRandom.GetBool();
			}

			if(FixedRandom.NextDouble() <= MutationRate)
			{
				double mutation = 0.0;
				
				double delta = Math.Max(Math.Abs(Value), Bias) * FixedRandom.NextDouble(MutationFactor);
				
				if(Increase)
				{
					mutation += delta;
				}
				else
				{
					mutation -= delta;
				}		
				
				SetValue(Value + mutation);

				if(FixedRandom.NextDouble() <= MutationRate)
				{
					MutationRate += FixedRandom.NextDouble(-0.01, 0.01);
				}
			}
		}

		////////////////////////////////////////////////////////////////////////

		public void ResetValue()
		{
			Value = FixedRandom.NextDouble(DefaultMinValue, DefaultMaxValue);
		}

		////////////////////////////////////////////////////////////////////////

		public Param Clone()
		{
			Param param = new Param(Value, MutationRate, MutationFactor);
			
			param.IncreaseFactor = IncreaseFactor;
			param.Increase = Increase;

			return param;
		}

		////////////////////////////////////////////////////////////////////////
	}
}

