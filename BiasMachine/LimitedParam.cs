using System;

namespace BiasMachine
{
	public class LimitedParam : Param
	{
		////////////////////////////////////////////////////////////////////////
		protected double Min { get; private set; }

		////////////////////////////////////////////////////////////////////////

		protected double Max { get; private set; }
		
		////////////////////////////////////////////////////////////////////////

		static LimitedParam()
		{
			DefaultMutationRate = 0.100;
			DefaultMutationFactor = 0.200;
			DefaultIncreaseFactor = 0.200;
		}

		////////////////////////////////////////////////////////////////////////

		public LimitedParam(): base()
		{
			Min = 0.0;
			Max = 1.0;

			ResetValue();
		}

		////////////////////////////////////////////////////////////////////////

		public LimitedParam(double min, double max): base()
		{
			Min = min;
			Max = max;

			ResetValue();
		}

		////////////////////////////////////////////////////////////////////////

		public LimitedParam(double min, double max, double value): base(value)
		{
			if(value < min || value > max)
			{
				throw new ArgumentException();
			}
			
			if(min >= max)
			{
				throw new ArgumentException();
			}

			Min = min;
			Max = max;
		}

		////////////////////////////////////////////////////////////////////////

		public LimitedParam(double min, double max, double mutationRate, double mutationFactor): base(mutationRate, mutationFactor)
		{
			Min = min;
			Max = max;

			ResetValue();
		}

		////////////////////////////////////////////////////////////////////////

		public LimitedParam(double min, double max, double value, double mutationRate, double mutationFactor): base(value, mutationRate, mutationFactor)
		{
			if(value < min || value > max)
			{
				throw new ArgumentException();
			}

			if(min >= max)
			{
				throw new ArgumentException();
			}

			Min = min;
			Max = max;
		}

		////////////////////////////////////////////////////////////////////////

		public override void SetValue(double value)
		{
			Value = value;

			if(Value < Min)
			{
				Value = Min;
			}

			if(Value > Max)
			{
				Value = Max;
			}
		}

		////////////////////////////////////////////////////////////////////////

		public new LimitedParam Clone()
		{
			LimitedParam param = new LimitedParam(Min, Max, Value, MutationRate, MutationFactor);

			param.IncreaseFactor = IncreaseFactor;
			param.Increase = Increase;

			return param;
		}

		////////////////////////////////////////////////////////////////////////

		public new void ResetValue()
		{
			Value = FixedRandom.NextDouble(Min, Max);
		}

		////////////////////////////////////////////////////////////////////////
	}
}
