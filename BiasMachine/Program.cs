using System;

namespace BiasMachine
{
	class Program
	{
		static void Main()
		{
			BiasMachine machine = new BiasMachine(4, 5, 4);

			machine.SetActivationFunction(1, new Sigmoid());
			machine.SetActivationFunction(2, new Gaussian());
			machine.SetActivationFunction(1, 1, new LeakyReLU());
			
			BiasMachine clone = machine.Clone();
			
			for(int i=0; i<1000; ++i)
			{
				machine.Mutation();
			}
			
			BiasMachine child = machine.Pairing(clone);
		}
	}
}
