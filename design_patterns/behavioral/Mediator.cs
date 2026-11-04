
using System;
using System.Collections.Generic;

// 1. Mediator Interface
interface IChatMediator
{
    void SendMessage(string message, User sender);
    void AddUser(User user);
}

// 2. Concrete Mediator
class ChatRoom : IChatMediator
{
    private List<User> users = new List<User>();

    public void AddUser(User user)
    {
        users.Add(user);
    }

    public void SendMessage(string message, User sender)
    {
        foreach (User user in users)
        {
            if (user != sender)
            {
                user.Receive(message, sender.Name);
            }
        }
    }
}

// 3. Component
class User
{
    public string Name { get; private set; }
    private IChatMediator mediator;

    public User(string name, IChatMediator mediator)
    {
        Name = name;
        this.mediator = mediator;
    }

    public void Send(string message)
    {
        Console.WriteLine(Name + " sends: " + message);
        mediator.SendMessage(message, this);
    }

    public void Receive(string message, string senderName)
    {
        Console.WriteLine(Name + " received from "
            + senderName + ": " + message);
    }
}

// 4. Main Program
class Program
{
    static void Main()
    {
        IChatMediator chatRoom = new ChatRoom();

        User alice = new User("Alice", chatRoom);
        User bob = new User("Bob", chatRoom);
        User charlie = new User("Charlie", chatRoom);

        chatRoom.AddUser(alice);
        chatRoom.AddUser(bob);
        chatRoom.AddUser(charlie);

        alice.Send("Hello everyone!");
        Console.WriteLine();

        bob.Send("Hi Alice!");
    }
}
