
using System;
using System.Collections.Generic;
using System.Xml.Linq;

// 1. ELEMENT INTERFACE
interface IShape
{
    void Accept(IShapeVisitor visitor);
}

// 2. VISITOR INTERFACE
interface IShapeVisitor
{
    void Visit(Circle circle);
    void Visit(Rectangle rectangle);
}

// 3. CONCRETE ELEMENTS
class Circle : IShape
{
    public double Radius { get; }

    public Circle(double radius)
    {
        Radius = radius;
    }

    public void Accept(IShapeVisitor visitor)
    {
        visitor.Visit(this);
    }
}

class Rectangle : IShape
{
    public double Width { get; }
    public double Height { get; }

    public Rectangle(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public void Accept(IShapeVisitor visitor)
    {
        visitor.Visit(this);
    }
}

// 4. CONCRETE VISITOR: XML EXPORT
class XmlExportVisitor : IShapeVisitor
{
    private readonly List<XElement> _elements = new();

    public void Visit(Circle circle)
    {
        _elements.Add(new XElement("Circle",
            new XAttribute("Radius", circle.Radius)));
    }

    public void Visit(Rectangle rectangle)
    {
        _elements.Add(new XElement("Rectangle",
            new XAttribute("Width", rectangle.Width),
            new XAttribute("Height", rectangle.Height)));
    }

    public string GetXml()
    {
        return new XElement("Shapes", _elements).ToString();
    }
}

// 5. CONCRETE VISITOR: AREA CALCULATION
class AreaVisitor : IShapeVisitor
{
    public double TotalArea { get; private set; }

    public void Visit(Circle circle)
    {
        TotalArea += Math.PI * circle.Radius * circle.Radius;
    }

    public void Visit(Rectangle rectangle)
    {
        TotalArea += rectangle.Width * rectangle.Height;
    }
}

// 6. CLIENT
class Program
{
    static void Main()
    {
        List<IShape> shapes = new()
        {
            new Circle(2),
            new Rectangle(3, 4)
        };

        // Operation 1: Export to XML
        var xmlVisitor = new XmlExportVisitor();

        foreach (IShape shape in shapes)
        {
            shape.Accept(xmlVisitor);
        }

        Console.WriteLine("XML export:");
        Console.WriteLine(xmlVisitor.GetXml());

        // Operation 2: Calculate area
        var areaVisitor = new AreaVisitor();

        foreach (IShape shape in shapes)
        {
            shape.Accept(areaVisitor);
        }

        Console.WriteLine(
            $"Total area: {areaVisitor.TotalArea:F2}");
    }
}
