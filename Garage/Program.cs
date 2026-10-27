using System.Diagnostics.Metrics;

namespace GarageApp;

internal static class Program
{
    private static Garage? _garage;

    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("--------------------------------------");
        Console.WriteLine("  Välkommen till Walters Garage v1.0  ");
        Console.WriteLine("--------------------------------------");
        _garage = CreateGarage();

        bool running = true;
        while (running)
        {
            Console.WriteLine();
            Console.WriteLine($"Garage: {_garage.VehicleCount}/{_garage.Capacity} fordon | {_garage.AvailableSpaces} lediga platser");
            Console.WriteLine("(1) Parkera fordon");
            Console.WriteLine("(2) Hämta ut fordon");
            Console.WriteLine("(3) Lista alla fordon");
            Console.WriteLine("(4) Visa antal per fordonstyp");
            Console.WriteLine("(5) Sök via registreringsnummer");
            Console.WriteLine("(6) Sök via egenskaper");
            Console.WriteLine("(7) Visa lediga platser");
            Console.WriteLine("(0) Avsluta");
            string choice = ReadText("Välj: ");

            switch (choice)
            {
                case "1": ParkVehicle(); break;
                case "2": RemoveVehicle(); break;
                case "3": ListVehicles(); break;
                case "4": ShowCounts(); break;
                case "5": FindVehicle(); break;
                case "6": SearchVehicles(); break;
                case "7": Console.WriteLine($"Lediga platser: {_garage.AvailableSpaces} av {_garage.Capacity}"); break;
                case "0": running = false; Console.WriteLine("Programmet avslutas. Hej då!"); break;
                default: Console.WriteLine("Ogiltigt menyval. Välj ett av alternativen."); break;
            }
        }
    }

    private static Garage CreateGarage()
    {
        int capacity = ReadInt("Ange garagets kapacitet (1-100 fordon): ", 1, 100);
        Garage garage = new(capacity);

        Console.Write("Vill du lägga in exempel-fordon vid start? (j/n): ");
        if (ReadText("").Equals("j", StringComparison.OrdinalIgnoreCase))
        {
            Vehicle[] examples =
            {
                new Car("ABC123", "Svart", 4, 2000, "Bensin"),
                new Motorcycle("MOT456", "Röd", 2, 650),
                new Airplane("AIR789", "Vit", 6, 2),
                new Bus("BUS321", "Blå", 6, 50),
                new Boat("BOA654", "Vit", 0, 7.5)
            };

            foreach (Vehicle vehicle in examples)
            {
                if (garage.AvailableSpaces == 0) break;
                garage.TryPark(vehicle, out _);
            }
            Console.WriteLine($"Exempeldata inlagd. Antal fordon: {garage.VehicleCount}.");
        }

        return garage;
    }

    private static void ParkVehicle()
    {
        if (_garage!.AvailableSpaces == 0)
        {
            Console.WriteLine("Garaget är fullt. Ta ut ett fordon innan du parkerar ett nytt.");
            return;
        }

        Console.WriteLine("Välj fordonstyp: 1. Bil  2. Motorcykel  3. Flygplan  4. Buss  5. Båt");
        string type = ReadText("Typ: ");
        string registration = ReadRequired("Registreringsnummer: ");

        if (_garage.FindByRegistrationNumber(registration) is not null)
        {
            Console.WriteLine("Det registreringsnumret finns redan i garaget.");
            return;
        }

        string color = ReadRequired("Färg: ");
        int wheels = ReadInt("Antal hjul: ", 0, 100);
        Vehicle? vehicle = null;

        try
        {
            switch (type)
            {
                case "1":
                    int cc = ReadInt("Cylindervolym (cc): ", 1, 20000);
                    string fuel = ReadFuelType();
                    vehicle = new Car(registration, color, wheels, cc, fuel);
                    break;
                case "2":
                    int motorcycleCc = ReadInt("Cylindervolym (cc): ", 1, 20000);
                    vehicle = new Motorcycle(registration, color, wheels, motorcycleCc);
                    break;
                case "3":
                    int engines = ReadInt("Antal motorer: ", 1, 20);
                    vehicle = new Airplane(registration, color, wheels, engines);
                    break;
                case "4":
                    int seats = ReadInt("Antal sittplatser: ", 1, 500);
                    vehicle = new Bus(registration, color, wheels, seats);
                    break;
                case "5":
                    double length = ReadDouble("Båtens längd i meter: ", 0.1, 1000);
                    vehicle = new Boat(registration, color, wheels, length);
                    break;
                default:
                    Console.WriteLine("Okänd fordonstyp.");
                    return;
            }

            if (_garage.TryPark(vehicle, out string message)) Console.WriteLine("KLART: " + message);
            else Console.WriteLine("KUNDE INTE PARKERA: " + message);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("Ogiltiga fordonsuppgifter: " + ex.Message);
        }
    }

    private static void RemoveVehicle()
    {
        string registration = ReadRequired("Ange registreringsnummer för fordonet som ska hämtas: ");
        bool success = _garage!.TryRemove(registration, out _, out string message);
        Console.WriteLine((success ? "KLART: " : "MISSLYCKADES: ") + message);
    }

    private static void ListVehicles()
    {
        Vehicle[] vehicles = _garage!.GetAllVehicles();
        if (vehicles.Length == 0)
        {
            Console.WriteLine("Garaget är tomt.");
            return;
        }

        Console.WriteLine("--- Parkerade fordon ---");
        foreach (Vehicle vehicle in vehicles) Console.WriteLine(vehicle);
    }

    private static void ShowCounts()
    {
        Dictionary<string, int> counts = _garage!.GetVehicleCountsByType();
        if (counts.Count == 0)
        {
            Console.WriteLine("Garaget är tomt.");
            return;
        }

        Console.WriteLine("--- Fordon per typ ---");
        foreach (KeyValuePair<string, int> item in counts)
            Console.WriteLine($"{item.Key}: {item.Value}");
    }

    private static void FindVehicle()
    {
        string registration = ReadRequired("Registreringsnummer: ");
        Vehicle? vehicle = _garage!.FindByRegistrationNumber(registration);
        Console.WriteLine(vehicle is null ? "Inget fordon hittades." : vehicle.ToString());
    }

    private static void SearchVehicles()
    {
        Console.WriteLine("Lämna ett fält tomt för att inte filtrera på det.");
        string color = ReadText("Färg: ");
        string wheelsInput = ReadText("Antal hjul: ");
        int? wheels = null;

        if (!string.IsNullOrWhiteSpace(wheelsInput))
        {
            if (!int.TryParse(wheelsInput, out int parsedWheels) || parsedWheels < 0)
            {
                Console.WriteLine("Antal hjul måste vara ett heltal som är 0 eller större.");
                return;
            }
            wheels = parsedWheels;
        }

        Console.WriteLine("Fordonstyp: 1. Alla  2. Bil  3. Motorcykel  4. Flygplan  5. Buss  6. Båt");
        string typeChoice = ReadText("Typ: ");
        Type? type = typeChoice switch
        {
            "1" or "" => null,
            "2" => typeof(Car),
            "3" => typeof(Motorcycle),
            "4" => typeof(Airplane),
            "5" => typeof(Bus),
            "6" => typeof(Boat),
            _ => typeof(void)
        };

        if (type == typeof(void))
        {
            Console.WriteLine("Ogiltig fordonstyp.");
            return;
        }

        Vehicle[] results = _garage!.Search(color: color, numberOfWheels: wheels, vehicleType: type);
        if (results.Length == 0) Console.WriteLine("Inga fordon matchade sökningen.");
        else
        {
            Console.WriteLine($"Hittade {results.Length} fordon:");
            foreach (Vehicle vehicle in results) Console.WriteLine(vehicle);
        }
    }

    private static string ReadFuelType()
    {
        while (true)
        {
            string fuel = ReadText("Bränsletyp (Bensin/Diesel/El/Hybrid): ");
            if (new[] { "bensin", "diesel", "el", "hybrid" }
                .Contains(fuel, StringComparer.OrdinalIgnoreCase))
                return fuel;
            Console.WriteLine("Välj Bensin, Diesel, El eller Hybrid.");
        }
    }

    private static string ReadRequired(string prompt)
    {
        while (true)
        {
            string value = ReadText(prompt);
            if (!string.IsNullOrWhiteSpace(value)) return value.Trim();
            Console.WriteLine("Värdet får inte vara tomt.");
        }
    }

    private static string ReadText(string prompt)
    {
        if (prompt.Length > 0) Console.Write(prompt);
        return Console.ReadLine() ?? "";
    }

    private static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            string input = ReadText(prompt);
            if (int.TryParse(input, out int value) && value >= min && value <= max)
                return value;
            Console.WriteLine($"Ange ett heltal mellan {min} och {max}.");
        }
    }

    private static double ReadDouble(string prompt, double min, double max)
    {
        while (true)
        {
            string input = ReadText(prompt).Replace(',', '.');
            if (double.TryParse(input, System.Globalization.NumberStyles.AllowDecimalPoint,
                    System.Globalization.CultureInfo.InvariantCulture, out double value) &&
                value >= min && value <= max)
                return value;
            Console.WriteLine($"Ange ett tal mellan {min} och {max}.");
        }
    }
}
