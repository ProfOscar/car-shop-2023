﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarShopLibrary
{
    public abstract class Veicolo
    {
        public string Marca { get; set; }
        public string Modello { get; set; }

        public Veicolo(string marca, string modello)
        {
            Marca = marca;
            Modello = modello;
        }
    }
}
