using System;
using Vehicles;

class Program
{
    static void Main()
    {
        Vehicle bmw = new Car();
        bmw.drive();

        Vehicle F150 = new Truck();
        F150.drive();

        //C# doesn't suppport multiple inheritance which have common anscestor 
        // Vehicle doesnt_exist = new BMW_X7_Pick_Up();
        // doesnt_exist.drive();
    }
}
