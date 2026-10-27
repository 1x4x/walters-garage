namespace GarageApp;

public sealed class Airplane : Vehicle
{
    public override string VehicleType => "Airplane";
    public int NumberOfEngines { get; }

    public Airplane(string registrationNumber, string color, int numberOfWheels, int numberOfEngines)
        : base(registrationNumber, color, numberOfWheels)
    {
        if (numberOfEngines <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfEngines));
        NumberOfEngines = numberOfEngines;
    }

    public override string ToString() => $"{base.ToString()} | Motorer: {NumberOfEngines}";
}
