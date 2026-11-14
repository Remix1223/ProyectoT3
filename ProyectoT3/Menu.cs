public class Menu
{
    private Carta carta = new Carta();

    public void Iniciar()
    {
        try
        {
            carta.Preparar();
        }
        catch (Exception ex)
        {
            Console.WriteLine("No se pudo conectar a la base de datos: " + ex.Message);
            return;
        }

        string opcion = "";

        while (opcion != "6")
        {
            Console.WriteLine();
            Console.WriteLine("MENU DEL RESTAURANTE");
            Console.WriteLine("1. Agregar plato");
            Console.WriteLine("2. Buscar plato");
            Console.WriteLine("3. Modificar plato");
            Console.WriteLine("4. Eliminar plato");
            Console.WriteLine("5. Listar platos");
            Console.WriteLine("6. Salir");
            Console.Write("Opcion: ");
            opcion = (Console.ReadLine() ?? "").Trim();

            try
            {
                switch (opcion)
                {
                    case "1":
                        Agregar();
                        break;
                    case "2":
                        Buscar();
                        break;
                    case "3":
                        Modificar();
                        break;
                    case "4":
                        Eliminar();
                        break;
                    case "5":
                        Mostrar(carta.Listar());
                        break;
                    case "6":
                        Console.WriteLine("Fin del programa.");
                        break;
                    default:
                        Console.WriteLine("Opcion invalida.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }

    private string LeerTexto(string mensaje)
    {
        Console.Write(mensaje);
        return (Console.ReadLine() ?? "").Trim();
    }

    private int LeerEntero(string mensaje)
    {
        int numero;
        Console.Write(mensaje);

        while (!int.TryParse(Console.ReadLine(), out numero))
        {
            Console.Write("Numero invalido, intente de nuevo: ");
        }

        return numero;
    }

    private decimal LeerDecimal(string mensaje)
    {
        decimal numero;
        Console.Write(mensaje);

        while (!decimal.TryParse(Console.ReadLine(), out numero))
        {
            Console.Write("Valor invalido, intente de nuevo: ");
        }

        return numero;
    }

    private void Mostrar(List<Plato> lista)
    {
        if (lista.Count == 0)
        {
            Console.WriteLine("No hay platos.");
            return;
        }

        foreach (Plato p in lista)
        {
            Console.WriteLine(p.Id + " - " + p.Nombre + " - " + p.Categoria.Nombre + " - " + p.Precio.Valor);
        }
    }

    private void Agregar()
    {
        string nombre = LeerTexto("Nombre: ");
        string categoria = LeerTexto("Categoria: ");
        decimal precio = LeerDecimal("Precio: ");

        carta.Agregar(nombre, categoria, precio);
        Console.WriteLine("Plato agregado.");
    }

    private void Buscar()
    {
        Console.WriteLine("1. Por ID");
        Console.WriteLine("2. Por nombre");
        Console.WriteLine("3. Por categoria");
        Console.WriteLine("4. Por rango de precio");
        string criterio = LeerTexto("Buscar por: ");

        if (criterio == "1")
        {
            int id = LeerEntero("ID: ");
            Plato? plato = carta.BuscarPorId(id);
            List<Plato> lista = new List<Plato>();

            if (plato != null)
            {
                lista.Add(plato);
            }

            Mostrar(lista);
        }
        else if (criterio == "2")
        {
            Mostrar(carta.BuscarPorNombre(LeerTexto("Nombre: ")));
        }
        else if (criterio == "3")
        {
            Mostrar(carta.BuscarPorCategoria(LeerTexto("Categoria: ")));
        }
        else if (criterio == "4")
        {
            decimal minimo = LeerDecimal("Precio minimo: ");
            decimal maximo = LeerDecimal("Precio maximo: ");
            Mostrar(carta.BuscarPorPrecio(minimo, maximo));
        }
        else
        {
            Console.WriteLine("Opcion invalida.");
        }
    }

    private void Modificar()
    {
        int id = LeerEntero("ID del plato a modificar: ");
        string nombre = LeerTexto("Nuevo nombre: ");
        string categoria = LeerTexto("Nueva categoria: ");
        decimal precio = LeerDecimal("Nuevo precio: ");

        carta.Modificar(id, nombre, categoria, precio);
        Console.WriteLine("Plato modificado.");
    }

    private void Eliminar()
    {
        int id = LeerEntero("ID del plato a eliminar: ");

        carta.Eliminar(id);
        Console.WriteLine("Plato eliminado.");
    }
}