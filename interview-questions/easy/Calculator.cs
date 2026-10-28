
using System;
using System.Collections.Generic;

// 1. Interface for calculator operations
public interface IOperation
{
    decimal Execute(decimal a, decimal b);
}

// 2. Calculator class
public class Calculator
{
    private readonly Stack<decimal> _history;
    private readonly Dictionary<string, IOperation> _operations;

    public Calculator()
    {
        _history = new Stack<decimal>();

        _operations = new Dictionary<string, IOperation>
        {
            { "+", new AddOperation() },
            { "-", new SubtractOperation() },
            { "*", new MultiplyOperation() },
            { "/", new DivideOperation() }
        };
    }

    // Perform a calculation
    public decimal Calculate(string op, decimal a, decimal b)
    {
        if (!_operations.TryGetValue(op, out IOperation? operation))
        {
            throw new ArgumentException("Invalid operator.");
        }

        decimal result = operation.Execute(a, b);

        _history.Push(result);

        return result;
    }

    // Undo the latest calculation
    public decimal? Undo()
    {
        if (_history.Count > 0)
        {
            return _history.Pop();
        }

        return null;
    }

    // View calculation history
    public void ShowHistory()
    {
        Console.WriteLine("Calculation History:");

        foreach (decimal result in _history)
        {
            Console.WriteLine(result);
        }
    }
}

// 3. Addition
public class AddOperation : IOperation
{
    public decimal Execute(decimal a, decimal b)
    {
        return a + b;
    }
}

// 4. Subtraction
public class SubtractOperation : IOperation
{
    public decimal Execute(decimal a, decimal b)
    {
        return a - b;
    }
}

// 5. Multiplication
public class MultiplyOperation : IOperation
{
    public decimal Execute(decimal a, decimal b)
    {
        return a * b;
    }
}

// 6. Division
public class DivideOperation : IOperation
{
    public decimal Execute(decimal a, decimal b)
    {
        if (b == 0)
        {
            throw new DivideByZeroException("Cannot divide by zero.");
        }

        return a / b;
    }
}

// 7. Application usage
public class Program
{
    public static void Main(string[] args)
    {
        Calculator calculator = new Calculator();

        Console.WriteLine(calculator.Calculate("+", 10m, 5m));
        Console.WriteLine(calculator.Calculate("-", 10m, 3m));
        Console.WriteLine(calculator.Calculate("*", 4m, 2.5m));
        Console.WriteLine(calculator.Calculate("/", 10m, 2m));

        // Show all results
        calculator.ShowHistory();

        // Undo the latest calculation
        decimal? undone = calculator.Undo();

        Console.WriteLine($"Undone result: {undone}");

        // Show history after undo
        calculator.ShowHistory();

        // Error handling
        try
        {
            calculator.Calculate("/", 10m, 0m);
        }
        catch (DivideByZeroException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
