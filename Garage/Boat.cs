namespace GarageApp;

public sealed class Boat : Vehicle
{
    public override string VehicleType => "Boat";
    public double Length { get; }

    public Boat(string registrationNumber, string color, int numberOfWheels, double length)
        : base(registrationNumber, color, numberOfWheels)
    {
        if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));
        Length = length;
    }

    public override string ToString() => $"{base.ToString()} | Längd: {Length:0.##} m";
}
