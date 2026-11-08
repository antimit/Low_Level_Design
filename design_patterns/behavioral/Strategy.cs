
using System;

// 1. Strategy interface
interface ICalculationStrategy
{
    int Calculate(int a, int b);
}

// 2. Concrete strategies
class AddStrategy : ICalculationStrategy
{
    public int Calculate(int a, int b)
    {
        return a + b;
    }
}

class SubtractStrategy : ICalculationStrategy
{
    public int Calculate(int a, int b)
    {
        return a - b;
    }
}

class MultiplyStrategy : ICalculationStrategy
{
    public int Calculate(int a, int b)
    {
        return a * b;
    }
}

// 3. Context
class Calculator
{
    private ICalculationStrategy _strategy;

    public Calculator(ICalculationStrategy strategy)
    {
        _strategy = strategy;
    }

    public void SetStrategy(ICalculationStrategy strategy)
    {
        _strategy = strategy;
    }

    public int Execute(int a, int b)
    {
        return _strategy.Calculate(a, b);
    }
}

// 4. Client
class Program
{
    static void Main()
    {
        Calculator calculator = new Calculator(new AddStrategy());

        Console.WriteLine("Addition: " + calculator.Execute(10, 5));

        calculator.SetStrategy(new SubtractStrategy());
        Console.WriteLine("Subtraction: " + calculator.Execute(10, 5));

        calculator.SetStrategy(new MultiplyStrategy());
        Console.WriteLine("Multiplication: " + calculator.Execute(10, 5));
    }
}
