
using System;
using System.Collections.Generic;

// 1. OBSERVER INTERFACE
// Every subscriber must implement Update().
public interface IObserver
{
    void Update(string productName);
}

// 2. SUBJECT INTERFACE
// Defines how observers subscribe and unsubscribe.
public interface ISubject
{
    void Subscribe(IObserver observer);
    void Unsubscribe(IObserver observer);
    void Notify(string productName);
}

// 3. CONCRETE SUBJECT (PUBLISHER)
// Store maintains a list of observers.
public class Store : ISubject
{
    private readonly List<IObserver> _observers = new();

    public void Subscribe(IObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
            Console.WriteLine("New customer subscribed.");
        }
    }

    public void Unsubscribe(IObserver observer)
    {
        _observers.Remove(observer);
        Console.WriteLine("A customer unsubscribed.");
    }

    public void Notify(string productName)
    {
        foreach (IObserver observer in _observers)
        {
            observer.Update(productName);
        }
    }

    public void AddProduct(string productName)
    {
        Console.WriteLine(
            $"\nStore: {productName} is now available!");

        Notify(productName);
    }
}

// 4. CONCRETE OBSERVER
// Customer reacts when notified by Store.
public class Customer : IObserver
{
    public string Name { get; }

    public Customer(string name)
    {
        Name = name;
    }

    public void Update(string productName)
    {
        Console.WriteLine(
            $"{Name} received notification: " +
            $"{productName} is available!");
    }
}

// 5. CLIENT CODE
public class Program
{
    public static void Main()
    {
        // Create publisher
        Store store = new Store();

        // Create observers
        Customer alice = new Customer("Alice");
        Customer bob = new Customer("Bob");
        Customer charlie = new Customer("Charlie");

        // Register observers
        store.Subscribe(alice);
        store.Subscribe(bob);
        store.Subscribe(charlie);

        // Publisher changes state
        store.AddProduct("iPhone 17");

        // Bob no longer wants notifications
        Console.WriteLine("\nBob unsubscribes...");
        store.Unsubscribe(bob);

        // Another product becomes available
        store.AddProduct("Samsung Galaxy S25");
    }
}
