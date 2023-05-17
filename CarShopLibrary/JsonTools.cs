using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CarShopLibrary
{
    public class JsonTools : IDataTools
    {
        const string fileName = "parco-auto.json";

        public bool SalvaDati(List<Veicolo> dati)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize<List<Veicolo>>(dati, options);
                // Console.WriteLine(jsonString);
                File.WriteAllText(fileName, jsonString);
                return true;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return false;
            }
        }

        public List<Veicolo> CaricaDati()
        {
            try
            {
                string jsonString = File.ReadAllText(fileName);
                return JsonSerializer.Deserialize<List<Veicolo>>(jsonString);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return null;
            }
        }
    }
}
