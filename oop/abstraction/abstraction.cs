using NotificationSystem;
class Program

{
    static void Main()
    {
        NotificationManager manager = new NotificationManager();

        manager.notifyAll("Hello", "World");
    }
}
