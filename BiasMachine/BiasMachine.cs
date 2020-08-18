using System;

namespace BiasMachine
{
	public class BiasMachine
	{
		public Param[][] Weights { get; protected set; }

		public IActivation[][] Activation { get; protected set; }

		public int[] Wide { get; protected set; }

		public int Input { get; protected set; }

		public int Output { get; protected set; }

		public int Lays { get; protected set; }

		public Param Bias { get; set; }

		public BiasMachine(int input, params int[] wide)
		{
			BuildMachine(input, wide);
		}

		public void BuildMachine(int input, params int[] wide)
		{
			CheckInput(input, wide);
			SetInput(input, wide);
			
			Weights = new Param[Lays][];
			
			Weights[0] = new Param[Input*Wide[0]];

			for(int j=0; j<Input*Wide[0]; ++j)
			{
				Weights[0][j] = new Param();
			}

			for(int i=1; i<Lays; ++i)
			{
				int width = Wide[i-1]*Wide[i];
				
				Weights[i] = new Param[width];
				
				for(int j=0; j<width; ++j)
				{
					Weights[i][j] = new Param();
				}
			}

			Activation = new IActivation[Lays][];

			for(int i=0; i<Lays; ++i)
			{
				Activation[i] = new IActivation[Wide[i]];

				for(int j=0; j<Wide[i]; ++j)
				{
					Activation[i][j] = new LeakyReLU();
				}
			}

			Bias = new Param(0.01, 0.20, 0.05);
		}

		public void SetActivationFunction(int lay, IActivation activation)
		{
			if(lay < Lays)
			{
				for(int i=0; i<Wide[lay]; ++i)
				{
					Activation[lay][i] = activation;
				}
			}
		}

		public double[] Compute(double[] input)
		{
			if(input.Length != Input)
			{
				throw new ArgumentException();
			}
			
			double[] buffer = input;

			for(int i=0; i<Lays; ++i)
			{
				double[] next = new double[Wide[i]];

				int index = 0;

				for(int j=0; j<Wide[i]; ++j)
				{
					double value = Bias;

					for(int z=0; z<buffer.Length; ++z)
					{
						value += Weights[i][index]*buffer[z];
						++index;
					}

					next[j] = Activation[i][j].Compute(value);
				}

				buffer = next;
			}			

			return buffer;
		}

		public void Mutation()
		{
			for(int j=0; j<Input*Wide[0]; ++j)
			{
				Weights[0][j].Mutation();
			}

			for(int i=1; i<Lays; ++i)
			{
				int width = Wide[i-1]*Wide[i];
				
				for(int j=0; j<width; ++j)
				{
					Weights[i][j].Mutation();
				}
			}

			for(int i=0; i<Lays; ++i)
			{
				for(int j=0; j<Wide[i]; ++j)
				{
					Activation[i][j].Mutation();
				}
			}

			Bias.Mutation();
		}

		public BiasMachine Clone()
		{
			BiasMachine clone = new BiasMachine(Input, Wide);

			clone.Bias = Bias;

			for(int j=0; j<Input*Wide[0]; ++j)
			{
				clone.Weights[0][j] = Weights[0][j].Clone();
			}

			for(int i=1; i<Lays; ++i)
			{
				for(int j=0; j<Wide[i-1]*Wide[i]; ++j)
				{
					clone.Weights[i][j] = Weights[i][j].Clone();
				}
			}

			for(int i=0; i<Lays; ++i)
			{
				for(int j=0; j<Wide[i]; ++j)
				{
					clone.Activation[i][j] = Activation[i][j].Clone();
				}
			}

			return clone;
		}

		public BiasMachine Pairing(BiasMachine machine)
		{
			if(machine == null)
			{
				throw new ArgumentNullException();
			}

			if(machine.Wide.Length != Wide.Length)
			{
				throw new ArgumentException();
			}

			for(int i=0; i<Wide.Length; ++i)
			{
				if(machine.Wide[i] != Wide[i])
				{
					throw new ArgumentException();
				}
			}

			BiasMachine child = new BiasMachine(Input, Wide);

			int bias = FixedRandom.Next(3);

			if(bias == 0)
			{
				child.Bias = machine.Bias;
			}

			if(bias == 1)
			{
				child.Bias = Bias;
			}

			if(bias == 2)
			{
				child.Bias = new Param(0.5*(machine.Bias+Bias));
			}

			for(int i=0; i<Lays; ++i)
			{
				for(int j=0; j<Wide[i]; ++j)
				{
					if(FixedRandom.GetBool())
					{
						child.Activation[i][j] = machine.Activation[i][j];
					}
					else
					{
						child.Activation[i][j] = Activation[i][j];
					}
				}
			}

			for(int j=0; j<Input*Wide[0]; ++j)
			{
				int weight = FixedRandom.Next(3);

				if(weight == 0)
				{
					child.Weights[0][j] = machine.Weights[0][j];
				}
				
				if(weight == 1)
				{
					child.Weights[0][j] = Weights[0][j];
				}

				if(weight == 3)
				{
					child.Weights[0][j] = new Param(0.5*(machine.Weights[0][j] + Weights[0][j]));
				}
			}

			for(int i=1; i<Lays; ++i)
			{
				int width = Wide[i-1]*Wide[i];

				for(int j=0; j<width; ++j)
				{
					int weight = FixedRandom.Next(3);

					if(weight == 0)
					{
						child.Weights[i][j] = machine.Weights[i][j];
					}
				
					if(weight == 1)
					{
						child.Weights[i][j] = Weights[i][j];
					}

					if(weight == 3)
					{
						child.Weights[i][j] = new Param(0.5*(machine.Weights[i][j] + Weights[i][j]));
					}
				}
			}

			return child;
		}

		private void CheckInput(int input, params int[] wide)
		{
			if(input <= 0)
			{
				throw new ArgumentException();
			}

			int lays = wide.Length;

			if(lays <= 0)
			{
				throw new ArgumentException();
			}

			for(int i=0; i<lays; ++i)
			{
				if(wide[i] <= 0)
				{
					throw new ArgumentException();
				}
			}
		}

		private void SetInput(int input, params int[] wide)
		{
			Input = input;
			Output = wide[wide.Length-1];
			Lays = wide.Length;

			Wide = new int[Lays];

			for(int i=0; i<Lays; ++i)
			{
				Wide[i] = wide[i];
			}
		}
	}
}
