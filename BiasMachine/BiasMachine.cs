using System;

namespace BiasMachine
{
	public class BiasMachine
	{
		////////////////////////////////////////////////////////////////////////
		
		public Param[][] Weights { get; protected set; }

		////////////////////////////////////////////////////////////////////////

		public IActivation[][] Activation { get; protected set; }

		////////////////////////////////////////////////////////////////////////

		public int[] Wide { get; protected set; }

		////////////////////////////////////////////////////////////////////////

		public int Input { get; protected set; }

		////////////////////////////////////////////////////////////////////////

		public int Output { get; protected set; }

		////////////////////////////////////////////////////////////////////////

		public int Lays { get; protected set; }

		////////////////////////////////////////////////////////////////////////

		public bool ChangeActivationFunctionEnabled { get; set; } = true;

		////////////////////////////////////////////////////////////////////////

		public BiasMachine(params int[] wide)
		{
			BuildMachine(wide);
		}

		////////////////////////////////////////////////////////////////////////

		public BiasMachine Clone()
		{
			BiasMachine clone = new BiasMachine(Wide);

			for(int i=0; i<Lays-1; ++i)
			{
				int width = Wide[i]*Wide[i+1];

				for(int j=0; j<width; ++j)
				{
					clone.Weights[i][j] = Weights[i][j].Clone();
				}
			}

			for(int i=0; i<Lays-1; ++i)
			{				
				for(int j=0; j<Wide[i+1]; ++j)
				{
					clone.Activation[i][j] = Activation[i][j].Clone();
				}
			}

			return clone;
		}

		////////////////////////////////////////////////////////////////////////

		public BiasMachine Pairing(BiasMachine pair)
		{
			BiasMachine child = new BiasMachine(Wide);

			for(int i=0; i<Lays-1; ++i)
			{
				int width = Wide[i]*Wide[i+1];

				for(int j=0; j<width; ++j)
				{
					int random1 = FixedRandom.Next(2);

					Param param1 = pair.Weights[i][j];
					Param param2 = this.Weights[i][j];

					if(random1 == 0)
					{
						child.Weights[i][j] = param1.Clone();
					}

					if(random1 == 1)
					{
						child.Weights[i][j] = param2.Clone();
					}

					if(random1 == 2)
					{
						double average = 0.5*(param1 + param2);
						
						Param param = new Param(average);

						if(FixedRandom.GetBool())
						{
							param.IncreaseFactor = param1.IncreaseFactor;
							param.MutationFactor = param1.MutationFactor;
							param.MutationRate = param1.MutationRate;
						}
						else
						{
							param.IncreaseFactor = param2.IncreaseFactor;
							param.MutationFactor = param2.MutationFactor;
							param.MutationRate = param2.MutationRate;
						}
					}
				}
			}

			for(int i=0; i<Lays-1; ++i)
			{				
				for(int j=0; j<Wide[i+1]; ++j)
				{
					child.Activation[i][j] = Activation[i][j].Clone();
				}
			}

			return child;
		}

		////////////////////////////////////////////////////////////////////////

		public void BuildMachine(params int[] wide)
		{
			CheckInput(wide);
			SetInput(wide);
			
			Weights = new Param[Lays-1][];
			
			for(int i=0; i<Lays-1; ++i)
			{
				int width = Wide[i]*Wide[i+1];

				Weights[i] = new Param[width];

				for(int j=0; j<width; ++j)
				{
					Weights[i][j] = new Param();
				}
			}

			Activation = new IActivation[Lays-1][];
			
			for(int i=0; i<Lays-1; ++i)
			{
				Activation[i] = new IActivation[Wide[i+1]];
				
				for(int j=0; j<Wide[i+1]; ++j)
				{
					Activation[i][j] = new LeakyReLU();
				}
			}
		}

		////////////////////////////////////////////////////////////////////////

		public void SetActivationFunction(int lay, IActivation activation)
		{
			if(lay == 0)
			{
				throw new ArgumentException("Activation function used only for hidden lays");
			}

			if(lay <= 0 || lay >= Lays)
			{
				throw new ArgumentException();
			}

			for(int i=0; i<Wide[lay]; ++i)
			{
				Activation[lay-1][i] = activation.Clone();
			}
		}

		////////////////////////////////////////////////////////////////////////

		public void SetActivationFunction(int lay, int index, IActivation activation)
		{
			if(lay == 0)
			{
				throw new ArgumentException("Activation function used only for hidden lays");
			}

			if(lay <= 0 || lay >= Lays)
			{
				throw new ArgumentException();
			}

			if(index < 0 || index >= Wide[lay])
			{
				throw new ArgumentException();
			}

			Activation[lay-1][index] = activation.Clone();
		}

		////////////////////////////////////////////////////////////////////////

		public double[] Compute(double[] input)
		{
			if(input == null)
			{
				throw new ArgumentNullException();
			}

			if(input.Length != Input)
			{
				throw new ArgumentException();
			}

			double[] buffer = input;

			for(int i=0; i<Lays-1; ++i)
			{
				int index = 0;

				double[] next = new double[Wide[i+1]];

				for(int j=0; j<Wide[i+1]; ++j)
				{
					double value = 0.0;

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

		////////////////////////////////////////////////////////////////////////

		public void Mutation()
		{
			for(int i=0; i<Lays-1; ++i)
			{
				int width = Wide[i]*Wide[i+1];

				for(int j=0; j<width; ++j)
				{
					Weights[i][j].Mutation();
				}
			}

			for(int i=0; i<Lays-1; ++i)
			{
				for(int j=0; j<Wide[i+1]; ++j)
				{
					if(FixedRandom.NextDouble() <= 0.01)
					{
						if(ChangeActivationFunctionEnabled)
						{
							Activation[i][j] = ChangeActivationFunction();
						}
					}

					Activation[i][j].Mutation();					
				}
			}
		}

		////////////////////////////////////////////////////////////////////////

		IActivation ChangeActivationFunction()
		{
			int random = FixedRandom.Next(7);			

			switch(random)
			{
				case 0: return new LeakyReLU();
				case 1: return new SingleStep();
				case 2: return new Elu();
				case 3: return new Sigmoid();
				case 4: return new Sinc();
				case 5: return new Gaussian();
				case 6: return new Softsign();
			}

			return new LeakyReLU();
		}

		////////////////////////////////////////////////////////////////////////

		private void CheckInput(params int[] wide)
		{
			if(wide == null)
			{
				throw new ArgumentNullException();
			}

			int lays = wide.Length;

			if(lays <= 1)
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

		////////////////////////////////////////////////////////////////////////

		private void SetInput(params int[] wide)
		{
			Input = wide[0];
			Output = wide[wide.Length-1];
			Lays = wide.Length;

			Wide = new int[Lays];

			for(int i=0; i<Lays; ++i)
			{
				Wide[i] = wide[i];
			}
		}

		////////////////////////////////////////////////////////////////////////
	}
}
