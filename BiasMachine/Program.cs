using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BiasMachine
{
	class Program
	{
		////////////////////////////////////////////////////////////////////////

		private const int Input = 2;
		private const int Output = 2;

		////////////////////////////////////////////////////////////////////////

		private static BiasMachine Best;
		
		////////////////////////////////////////////////////////////////////////

		private static double[] GetInput()
		{
			double[] input = new double[Input];

			input[0] = FixedRandom.NextDouble();
			input[1] = FixedRandom.NextDouble();

			return input;
		}

		////////////////////////////////////////////////////////////////////////

		private static double[] GetOutput(double[] input)
		{
			double[] output = new double[Output];

			output[0] = input[0] + input[1];
			output[1] = input[0];

			return output;
		}

		////////////////////////////////////////////////////////////////////////

		private static void Test(BiasMachine best)
		{
			const int count = 16;
			
			double average = 0.0;
			double order = 0.0;

			for(int i=0; i<count; ++i)
			{
				double[] input = GetInput();
				double[] result = GetOutput(input);

				double[] solution = best.Compute(input);
				
				double error = 0.0;
				double temp = 0.0;

				for(int z=0; z<result.Length; ++z)
				{
					error += Math.Abs(solution[z] - result[z]);
					temp += Math.Abs(result[z]);
				}
				
				average += error;
				order += temp;

				StringBuilder stringBuilder = new StringBuilder();		
				
				stringBuilder.Append(Float3(input[0]));
				stringBuilder.Append(" + ");
				stringBuilder.Append(Float3(input[1]));
				stringBuilder.Append(" = ");
				stringBuilder.Append(Float3(result[0]));
				stringBuilder.Append(" <==> ");
				stringBuilder.Append(Float3(solution[0]));

				if(result[0] != 0.0)
				{
					stringBuilder.Append(" [");
					stringBuilder.Append(Float3(3.0*100.0*error/temp));
					stringBuilder.Append(" %]");
				}
				
				stringBuilder.Append("\n\n");
				
				Console.Write(stringBuilder.ToString());
			}
			
			average /= (count*Output);
			order /= (count*Output);

			Console.WriteLine();
			Console.WriteLine(" [" + Float3(100.0*average/order) + " %]");

			Best = best;
		}

		////////////////////////////////////////////////////////////////////////

		private static void StartSelection()
		{
			const int iteration = 100;
			const int steps = 100;
			const int count = 40;
			const int best = 10;
			
			BiasMachine[] population = new BiasMachine[count];
			BiasMachine[] successful = new BiasMachine[best];
			
			for(int j=0; j<count; ++j)
			{
				population[j] = new BiasMachine(Input, Output);
			}
			
			for(int i=0; i<iteration; ++i)
			{
				List<KeyValuePair<BiasMachine, double>> scores = new List<KeyValuePair<BiasMachine, double>>(count);
				
				double[] error = new double[count];
				
				for(int u=0; u<steps; ++u)
				{
					double[] input = GetInput();
					double[] result = GetOutput(input);
					
					for(int j=0; j<count; ++j)
					{
						double[] solution = population[j].Compute(input);
						
						for(int z=0; z<Output; ++z)
						{
							error[j] += Math.Abs(solution[z]-result[z]);
						}
					}
				}
				
				for(int j=0; j<count; ++j)
				{
					scores.Add(new KeyValuePair<BiasMachine, double>(population[j], error[j]));				
				}
				
				scores.Sort((x, y) => x.Value.CompareTo(y.Value));
				
				for(int j=0; j<best; ++j)
				{
					successful[j] = scores[j].Key;
				}
				
				for(int j=0; j<count; ++j)
				{
					if(j < best)
					{
						population[j] = successful[j];
					}					
					else
					{
						population[j] = successful[FixedRandom.Next(best)].Clone();
						
						population[j].Mutation();
					}
				}
			}
			
			Test(successful[0]);
		}

		////////////////////////////////////////////////////////////////////////

		private static void StartPairing()
		{
			const int iteration = 100;
			const int steps = 100;
			const int count = 40;
			const int best = 10;
			
			BiasMachine[] population = new BiasMachine[count];
			BiasMachine[] successful = new BiasMachine[best];
			
			for(int j=0; j<count; ++j)
			{
				population[j] = new BiasMachine(Input, Output);
			}
			
			for(int i=0; i<iteration; ++i)
			{
				List<KeyValuePair<BiasMachine, double>> scores = new List<KeyValuePair<BiasMachine, double>>(count);
				
				double[] error = new double[count];
				
				for(int u=0; u<steps; ++u)
				{
					double[] input = GetInput();
					double[] result = GetOutput(input);
					
					for(int j=0; j<count; ++j)
					{
						double[] solution = population[j].Compute(input);
						
						for(int z=0; z<Output; ++z)
						{
							error[j] += Math.Abs(solution[z]-result[z]);
						}
					}
				}
				
				for(int j=0; j<count; ++j)
				{
					scores.Add(new KeyValuePair<BiasMachine, double>(population[j], error[j]));				
				}
				
				scores.Sort((x, y) => x.Value.CompareTo(y.Value));
				
				for(int j=0; j<best; ++j)
				{
					successful[j] = scores[j].Key;
				}
				
				for(int j=0; j<count; ++j)
				{
					if(j < best)
					{
						population[j] = successful[j];
					}					
					else
					{
						int parent1 = FixedRandom.Next(best);
						int parent2 = FixedRandom.Next(best);

						population[j] = successful[parent1].Pairing(successful[parent2]);
					}
				}
			}
			
			Test(successful[0]);
		}

		////////////////////////////////////////////////////////////////////////

		private static void Main()
		{
			StartSelection();
			StartPairing();

			Console.ReadKey();
		}

		////////////////////////////////////////////////////////////////////////

		private static string Float3(double value)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0:F3}", value);
		}

		////////////////////////////////////////////////////////////////////////

		private static string Float3(float value)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0:F3}", value);
		}
	}
}
