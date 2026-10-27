using System;

public abstract class Vehicle
{
    public string RegistrationNumber { get; private set; }
    public string Color { get; set; }
    public int NumberOfWheels { get; set; }

    public abstract string VehicleType { get; }

    protected Vehicle(string registrationNumber, string color, int numberOfWheels)
    {
        RegistrationNumber = registrationNumber;
        Color = color;
        NumberOfWheels = numberOfWheels;
    }

    public override string ToString()
    {
        return $"Typ: {VehicleType}, Regnummer: {RegistrationNumber}, " +
               $"Färg: {Color}, Hjul: {NumberOfWheels}";
    }
}
