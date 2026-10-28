using System;
using System.Collections.Generic;

public class Product
{
    public string Code { get; }
    public string Name { get; }
    public decimal Price { get; }
    public int Quantity { get; private set; }

    internal Product(string code, string name, decimal price, int quantity)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product code and name are required.");
        }

        if (price <= 0 || quantity < 0)
        {
            throw new ArgumentException("Price must be positive and quantity cannot be negative.");
        }

        Code = code.Trim();
        Name = name;
        Price = price;
        Quantity = quantity;
    }

    internal void Restock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Restock quantity must be positive.");
        }

        Quantity += quantity;
    }

    internal void RemoveOne()
    {
        if (Quantity == 0)
        {
            throw new InvalidOperationException("Product is sold out.");
        }

        Quantity--;
    }
}

// Simple factory: create and validate products in one place.
public static class ProductFactory
{
    public static Product Create(string code, string name, decimal price, int quantity)
    {
        return new Product(code, name, price, quantity);
    }
}

// Strategy pattern: each payment method decides which amounts it accepts.
public interface IPaymentMethod
{
    bool ProcessPayment(decimal amount);
}

public class CoinPayment : IPaymentMethod
{
    public bool ProcessPayment(decimal amount)
    {
        return amount == 0.10m || amount == 0.20m || amount == 0.50m ||
            amount == 1m || amount == 2m;
    }
}

public class BanknotePayment : IPaymentMethod
{
    public bool ProcessPayment(decimal amount)
    {
        return amount == 5m || amount == 10m || amount == 20m;
    }
}

// State pattern: selecting a product behaves differently before and after payment.
public interface IVendingMachineState
{
    void InsertMoney(VendingMachine machine, decimal amount, IPaymentMethod paymentMethod);
    Product SelectProduct(VendingMachine machine, string code);
}

public class IdleState : IVendingMachineState
{
    public void InsertMoney(VendingMachine machine, decimal amount, IPaymentMethod paymentMethod)
    {
        machine.AcceptPayment(amount, paymentMethod);
    }

    public Product SelectProduct(VendingMachine machine, string code)
    {
        throw new InvalidOperationException("Insert money before selecting a product.");
    }
}

public class HasMoneyState : IVendingMachineState
{
    public void InsertMoney(VendingMachine machine, decimal amount, IPaymentMethod paymentMethod)
    {
        machine.AcceptPayment(amount, paymentMethod);
    }

    public Product SelectProduct(VendingMachine machine, string code)
    {
        return machine.Dispense(code);
    }
}

public class VendingMachine
{
    private readonly Dictionary<string, Product> _inventory =
        new Dictionary<string, Product>(StringComparer.OrdinalIgnoreCase);
    private IVendingMachineState _state = new IdleState();

    public decimal Balance { get; private set; }

    // Observer pattern: notify subscribers when stock is added or removed.
    public event Action<Product>? InventoryChanged;

    public void AddProduct(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        if (_inventory.ContainsKey(product.Code))
        {
            throw new InvalidOperationException("Product code already exists.");
        }

        _inventory.Add(product.Code, product);
        InventoryChanged?.Invoke(product);
    }

    public void Restock(string code, int quantity)
    {
        Product product = FindProduct(code);
        product.Restock(quantity);
        InventoryChanged?.Invoke(product);
    }

    public void InsertMoney(decimal amount, IPaymentMethod paymentMethod)
    {
        _state.InsertMoney(this, amount, paymentMethod);
    }

    public Product SelectProduct(string code)
    {
        return _state.SelectProduct(this, code);
    }

    // Return leftover money after a purchase, or refund a cancelled purchase.
    // This example calculates the amount; it assumes change is always available.
    public decimal ReturnChange()
    {
        decimal change = Balance;
        Balance = 0;
        _state = new IdleState();
        return change;
    }

    internal void AcceptPayment(decimal amount, IPaymentMethod paymentMethod)
    {
        ArgumentNullException.ThrowIfNull(paymentMethod);

        if (amount <= 0)
        {
            throw new ArgumentException("Payment must be positive.");
        }

        if (!paymentMethod.ProcessPayment(amount))
        {
            throw new InvalidOperationException("This payment amount is not accepted.");
        }

        Balance += amount;
        _state = new HasMoneyState();
    }

    internal Product Dispense(string code)
    {
        Product product = FindProduct(code);

        if (product.Quantity == 0)
        {
            throw new InvalidOperationException("Product is sold out.");
        }

        if (Balance < product.Price)
        {
            throw new InvalidOperationException("Insufficient funds. Insert more money.");
        }

        product.RemoveOne();
        Balance -= product.Price;
        if (Balance == 0)
        {
            _state = new IdleState();
        }

        InventoryChanged?.Invoke(product);
        return product;
    }

    private Product FindProduct(string code)
    {
        if (!_inventory.TryGetValue(code, out Product? product))
        {
            throw new InvalidOperationException("Product code was not found.");
        }

        return product;
    }
}

public class Program
{
    public static void Main()
    {
        VendingMachine machine = new VendingMachine();
        machine.InventoryChanged += ShowInventory;
        machine.AddProduct(ProductFactory.Create("A1", "Water", 1.50m, 2));
        machine.AddProduct(ProductFactory.Create("B1", "Chips", 2m, 1));

        IPaymentMethod coins = new CoinPayment();
        machine.InsertMoney(1m, coins);

        try
        {
            machine.SelectProduct("A1");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }

        // The first payment is still available after the failed selection.
        machine.InsertMoney(1m, coins);
        Product water = machine.SelectProduct("A1");
        Console.WriteLine($"Bought: {water.Name}");
        Console.WriteLine($"Change: {machine.ReturnChange():F2}");

        machine.InsertMoney(5m, new BanknotePayment());
        Product chips = machine.SelectProduct("B1");
        Console.WriteLine($"Bought: {chips.Name}");
        Console.WriteLine($"Change: {machine.ReturnChange():F2}");

        machine.Restock("B1", 2);

        // Cancelling before selecting a product returns the full balance.
        machine.InsertMoney(2m, coins);
        Console.WriteLine($"Refund: {machine.ReturnChange():F2}");
    }

    private static void ShowInventory(Product product)
    {
        Console.WriteLine($"{product.Code}: {product.Name}, stock: {product.Quantity}");
    }
}
