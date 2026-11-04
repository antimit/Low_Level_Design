
using System;
using System.Collections.Generic;

// 1. MEMENTO - Stores a snapshot
class Memento
{
    public string Text { get; }

    public Memento(string text)
    {
        Text = text;
    }
}

// 2. ORIGINATOR - Creates and restores snapshots
class Editor
{
    private string text = "";

    public void SetText(string newText)
    {
        text = newText;
    }

    public void ShowText()
    {
        Console.WriteLine("Current text: " + text);
    }

    public Memento Save()
    {
        return new Memento(text);
    }

    public void Restore(Memento memento)
    {
        text = memento.Text;
    }
}

// 3. CARETAKER - Manages history
class History
{
    private Stack<Memento> snapshots = new Stack<Memento>();

    public void Push(Memento memento)
    {
        snapshots.Push(memento);
    }

    public Memento Pop()
    {
        return snapshots.Pop();
    }
}

// 4. CLIENT
class Program
{
    static void Main()
    {
        Editor editor = new Editor();
        History history = new History();

        editor.SetText("Hello");
        editor.ShowText();

        // Save state before changing text
        history.Push(editor.Save());

        editor.SetText("Hello World");
        editor.ShowText();

        // Save again
        history.Push(editor.Save());

        editor.SetText("Hello World!!!");
        editor.ShowText();

        // First Undo
        editor.Restore(history.Pop());
        editor.ShowText();

        // Second Undo
        editor.Restore(history.Pop());
        editor.ShowText();
    }
}
