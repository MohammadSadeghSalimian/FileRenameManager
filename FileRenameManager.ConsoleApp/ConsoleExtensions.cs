namespace FileRenameManager.ConsoleApp;

public static class ConsoleExtensions
{
    public static int ReadInteger( string message, string? afterMessage)
    {
        while (true)
        {
            Console.WriteLine(message);
            var nText = Console.ReadLine();
            var res = int.TryParse(nText, out var n);
            if (res)
            {
                if (afterMessage != null)
                {
                    Console.WriteLine(afterMessage);
                }
                return n;
            }

            Console.WriteLine("Invalid input. Please enter a valid number.");
        }
    }
    public static string ReadText( string message, string? afterMessage)
    {
        while (true)
        {
            Console.WriteLine(message);
            var text = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(text))
            {
                Console.WriteLine("Invalid input. Please enter a valid text.");
                continue;
            }

            if (afterMessage != null)
            {
                Console.WriteLine(afterMessage);
            }
            return text;
        }
    }
    public static double ReadRealNumber( string message, string? afterMessage)
    {
        while (true)
        {
            Console.WriteLine(message);
            var nText = Console.ReadLine();
            var res = double.TryParse(nText, out var n);
            if (res)
            {
                if (afterMessage != null)
                {
                    Console.WriteLine(afterMessage);
                }
                return n;
            }
            Console.WriteLine("Invalid input. Please enter a valid number.");
        }
    }

    // New method: displays a message and numbered options, waits for user selection,
    // and returns the corresponding dictionary key.
    public static TKey ReadOption<TKey>( string message, IDictionary<TKey, string> options, string? afterMessage = null)
    {
        if (options == null || options.Count == 0)
            throw new System.ArgumentException("Options dictionary must contain at least one item.", nameof(options));

        // Create a stable list of keys to map numbers -> keys
        var keys = new System.Collections.Generic.List<TKey>();
        foreach (var k in options.Keys)
            keys.Add(k);

        while (true)
        {
            Console.WriteLine(message);
            // Display numbered options
            for (int i = 0; i < keys.Count; i++)
            {
                var key = keys[i];
                // Use the key to get display text (dictionary guarantees key exists)
                var display = options[key];
                Console.WriteLine($"{i + 1}. {display}");
            }

            Console.Write("> ");
            var input = Console.ReadLine();

            // Try numeric selection
            if (int.TryParse(input, out var selectedNumber))
            {
                if (selectedNumber >= 1 && selectedNumber <= keys.Count)
                {
                    var chosenKey = keys[selectedNumber - 1];
                    if (afterMessage != null)
                        Console.WriteLine(afterMessage);
                    return chosenKey;
                }
            }
    
            // Try matching by key.ToString() or by display text (case-insensitive)
            if (!string.IsNullOrWhiteSpace(input))
            {
                foreach (var kv in options)
                {
                    var keyString = kv.Key?.ToString();
                    if (!string.IsNullOrEmpty(keyString) && string.Equals(keyString, input, System.StringComparison.OrdinalIgnoreCase))
                    {
                        if (afterMessage != null)
                            Console.WriteLine(afterMessage);
                        return kv.Key;
                    }

                    if (!string.IsNullOrEmpty(kv.Value) && string.Equals(kv.Value, input, System.StringComparison.OrdinalIgnoreCase))
                    {
                        if (afterMessage != null)
                            Console.WriteLine(afterMessage);
                        return kv.Key;
                    }
                }
            }

            Console.WriteLine("Invalid selection. Please pick a valid option number, key, or option text.");
        }
    }

    public static bool ReadYesOrNo(string message, string? afterMessage)
    {
        while (true)
        {
            Console.WriteLine(message + " (y/n):");
            var input = Console.ReadLine();
            if (string.Equals(input, "y", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(input, "yes", StringComparison.OrdinalIgnoreCase))
            {
                if (afterMessage != null)
                {
                    Console.WriteLine(afterMessage);
                }
                return true;
            }

            if (string.Equals(input, "n", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(input, "no", StringComparison.OrdinalIgnoreCase))
            {
                if (afterMessage != null)
                {
                    Console.WriteLine(afterMessage);
                }
                return false;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
            }
        }
    }
    public static FileInfo OpenFile(string message, string? afterMessage)
    {
        while (true)
        {
            Console.WriteLine(message);
            var filePath = Console.ReadLine();
            if (File.Exists(filePath))
            {
                if (afterMessage != null)
                {
                    Console.WriteLine(afterMessage);
                }
                return new FileInfo(filePath);
            }
        }
            
    }
        
    public static FileInfo SaveFile(string message, string? afterMessage)
    {
        while (true)
        {
            Console.WriteLine(message);
            var filePath = Console.ReadLine();
            if (File.Exists(filePath))
            {
                   
                if (afterMessage != null)
                {
                    Console.WriteLine(afterMessage);
                }
                return new FileInfo(filePath);
            }
        }

    }

      
}