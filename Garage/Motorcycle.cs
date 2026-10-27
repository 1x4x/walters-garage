namespace GarageApp;

public sealed class Motorcycle : Vehicle
{
    public override string VehicleType => "Motorcycle";
    public int CylinderVolume { get; }

    public Motorcycle(string registrationNumber, string color, int numberOfWheels, int cylinderVolume)
        : base(registrationNumber, color, numberOfWheels)
    {
        if (cylinderVolume <= 0) throw new ArgumentOutOfRangeException(nameof(cylinderVolume));
        CylinderVolume = cylinderVolume;
    }

    public override string ToString() => $"{base.ToString()} | Motor: {CylinderVolume} cc";
}
