using System;
using System.Collections.Generic;

public enum VehicleType
{
    Car,
    Motorcycle,
    Bus
}

public class Vehicle
{
    public string LicensePlate { get; }
    public VehicleType Type { get; }

    public Vehicle(string licensePlate, VehicleType type)
    {
        if (string.IsNullOrWhiteSpace(licensePlate))
        {
            throw new ArgumentException("License plate is required.");
        }

        LicensePlate = licensePlate.Trim();
        Type = type;
    }
}

// State pattern: the current state decides which transitions are allowed.
public interface ISpotState
{
    bool IsOccupied { get; }
    ISpotState Occupy();
    ISpotState Release();
}

public class AvailableState : ISpotState
{
    public bool IsOccupied => false;

    public ISpotState Occupy()
    {
        return new OccupiedState();
    }

    public ISpotState Release()
    {
        throw new InvalidOperationException("The spot is already available.");
    }
}

public class OccupiedState : ISpotState
{
    public bool IsOccupied => true;

    public ISpotState Occupy()
    {
        throw new InvalidOperationException("The spot is already occupied.");
    }

    public ISpotState Release()
    {
        return new AvailableState();
    }
}

public class ParkingSpot
{
    private ISpotState _state = new AvailableState();

    public string Id { get; }
    public VehicleType Type { get; }
    public decimal HourlyRate { get; }
    public bool IsOccupied => _state.IsOccupied;

    public ParkingSpot(string id, VehicleType type, decimal hourlyRate)
    {
        if (string.IsNullOrWhiteSpace(id) || hourlyRate <= 0)
        {
            throw new ArgumentException("A spot needs an ID and a positive rate.");
        }

        Id = id;
        Type = type;
        HourlyRate = hourlyRate;
    }

    public bool CanPark(Vehicle vehicle)
    {
        return !IsOccupied && Type == vehicle.Type;
    }

    // The parking lot changes spots and updates its tickets together.
    internal void Park(Vehicle vehicle)
    {
        if (Type != vehicle.Type)
        {
            throw new InvalidOperationException("Vehicle type does not match.");
        }

        _state = _state.Occupy();
    }

    internal void Release()
    {
        _state = _state.Release();
    }
}

// Strategy pattern: the parking lot delegates the choice of spot.
public interface ISpotAllocationStrategy
{
    ParkingSpot? FindSpot(IReadOnlyList<ParkingSpot> spots, Vehicle vehicle);
}

public class FirstAvailableSpotStrategy : ISpotAllocationStrategy
{
    public ParkingSpot? FindSpot(IReadOnlyList<ParkingSpot> spots, Vehicle vehicle)
    {
        foreach (ParkingSpot spot in spots)
        {
            if (spot.CanPark(vehicle))
            {
                return spot;
            }
        }

        return null;
    }
}

public class Ticket
{
    public string Id { get; }
    public Vehicle Vehicle { get; }
    public ParkingSpot Spot { get; }
    public DateTime EntryTime { get; }

    internal Ticket(string id, Vehicle vehicle, ParkingSpot spot, DateTime entryTime)
    {
        Id = id;
        Vehicle = vehicle;
        Spot = spot;
        EntryTime = entryTime;
    }

    public decimal CalculateFee(DateTime exitTime)
    {
        if (exitTime < EntryTime)
        {
            throw new ArgumentException("Exit time cannot be before entry time.");
        }

        // Charge for each started hour, with a minimum charge of one hour.
        double hours = Math.Max(1, Math.Ceiling((exitTime - EntryTime).TotalHours));
        return (decimal)hours * Spot.HourlyRate;
    }
}

// Simple factory: keep ticket creation and ID generation in one place.
public static class TicketFactory
{
    public static Ticket Create(Vehicle vehicle, ParkingSpot spot, DateTime entryTime)
    {
        return new Ticket(Guid.NewGuid().ToString(), vehicle, spot, entryTime);
    }
}

public class ParkingLot
{
    private readonly List<ParkingSpot> _spots;
    private readonly ISpotAllocationStrategy _allocationStrategy;
    private readonly Dictionary<string, Ticket> _activeTickets =
        new Dictionary<string, Ticket>();

    public int ParkedVehicleCount => _activeTickets.Count;

    // Observer pattern: subscribers receive a notification after a spot changes.
    public event Action<ParkingSpot>? SpotStatusChanged;

    public ParkingLot(List<ParkingSpot> spots, ISpotAllocationStrategy allocationStrategy)
    {
        _spots = new List<ParkingSpot>(spots);
        _allocationStrategy = allocationStrategy
            ?? throw new ArgumentNullException(nameof(allocationStrategy));
    }

    // Pass UTC times so the example is easy to run without waiting.
    public Ticket ParkVehicle(Vehicle vehicle, DateTime entryTime)
    {
        ArgumentNullException.ThrowIfNull(vehicle);

        foreach (Ticket activeTicket in _activeTickets.Values)
        {
            if (string.Equals(activeTicket.Vehicle.LicensePlate, vehicle.LicensePlate,
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("This vehicle is already parked.");
            }
        }

        ParkingSpot? spot = _allocationStrategy.FindSpot(_spots.AsReadOnly(), vehicle);
        if (spot == null)
        {
            throw new InvalidOperationException("No suitable parking spot is available.");
        }

        Ticket ticket = TicketFactory.Create(vehicle, spot, entryTime);
        spot.Park(vehicle);
        _activeTickets.Add(ticket.Id, ticket);
        SpotStatusChanged?.Invoke(spot);
        return ticket;
    }

    public decimal UnparkVehicle(string ticketId, DateTime exitTime)
    {
        if (!_activeTickets.TryGetValue(ticketId, out Ticket? ticket))
        {
            throw new InvalidOperationException("Active ticket was not found.");
        }

        decimal fee = ticket.CalculateFee(exitTime);
        ticket.Spot.Release();
        _activeTickets.Remove(ticketId);
        SpotStatusChanged?.Invoke(ticket.Spot);
        return fee;
    }
}

public class Program
{
    public static void Main()
    {
        List<ParkingSpot> spots = new List<ParkingSpot>
        {
            new ParkingSpot("C1", VehicleType.Car, 2m),
            new ParkingSpot("C2", VehicleType.Car, 2m),
            new ParkingSpot("M1", VehicleType.Motorcycle, 1m),
            new ParkingSpot("B1", VehicleType.Bus, 5m)
        };

        ParkingLot parkingLot = new ParkingLot(spots, new FirstAvailableSpotStrategy());
        parkingLot.SpotStatusChanged += ShowSpotStatus;

        DateTime entryTime = DateTime.UtcNow;
        Vehicle car = new Vehicle("ABC-123", VehicleType.Car);
        Ticket ticket = parkingLot.ParkVehicle(car, entryTime);

        Console.WriteLine($"Vehicle {ticket.Vehicle.LicensePlate} parked in {ticket.Spot.Id}.");
        Console.WriteLine($"Parked vehicles: {parkingLot.ParkedVehicleCount}");

        decimal fee = parkingLot.UnparkVehicle(ticket.Id, entryTime.AddMinutes(90));
        Console.WriteLine($"Fee for 90 minutes: {fee:F2}");
        Console.WriteLine($"Parked vehicles: {parkingLot.ParkedVehicleCount}");

        // The released spot can now be assigned to another car.
        Vehicle nextCar = new Vehicle("XYZ-789", VehicleType.Car);
        Ticket nextTicket = parkingLot.ParkVehicle(nextCar, entryTime.AddHours(2));
        Console.WriteLine($"Next car parked in {nextTicket.Spot.Id}.");
    }

    private static void ShowSpotStatus(ParkingSpot spot)
    {
        string status = spot.IsOccupied ? "occupied" : "available";
        Console.WriteLine($"Spot {spot.Id} is now {status}.");
    }
}
