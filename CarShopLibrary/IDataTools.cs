using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarShopLibrary
{
    public interface IDataTools
    {
        bool SalvaDati(List<Veicolo> dati);

        List<Veicolo> CaricaDati();
    }
}
