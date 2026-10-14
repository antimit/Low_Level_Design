using System;

namespace Shapes;

interface Shape
{
    double getArea();
    double getPerimeter();
};

class Circle : Shape
{
    public int _radius { get; set; }
    public Circle(int radius)
    {
        _radius = radius;
    }

    public double getArea()
    {
        return (3.14 * _radius * _radius);
    }

    public double getPerimeter()
    {
        return (2 * 3.14 * _radius);
    }
};

class Rectangle : Shape
{
    public int _height { get; set; }
    public int _width { get; set; }

    public Rectangle(int height, int width)
    {
        _height = height;
        _width = width;
    }

    public double getArea()
    {
        return (_height * _width);
    }

    public double getPerimeter()
    {
        return (2 * _width + 2 * _height);
    }
}


class Animal
{
     public virtual void Speak()
    {
        System.Console.WriteLine("Animal is speaking");
    }
    public virtual Animal clone()
    {
        return new Animal();
    }
}

class Dog : Animal
{
     public override void Speak()
    {
        System.Console.WriteLine("The Dog is speaking");
    }

    public override Dog clone()
    {
        return new Dog();
    }
}
