using Shapes;

class Program
{
    static void Main()
    {
        // Shape circle = new Circle(10);

        // System.Console.WriteLine(circle.getPerimeter());


        // Shape rectangle = new Rectangle(50, 40);

        // System.Console.WriteLine(rectangle.getPerimeter());

        Animal animal = new Dog();

        Dog dog = (Dog)animal.clone();

        dog.Speak();
    }
}
