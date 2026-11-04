
using System;

// 1. Request object
public class OrderRequest
{
    public string Username { get; set; } = "";
    public bool IsAuthenticated { get; set; }
    public bool CanPlaceOrder { get; set; }
}

// 2. Handler interface
public interface IHandler
{
    IHandler SetNext(IHandler handler);
    void Handle(OrderRequest request);
}

// 3. Base handler
public abstract class BaseHandler : IHandler
{
    private IHandler? _nextHandler;

    public IHandler SetNext(IHandler handler)
    {
        _nextHandler = handler;
        return handler;
    }

    public virtual void Handle(OrderRequest request)
    {
        _nextHandler?.Handle(request);
    }
}

// 4. Authentication handler
public class AuthenticationHandler : BaseHandler
{
    public override void Handle(OrderRequest request)
    {
        if (!request.IsAuthenticated)
        {
            Console.WriteLine("Authentication failed!");
            return;
        }

        Console.WriteLine("Authentication successful.");
        base.Handle(request);
    }
}

// 5. Authorization handler
public class AuthorizationHandler : BaseHandler
{
    public override void Handle(OrderRequest request)
    {
        if (!request.CanPlaceOrder)
        {
            Console.WriteLine("Access denied!");
            return;
        }

        Console.WriteLine("Authorization successful.");
        base.Handle(request);
    }
}

// 6. Order processing handler
public class OrderHandler : BaseHandler
{
    public override void Handle(OrderRequest request)
    {
        Console.WriteLine(
            $"Order placed successfully by {request.Username}!"
        );
    }
}

// 7. Client
public class Program
{
    public static void Main()
    {
        var authentication = new AuthenticationHandler();
        var authorization = new AuthorizationHandler();
        var order = new OrderHandler();

        // Build the chain
        authentication.SetNext(authorization)
                      .SetNext(order);

        // Example 1: Valid request
        Console.WriteLine("Example 1:");

        var request1 = new OrderRequest
        {
            Username = "John",
            IsAuthenticated = true,
            CanPlaceOrder = true
        };

        authentication.Handle(request1);

        // Example 2: Invalid request
        Console.WriteLine("\nExample 2:");

        var request2 = new OrderRequest
        {
            Username = "Alice",
            IsAuthenticated = false,
            CanPlaceOrder = true
        };

        authentication.Handle(request2);
    }
}
