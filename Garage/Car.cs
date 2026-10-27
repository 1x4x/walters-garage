namespace GarageApp;

public sealed class Car : Vehicle
{
    public override string VehicleType => "Car";
    public int CylinderVolume { get; }
    public string FuelType { get; }

    public Car(string registrationNumber, string color, int numberOfWheels,
        int cylinderVolume, string fuelType)
        : base(registrationNumber, color, numberOfWheels)
    {
        if (cylinderVolume <= 0) throw new ArgumentOutOfRangeException(nameof(cylinderVolume));
        if (string.IsNullOrWhiteSpace(fuelType)) throw new ArgumentException("Bränsletyp krävs.");
        CylinderVolume = cylinderVolume;
        FuelType = fuelType.Trim();
    }

    public override string ToString() => $"{base.ToString()} | Motor: {CylinderVolume} cc | Bränsle: {FuelType}";
}
