using System;
using System.Collections.Generic;
using System.Text;

// Both files and directories have a name and a size.
public abstract class FileSystemNode
{
    public string Name { get; }

    protected FileSystemNode(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Contains('/') ||
            name == "." || name == "..")
        {
            throw new ArgumentException("Invalid name.");
        }

        Name = name;
    }

    public abstract long GetSize();
}

// A file stores text and lets us read or replace it.
public class FileNode : FileSystemNode
{
    private string _content;

    public FileNode(string name, string content) : base(name)
    {
        _content = content ?? throw new ArgumentNullException(nameof(content));
    }

    public string Read()
    {
        return _content;
    }

    public void Write(string content)
    {
        _content = content ?? throw new ArgumentNullException(nameof(content));
    }

    public override long GetSize()
    {
        return Encoding.UTF8.GetByteCount(_content);
    }
}

// A directory stores files and other directories in the same list.
public class Directory : FileSystemNode
{
    private readonly List<FileSystemNode> _children = new List<FileSystemNode>();

    public IReadOnlyList<FileSystemNode> Children => _children.AsReadOnly();

    public Directory(string name) : base(name)
    {
    }

    // Create children here so a directory cannot contain itself or an ancestor.
    public Directory AddDirectory(string name)
    {
        Directory directory = new Directory(name);
        AddNode(directory);
        return directory;
    }

    public FileNode AddFile(string name, string content)
    {
        FileNode file = new FileNode(name, content);
        AddNode(file);
        return file;
    }

    private void AddNode(FileSystemNode node)
    {
        if (Find(node.Name) != null)
        {
            throw new InvalidOperationException("That name already exists.");
        }

        _children.Add(node);
    }

    // Search only the immediate children of this directory.
    public FileSystemNode? Find(string name)
    {
        foreach (FileSystemNode child in _children)
        {
            if (child.Name == name)
            {
                return child;
            }
        }

        return null;
    }

    public void RemoveNode(string name)
    {
        FileSystemNode? node = Find(name);
        if (node == null)
        {
            throw new InvalidOperationException("Node was not found.");
        }

        _children.Remove(node);
    }

    public override long GetSize()
    {
        long totalSize = 0;
        foreach (FileSystemNode child in _children)
        {
            // For a directory, this call adds up its children recursively.
            totalSize += child.GetSize();
        }

        return totalSize;
    }
}

public class Program
{
    public static void Main()
    {
        Directory root = new Directory("root");
        Directory documents = root.AddDirectory("Documents");
        FileNode notes = documents.AddFile("notes.txt", "Hello");
        root.AddFile("readme.txt", "Welcome");

        Console.WriteLine(notes.Read());
        notes.Write("Hello World!");
        Console.WriteLine(notes.Read());
        Console.WriteLine($"Found: {documents.Find("notes.txt")?.Name}");
        Console.WriteLine($"Total size: {root.GetSize()} bytes");
        PrintTree(root);

        documents.RemoveNode("notes.txt");
        Console.WriteLine($"Size after deleting notes: {root.GetSize()} bytes");
    }

    private static void PrintTree(FileSystemNode node, string indent = "")
    {
        Console.WriteLine(indent + node.Name);

        if (node is Directory directory)
        {
            foreach (FileSystemNode child in directory.Children)
            {
                PrintTree(child, indent + "  ");
            }
        }
    }
}
