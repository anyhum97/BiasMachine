using System;

namespace BiasMachine
{
	public interface IActivation : IMutation
	{
		double Compute(double value);

		IActivation Clone();
	}

	public class LeakyReLU : IActivation
	{
		public Param Factor { get; protected set; }

		public Param Leak { get; protected set; }

		public LeakyReLU()
		{
			Factor = new Param(1.0);
			Leak = new Param(0.025);
		}

		public LeakyReLU(double factor, double leak)
		{
			Factor = new Param(factor);
			Leak = new Param(leak);
		}

		public LeakyReLU(Param factor, Param leak)
		{
			Factor = factor.Clone();
			Leak = leak.Clone();
		}

		public double Compute(double value)
		{
			if(value >= 0.0)
			{
				return Factor*value;
			}
			else
			{
				return Leak*value;
			}
		}

		public IActivation Clone()
		{
			return new LeakyReLU(Factor, Leak);
		}

		public void Mutation()
		{
			Factor.Mutation();
			Leak.Mutation();
		}
	}

	public class SingleStep : IActivation
	{
		public Param Threshold {  get; protected set; }

		public Param Amplitude {  get; protected set; }

		public SingleStep()
		{
			Threshold = new Param(0.0);
			Amplitude = new Param(1.0);
		}

		public SingleStep(double threshold)
		{
			Threshold = new Param(threshold);
			Amplitude = new Param(1.0);
		}

		public SingleStep(double threshold, double amplitude)
		{
			Threshold = new Param(threshold);
			Amplitude = new Param(amplitude);
		}

		public SingleStep(Param value)
		{
			Threshold = value.Clone();
			Amplitude = new Param(1.0);
		}

		public SingleStep(Param threshold, Param amplitude)
		{
			Threshold = threshold.Clone();
			Amplitude = amplitude.Clone();
		}

		public double Compute(double value)
		{
			if(value >= 0.0)
			{
				return Amplitude;
			}
			else
			{
				return 0.0;
			}
		}

		public IActivation Clone()
		{
			return new SingleStep(Threshold, Amplitude);
		}

		public void Mutation()
		{
			Threshold.Mutation();
			Amplitude.Mutation();
		}
	}

	public class Elu : IActivation
	{
		public Param Factor { get; protected set; }

		public Elu()
		{
			Factor = new Param(0.7);
		}

		public Elu(double factor)
		{
			Factor = new Param(factor);
		}

		public Elu(Param factor)
		{
			Factor = factor.Clone();
		}

		public double Compute(double value)
		{
			if(value > 0.0)
			{
				return value;
			}
			else
			{
				return Factor*(Math.Exp(value)-1.0);
			}
		}

		public IActivation Clone()
		{
			return new Elu(Factor);
		}

		public void Mutation()
		{
			Factor.Mutation();
		}
	}

	public class Sigmoid : IActivation
	{
		public Param Factor { get; protected set; }

		public Sigmoid()
		{
			Factor = new Param(4.0);
		}

		public Sigmoid(double factor)
		{
			Factor = new Param(factor);
		}

		public Sigmoid(Param factor)
		{
			Factor = factor.Clone();
		}

		public double Compute(double value)
		{
			return 1.0/(1.0+Math.Exp(-Factor*value));
		}

		public IActivation Clone()
		{
			return new Sigmoid(Factor);
		}

		public void Mutation()
		{
			Factor.Mutation();
		}
	}

	public class Sinc : IActivation
	{
		public double Compute(double value)
		{
			if(Math.Abs(value) < 0.0001)
			{
				return 1.0;
			}

			return Math.Sin(value)/value;
		}

		public IActivation Clone()
		{
			return new Sinc();
		}

		public void Mutation()
		{
			
		}
	}

	public class Gaussian : IActivation
	{
		public Param Factor { get; protected set; }

		public Gaussian()
		{
			Factor = new Param(1.0);
		}

		public Gaussian(double factor)
		{
			Factor = new Param(factor);
		}

		public Gaussian(Param factor)
		{
			Factor = factor.Clone();
		}

		public double Compute(double value)
		{
			return Math.Exp(-value*value);
		}

		public IActivation Clone()
		{
			return new Gaussian(Factor);
		}

		public void Mutation()
		{
			Factor.Mutation();
		}
	}

	public class Softsign : IActivation
	{
		public double Compute(double value)
		{
			return value / (1.0 + Math.Abs(value));
		}

		public IActivation Clone()
		{
			return new Softsign();
		}

		public void Mutation()
		{
			
		}
	}

	public class Exponential : IActivation
	{
		public LimitedParam Factor { get; protected set; }

		public Param Leak { get; protected set; }

		public double DefaultMinValue = 0.5;
		public double DefaultMaxValue = 2.5;
		public double DefaultValue = 1.25;

		public Exponential()
		{
			Factor = new LimitedParam(DefaultMinValue, DefaultMaxValue, DefaultValue);

			Leak = new Param(0.01);
		}

		public Exponential(double factor)
		{
			Factor = new LimitedParam(DefaultMinValue, DefaultMaxValue, factor);

			Leak = new Param(0.01);
		}

		public Exponential(double factor, double leak)
		{
			Factor = new LimitedParam(DefaultMinValue, DefaultMaxValue, factor);

			Leak = new Param(leak);
		}

		public Exponential(LimitedParam factor, LimitedParam leak)
		{
			Factor = factor.Clone();
			Leak = leak.Clone();
		}

		public double Compute(double value)
		{
			if(value >= 0.0)
			{
				return Math.Pow(value, Factor);
			}
			else
			{
				return Leak*value;
			}
		}

		public IActivation Clone()
		{
			return new Exponential(Factor, Leak);
		}

		public void Mutation()
		{
			Factor.Mutation();
			Leak.Mutation();
		}
	}
}
