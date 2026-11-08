
using System;

// ========================================
// 1. STATE INTERFACE
// ========================================

public interface IPlayerState
{
    string Name { get; }

    void Play();
    void Lock();
    void Next();
}

// ========================================
// 2. CONTEXT
// ========================================

public class MusicPlayer
{
    private readonly string[] _songs =
    {
        "Song A",
        "Song B",
        "Song C"
    };

    private int _songIndex = 0;

    public IPlayerState CurrentState { get; private set; }

    public string CurrentSong => _songs[_songIndex];

    public MusicPlayer()
    {
        CurrentState = new ReadyState(this);
    }

    // Change the current state
    public void ChangeState(IPlayerState newState)
    {
        Console.WriteLine(
            $"STATE: {CurrentState.Name} -> {newState.Name}");

        CurrentState = newState;
    }

    // Delegate actions to the current state
    public void PressPlay()
    {
        CurrentState.Play();
    }

    public void PressLock()
    {
        CurrentState.Lock();
    }

    public void PressNext()
    {
        CurrentState.Next();
    }

    // Operations used by concrete states
    public void StartPlayback()
    {
        Console.WriteLine($"Playing: {CurrentSong}");
    }

    public void StopPlayback()
    {
        Console.WriteLine("Playback stopped.");
    }

    public void NextSong()
    {
        _songIndex = (_songIndex + 1) % _songs.Length;
        Console.WriteLine($"Selected: {CurrentSong}");
    }
}

// ========================================
// 3. CONCRETE STATE: READY
// ========================================

public class ReadyState : IPlayerState
{
    private readonly MusicPlayer _player;

    public string Name => "Ready";

    public ReadyState(MusicPlayer player)
    {
        _player = player;
    }

    public void Play()
    {
        _player.StartPlayback();

        _player.ChangeState(
            new PlayingState(_player));
    }

    public void Lock()
    {
        _player.ChangeState(
            new LockedState(_player, this));
    }

    public void Next()
    {
        _player.NextSong();
    }
}

// ========================================
// 4. CONCRETE STATE: PLAYING
// ========================================

public class PlayingState : IPlayerState
{
    private readonly MusicPlayer _player;

    public string Name => "Playing";

    public PlayingState(MusicPlayer player)
    {
        _player = player;
    }

    public void Play()
    {
        _player.StopPlayback();

        _player.ChangeState(
            new ReadyState(_player));
    }

    public void Lock()
    {
        _player.ChangeState(
            new LockedState(_player, this));
    }

    public void Next()
    {
        _player.NextSong();
        _player.StartPlayback();
    }
}

// ========================================
// 5. CONCRETE STATE: LOCKED
// ========================================

public class LockedState : IPlayerState
{
    private readonly MusicPlayer _player;
    private readonly IPlayerState _previousState;

    public string Name => "Locked";

    public LockedState(
        MusicPlayer player,
        IPlayerState previousState)
    {
        _player = player;
        _previousState = previousState;
    }

    public void Play()
    {
        Console.WriteLine(
            "Player is locked. Play ignored.");
    }

    public void Lock()
    {
        Console.WriteLine("Unlocking player...");

        _player.ChangeState(_previousState);
    }

    public void Next()
    {
        Console.WriteLine(
            "Player is locked. Next ignored.");
    }
}

// ========================================
// 6. CLIENT / CONSOLE APPLICATION
// ========================================

public class Program
{
    public static void Main()
    {
        MusicPlayer player = new MusicPlayer();

        Console.WriteLine("=== MUSIC PLAYER ===");
        Console.WriteLine("Commands: play, next, lock, exit");

        while (true)
        {
            Console.WriteLine();
            Console.Write(
                $"[{player.CurrentState.Name}] Enter command: ");

            string? command =
                Console.ReadLine()?.Trim().ToLowerInvariant();

            switch (command)
            {
                case "play":
                    player.PressPlay();
                    break;

                case "next":
                    player.PressNext();
                    break;

                case "lock":
                    player.PressLock();
                    break;

                case "exit":
                    return;

                default:
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }
    }
}
