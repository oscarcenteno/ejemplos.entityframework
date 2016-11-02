﻿using System;
using System.Linq;

namespace Ejemplos.EntityFramework.ConsoleApplication.Cuentas
{
    public static class Escenario
    {
        public static void EjecuteEscenarioDeAgregar()
        {
            Console.WriteLine(string.Empty);
            Console.WriteLine("** AGREGAR **");

            Console.Write("Escriba el nombre para la nueva Cuenta: ");
            string elNombre = Console.ReadLine();

            Console.Write("Escriba el Id de la Entidad: ");
            string elIdDeEntidad = Console.ReadLine();
            int elIdEntidadComoNumero = int.Parse(elIdDeEntidad);

            Console.Write("Escriba el Id de la Moneda: ");
            string elIdDeMoneda = Console.ReadLine();
            int elIdMonedaComoNumero = int.Parse(elIdDeMoneda);

            AgregueUnaCuenta(elNombre, elIdEntidadComoNumero, elIdMonedaComoNumero);

            ImprimaTodasLasCuentasEHistoricos();
        }

        public static void ImprimaTodasLasCuentasEHistoricos()
        {
            Trace.WriteLine($"Consultanto todas las cuentas y sus historicos");

            using (var db = new CuentasContext())
            {
                var lasCuentas = from cadaCuenta in db.Cuentas
                                 select cadaCuenta;

                Console.WriteLine(string.Empty);
                Console.WriteLine("CONSULTANDO todas las cuentas y sus historicos:");

                foreach (var unaCuenta in lasCuentas)
                {
                    Console.WriteLine(unaCuenta.IdEntidad + " " + unaCuenta.IdMoneda);

                    var losHistoricos = from cadaHistorico in unaCuenta.Historicos
                                        select cadaHistorico;
                    foreach (var unHistorico in losHistoricos)
                    {
                        Console.WriteLine(" - " + unHistorico.Nombre + " " + unHistorico.Estado + " " + unHistorico.FechaDeModificacion);
                    }
                }
            }

            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();
        }

        public static void AgregueUnaCuenta(string elNombre, int elIdEntidadComoNumero, int elIdMonedaComoNumero)
        {
            Trace.WriteLine($"Agregando la cuenta IdEntidad: {elIdEntidadComoNumero} IdMoneda: {elIdMonedaComoNumero} Nombre: {elNombre}");

            using (var db = new CuentasContext())
            {
                var laCuenta = new Cuenta
                {
                    IdEntidad = elIdEntidadComoNumero,
                    IdMoneda = elIdMonedaComoNumero
                };

                var elHistorico = new Historico
                {
                    Nombre = elNombre,
                    FechaDeModificacion = DateTime.Now,
                    Estado = 1
                };

                laCuenta.Agregue(elHistorico);
                db.Cuentas.Add(laCuenta);

                db.SaveChanges();
            }
        }

        public static void EjecuteEscenarioDeEditar()
        {
            Console.WriteLine(string.Empty);
            Console.WriteLine("** EDITAR **");
            Console.Write("Escriba el Id de la Entidad: ");
            string elIdDeEntidad = Console.ReadLine();
            int elIdDeEntidadComoNumero = int.Parse(elIdDeEntidad);

            Console.Write("Escriba el Id de la Moneda: ");
            string elIdMoneda = Console.ReadLine();
            int elIdMonedaComoNumero = int.Parse(elIdMoneda);

            Console.Write("Escriba el nuevo nombre para la Cuenta a editar: ");
            string elNombre = Console.ReadLine();

            EditeLaCuenta(elIdDeEntidadComoNumero, elIdMonedaComoNumero, elNombre);

            ImprimaLosDatosActuales();

            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();
        }

        private static void ImprimaLosDatosActuales()
        {
            Console.WriteLine("CONSULTANDO los datos actuales de cada cuenta:");
            var lasCuentas = ListeLasCuentas();

            foreach (var unaCuenta in lasCuentas)
            {
                Console.WriteLine(unaCuenta.IdEntidad + " " + unaCuenta.IdMoneda + " " + unaCuenta.Nombre + " " + unaCuenta.Estado + " " + unaCuenta.FechaDeActualizacion);
            }
        }

        public static IEnumerable<CuentaVigente> ListeLasCuentas()
        {
            IEnumerable<CuentaVigente> lasCuentas;
            using (var elContexto = new CuentasContext())
            {
                DbSet<Cuenta> todasLasCuentas = ObtengaTodasLasCuentas(elContexto);
                lasCuentas = ObtengaLasCuentasAplanadas(todasLasCuentas);
            }

            return lasCuentas;
        }

        private static DbSet<Cuenta> ObtengaTodasLasCuentas(CuentasContext elContexto)
        {
            using (var db = new CuentasContext())
            {
                // Obtenga las cuentas aplanadas con el historico mas reciente
                IQueryable<CuentaVigente> lasCuentas;
                lasCuentas = from cadaCuenta in db.Cuentas
                             let losHistoricos = cadaCuenta.Historicos
                             let losOrdenados = losHistoricos.OrderByDescending(x => x.FechaDeModificacion)
                             let elMasReciente = losOrdenados.FirstOrDefault()
                             select new CuentaVigente()
                             {
                                 IdEntidad = cadaCuenta.IdEntidad,
                                 IdMoneda = cadaCuenta.IdMoneda,
                                 Nombre = elMasReciente.Nombre,
                                 FechaDeActualizacion = elMasReciente.FechaDeModificacion,
                                 Estado = elMasReciente.Estado
                             };

                Console.WriteLine("CONSULTANDO los datos actuales de cada cuenta:");
                foreach (var unaCuenta in lasCuentas)
                {
                    Console.WriteLine(unaCuenta.IdEntidad + " " + unaCuenta.IdMoneda + " " + unaCuenta.Nombre + " " + unaCuenta.Estado + " " + unaCuenta.FechaDeActualizacion);
                }
            }
        }

        public static void EditeLaCuenta(int elIdEntidadComoNumero, int elIdMonedaComoNumero, string elNombre)
        {
            Trace.WriteLine($"Editando la cuenta IdEntidad: {elIdEntidadComoNumero} IdMoneda: {elIdMonedaComoNumero} Nombre: {elNombre}");

            using (var db = new CuentasContext())
            {
                var elNuevoHistorico = new Historico
                {
                    IdEntidad = elIdEntidadComoNumero,
                    IdMoneda = elIdMonedaComoNumero,
                    Nombre = elNombre,
                    FechaDeModificacion = DateTime.Now,
                    Estado = 2
                };

                db.Historicos.Add(elNuevoHistorico);

                db.SaveChanges();
            }
        }
    }
}