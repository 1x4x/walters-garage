namespace GarageApp;

public sealed class Garage
{
    private readonly Vehicle?[] _vehicles;

    public int Capacity => _vehicles.Length;
    public int VehicleCount => _vehicles.Count(vehicle => vehicle is not null);
    public int AvailableSpaces => Capacity - VehicleCount;

    public Garage(int capacity, IEnumerable<Vehicle>? startingVehicles = null)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Kapaciteten måste vara större än noll.");

        _vehicles = new Vehicle?[capacity];

        if (startingVehicles is null) return;

        foreach (Vehicle vehicle in startingVehicles)
        {
            if (!TryPark(vehicle, out string message))
                throw new ArgumentException($"Kunde inte lägga till startfordon: {message}", nameof(startingVehicles));
        }
    }

    public bool TryPark(Vehicle vehicle, out string message)
    {
        if (vehicle is null)
        {
            message = "Fordon saknas.";
            return false;
        }

        if (FindByRegistrationNumber(vehicle.RegistrationNumber) is not null)
        {
            message = $"Registreringsnumret {vehicle.RegistrationNumber} finns redan i garaget.";
            return false;
        }

        int freeIndex = Array.FindIndex(_vehicles, item => item is null);
        if (freeIndex < 0)
        {
            message = "Garaget är fullt.";
            return false;
        }

        _vehicles[freeIndex] = vehicle;
        message = $"{vehicle.RegistrationNumber} parkerades på plats {freeIndex + 1}.";
        return true;
    }

    public bool TryRemove(string registrationNumber, out Vehicle? removedVehicle, out string message)
    {
        removedVehicle = null;

        if (string.IsNullOrWhiteSpace(registrationNumber))
        {
            message = "Ange ett registreringsnummer.";
            return false;
        }

        for (int i = 0; i < _vehicles.Length; i++)
        {
            if (_vehicles[i] is not null &&
                string.Equals(_vehicles[i]!.RegistrationNumber, registrationNumber.Trim(),
                    StringComparison.OrdinalIgnoreCase))
            {
                removedVehicle = _vehicles[i];
                _vehicles[i] = null;
                message = $"Fordonet {removedVehicle.RegistrationNumber} hämtades ut.";
                return true;
            }
        }

        message = $"Hittade inget fordon med registreringsnummer {registrationNumber.Trim()}.";
        return false;
    }

    public Vehicle? FindByRegistrationNumber(string registrationNumber)
    {
        if (string.IsNullOrWhiteSpace(registrationNumber)) return null;

        return _vehicles.FirstOrDefault(vehicle =>
            vehicle is not null &&
            string.Equals(vehicle.RegistrationNumber, registrationNumber.Trim(),
                StringComparison.OrdinalIgnoreCase));
    }

    public Vehicle[] GetAllVehicles() => _vehicles.Where(vehicle => vehicle is not null)
        .Select(vehicle => vehicle!)
        .ToArray();

    public Vehicle[] Search(string? registrationNumber = null, string? color = null,
        int? numberOfWheels = null, Type? vehicleType = null)
    {
        return GetAllVehicles().Where(vehicle =>
            (string.IsNullOrWhiteSpace(registrationNumber) ||
             vehicle.RegistrationNumber.Contains(registrationNumber.Trim(), StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(color) ||
             string.Equals(vehicle.Color, color.Trim(), StringComparison.OrdinalIgnoreCase)) &&
            (!numberOfWheels.HasValue || vehicle.NumberOfWheels == numberOfWheels.Value) &&
            (vehicleType is null || vehicleType.IsInstanceOfType(vehicle)))
            .ToArray();
    }

    public Dictionary<string, int> GetVehicleCountsByType() =>
        GetAllVehicles()
            .GroupBy(vehicle => vehicle.VehicleType)
            .OrderBy(group => group.Key)
            .ToDictionary(group => group.Key, group => group.Count());
}
