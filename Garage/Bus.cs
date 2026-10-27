namespace GarageApp;

public sealed class Bus : Vehicle
{
    public override string VehicleType => "Bus";
    public int NumberOfSeats { get; }

    public Bus(string registrationNumber, string color, int numberOfWheels, int numberOfSeats)
        : base(registrationNumber, color, numberOfWheels)
    {
        if (numberOfSeats <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfSeats));
        NumberOfSeats = numberOfSeats;
    }

    public override string ToString() => $"{base.ToString()} | Säten: {NumberOfSeats}";
}
