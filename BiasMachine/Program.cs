﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BiasMachine
{
	class Program
	{
		////////////////////////////////////////////////////////////////////////

		private const int Input = 1;
		private const int Output = 1;

		////////////////////////////////////////////////////////////////////////

		private static BiasMachine Best { get; set; }
		
		////////////////////////////////////////////////////////////////////////

		private static BiasMachine GetBiasMachine()
		{
			return new BiasMachine(Input, 4, Output);
		}

		////////////////////////////////////////////////////////////////////////

		private static double[] GetInput()
		{
			double[] input = new double[Input];

			input[0] = FixedRandom.NextDouble();

			return input;
		}

		////////////////////////////////////////////////////////////////////////

		private static double[] GetOutput(double[] input)
		{
			double[] output = new double[Output];

			output[0] = Math.Sin(input[0]);

			return output;
		}

		////////////////////////////////////////////////////////////////////////

		private static void Test(BiasMachine best)
		{
			const int count = 16;

			double[] average = new double[Output];

			StringBuilder stringBuilder = new StringBuilder();
			
			double absolute = 0.0;

			for(int i=0; i<count; ++i)
			{
				double[] input = GetInput();
				double[] output = GetOutput(input);

				double[] solution = best.Compute(input);
				
				stringBuilder.Append("(");

				for(int j=0; j<Input; ++j)
				{
					stringBuilder.Append(Float3(input[j]));

					if(j < Input-1)
					{
						stringBuilder.Append(", ");
					}
				}

				stringBuilder.Append(") => (");

				for(int j=0; j<Output; ++j)
				{
					stringBuilder.Append(Float3(output[j]));

					if(j < Output-1)
					{
						stringBuilder.Append(", ");
					}
				}

				stringBuilder.Append(") || (");

				for(int j=0; j<Output; ++j)
				{
					stringBuilder.Append(Float3(solution[j]));

					if(j < Output-1)
					{
						stringBuilder.Append(", ");
					}
				}

				stringBuilder.Append(") || (");

				for(int j=0; j<Output; ++j)
				{
					absolute += Math.Abs(output[j]-solution[j]);

					if(output[j] > 0.001)
					{
						double error = Math.Abs((output[j]-solution[j])/output[j]);
						stringBuilder.Append(Float3(100.0*error) + " %");
						average[j] += error;
					}
					else
					{
						stringBuilder.Append("X.XXX %");
					}
					
					if(j < Output-1)
					{
						stringBuilder.Append(", ");
					}
				}

				stringBuilder.Append(")\n\n");
			}

			stringBuilder.Append("(");

			for(int j=0; j<Output; ++j)
			{
				stringBuilder.Append(Float3(100.0*average[j]/count) + " %");

				if(j < Output-1)
				{
					stringBuilder.Append(", ");
				}
			}

			stringBuilder.Append(") || (");

			stringBuilder.Append(Float3(100.0*absolute/(count*Output)));

			stringBuilder.Append(")\n\n");

			Console.Write(stringBuilder.ToString());
			
			Best = best;
		}

		////////////////////////////////////////////////////////////////////////

		private static void StartSelection()
		{
			const int iteration = 100;
			const int steps = 100;
			const int count = 100;
			const int best = 10;
			
			BiasMachine[] population = new BiasMachine[count];
			BiasMachine[] successful = new BiasMachine[best];
			
			for(int j=0; j<count; ++j)
			{
				population[j] = GetBiasMachine();
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
						int random = FixedRandom.Next(best);

						population[j] = successful[random].Clone();
						
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
			const int count = 100;
			const int best = 10;
			
			BiasMachine[] population = new BiasMachine[count];
			BiasMachine[] successful = new BiasMachine[best];
			
			for(int j=0; j<count; ++j)
			{
				population[j] = GetBiasMachine();
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

						//if(best > 1)
						//{
						//	while(parent2 == parent1)
						//	{
						//		parent2 = FixedRandom.Next(best);
						//	}
						//}

						population[j] = successful[parent1].Pairing(successful[parent2]);

						population[j].Mutation();
					}
				}
			}
			
			Test(successful[0]);
		}

		////////////////////////////////////////////////////////////////////////

		private static BiasMachine Learn(double RequiredError)
		{
			const int iteration = 1000;
			const int steps = 100;
			const int count = 100;
			const int best = 10;

			BiasMachine[] population = new BiasMachine[count];
			BiasMachine[] successful = new BiasMachine[best];
			
			RequiredError = Math.Abs(RequiredError);

			for(int j=0; j<count; ++j)
			{
				population[j] = GetBiasMachine();
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

				if(i % 20 == 0)
				{
					Console.Clear();
					Console.WriteLine("Itteration: " + (i+1) + " error: " + Float3(scores[0].Value) + "\n\n");
				}

				if(Math.Abs(scores[0].Value) <= RequiredError)
				{
					return scores[0].Key;
				}

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
						int random = FixedRandom.Next(best);

						population[j] = successful[random].Clone();
						
						population[j].Mutation();
					}
				}
			}
			
			return successful[0];
		}

		////////////////////////////////////////////////////////////////////////

		private static string Float3(double value)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0:F3}", value);
		}

		////////////////////////////////////////////////////////////////////////

		private static void Main()
		{
			//StartSelection();
			//StartPairing();

			BiasMachine machine = Learn(0.01);
			Test(machine);

			Console.ReadKey();
		}

		////////////////////////////////////////////////////////////////////////
	}
}
