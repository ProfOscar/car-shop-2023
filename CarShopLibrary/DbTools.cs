using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarShopLibrary
{
    public class DbTools : IDataTools
    {
        const string connStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\oscar.cambieri\Desktop\car-shop-2023\CarShopLibrary\ParcoMezzi.mdf;Integrated Security=True";

        public List<Veicolo> CaricaDati()
        {
            List<Veicolo> veicoli = new List<Veicolo>();
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                String sql = "SELECT * FROM Veicoli";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Console.WriteLine("{0} {1} {2} {3}", reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetString(3));
                            int tipoVeicolo = reader.GetInt32(1);
                            string marca = reader.GetString(2);
                            string modello = reader["Modello"].ToString();
                            EAlimentazione alimentazione = (EAlimentazione) (reader.GetInt32(4) - 1);
                            string colore = reader["Colore"].ToString();
                            int lung = Convert.ToInt32(reader["Lunghezza"]);
                            int larg = Convert.ToInt32(reader["Larghezza"]);
                            int alt = Convert.ToInt32(reader["Altezza"]);
                            StructDimensioni dimensioni = new StructDimensioni(lung, larg, alt);
                            string vin = reader["VIN"].ToString();
                            int km = Convert.ToInt32(reader["KM"]);
                            int maxSpeed = Convert.ToInt32(reader["VelocitaMassima"]);
                            int potenza = Convert.ToInt32(reader["Potenza"]);
                            DateTime dataImmatricolazione = new DateTime();
                            int prezzo = Convert.ToInt32(reader["Prezzo"]);
                            string immagine = reader["Immagine"].ToString();
                            switch (tipoVeicolo)
                            {
                                case 1:
                                    // auto
                                    Auto a = new Auto(marca, modello, alimentazione, colore, dimensioni, vin, km, maxSpeed, potenza, dataImmatricolazione, prezzo, immagine, false, 0, 0);
                                    veicoli.Add(a);
                                    break;
                                case 2:
                                    // moto
                                    Moto m = new Moto(marca, modello, alimentazione, colore, dimensioni, vin, km, maxSpeed, potenza, dataImmatricolazione, prezzo, immagine, ETipoMoto.Undefined, 0);
                                    veicoli.Add(m);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            return veicoli;
        }

        public bool SalvaDati(List<Veicolo> dati)
        {
            throw new NotImplementedException();
        }
    }
}
