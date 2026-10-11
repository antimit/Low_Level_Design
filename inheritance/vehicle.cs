namespace Vehicles;

abstract class Vehicle
{
    public string brand { get; set; }
    public string model { get; set; }

    public abstract void drive();

    public abstract void stop();
};



class Car : Vehicle
{
    public string brand { get; set; }
    public string model { get; set; }

    public override void drive()
    {
        System.Console.WriteLine("I don't know how fast I was going but  I was fast");
    }

    public override void stop()
    {
        System.Console.WriteLine("I don't know how or what stopped me");
    }
};


class Truck : Vehicle
{
    public string brand { get; set; }
    public string model { get; set; }

    public override void drive()
    {
        System.Console.WriteLine("I don't know how slow I was going but  I definetely not hot!");
    }

    public override void stop()
    {
        System.Console.WriteLine("I think I stopped some cars behind me, I am BIIIIIG!");
    }
};

class BMW_X7_Pick_Up : Car, Truck
{

};