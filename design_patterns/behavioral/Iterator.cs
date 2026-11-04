
using System;
using System.Collections.Generic;

// 1. Iterator interface
interface IIterator
{
    bool HasNext();
    string Next();
}

// 2. Concrete Iterator
class StudentIterator : IIterator
{
    private List<string> students;
    private int position = 0;

    public StudentIterator(List<string> students)
    {
        this.students = students;
    }

    public bool HasNext()
    {
        return position < students.Count;
    }

    public string Next()
    {
        if (!HasNext())
            throw new InvalidOperationException("No more students.");

        return students[position++];
    }
}

// 3. Collection
class StudentCollection
{
    private List<string> students = new List<string>();

    public void AddStudent(string name)
    {
        students.Add(name);
    }

    public IIterator CreateIterator()
    {
        return new StudentIterator(students);
    }
}

// 4. Client
class Program
{
    static void Main()
    {
        StudentCollection collection = new StudentCollection();

        collection.AddStudent("Alice");
        collection.AddStudent("Bob");
        collection.AddStudent("Charlie");

        IIterator iterator = collection.CreateIterator();

        while (iterator.HasNext())
        {
            Console.WriteLine(iterator.Next());
        }
    }
}
