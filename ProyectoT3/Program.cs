SqliteConnection con = new SqliteConnection("Data Source=restaurante.db;Foreign Keys=True");

try
{
    con.Open();

    SqliteCommand tabla = con.CreateCommand();
    tabla.CommandText = "CREATE TABLE IF NOT EXISTS Categorias (Id INTEGER PRIMARY KEY AUTOINCREMENT, Nombre TEXT NOT NULL UNIQUE COLLATE NOCASE)";
    tabla.ExecuteNonQuery();
    tabla.CommandText = "CREATE TABLE IF NOT EXISTS Platos (Id INTEGER PRIMARY KEY AUTOINCREMENT, Nombre TEXT NOT NULL, CategoriaId INTEGER NOT NULL, Precio REAL NOT NULL, FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id))";
    tabla.ExecuteNonQuery();
}
catch (Exception ex)
{
    Console.WriteLine("No se pudo preparar la base de datos: " + ex.Message);
    return;
}

string selectPlatos = "SELECT p.Id, p.Nombre, c.Id, c.Nombre, p.Precio FROM Platos p INNER JOIN Categorias c ON p.CategoriaId = c.Id ";
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

    string? filtro = null;
    object valor = "";
    object valor2 = "";

    try
    {
        switch (opcion)
        {
            case "1":
                {
                    Console.Write("Nombre: ");
                    string nombre = (Console.ReadLine() ?? "").Trim();
                    Console.Write("Categoria: ");
                    string categoria = (Console.ReadLine() ?? "").Trim();
                    Console.Write("Precio: ");
                    decimal precio;
                    while (!decimal.TryParse(Console.ReadLine(), out precio))
                    {
                        Console.Write("Valor invalido, intente de nuevo: ");
                    }

                    if (nombre == "" || categoria == "")
                    {
                        throw new Exception("Los campos no pueden estar vacios.");
                    }

                    if (precio <= 0)
                    {
                        throw new Exception("El precio debe ser mayor que cero.");
                    }

                    SqliteCommand buscarCategoria = con.CreateCommand();
                    buscarCategoria.CommandText = "SELECT Id FROM Categorias WHERE Nombre = @n";
                    buscarCategoria.Parameters.AddWithValue("@n", categoria);
                    object? existente = buscarCategoria.ExecuteScalar();
                    int categoriaId;

                    if (existente != null)
                    {
                        categoriaId = Convert.ToInt32(existente);
                    }
                    else
                    {
                        SqliteCommand crearCategoria = con.CreateCommand();
                        crearCategoria.CommandText = "INSERT INTO Categorias (Nombre) VALUES (@n); SELECT last_insert_rowid();";
                        crearCategoria.Parameters.AddWithValue("@n", categoria);
                        categoriaId = Convert.ToInt32(crearCategoria.ExecuteScalar());
                    }

                    SqliteCommand insertar = con.CreateCommand();
                    insertar.CommandText = "INSERT INTO Platos (Nombre, CategoriaId, Precio) VALUES (@n, @c, @p)";
                    insertar.Parameters.AddWithValue("@n", nombre);
                    insertar.Parameters.AddWithValue("@c", categoriaId);
                    insertar.Parameters.AddWithValue("@p", Convert.ToDouble(precio));
                    insertar.ExecuteNonQuery();

                    Console.WriteLine("Plato agregado.");
                    break;
                }
            case "2":
                {
                    Console.WriteLine("1. Por ID");
                    Console.WriteLine("2. Por nombre");
                    Console.WriteLine("3. Por categoria");
                    Console.WriteLine("4. Por rango de precio");
                    Console.Write("Buscar por: ");
                    string criterio = (Console.ReadLine() ?? "").Trim();

                    if (criterio == "1")
                    {
                        Console.Write("ID: ");
                        int idBuscado;
                        while (!int.TryParse(Console.ReadLine(), out idBuscado))
                        {
                            Console.Write("Numero invalido, intente de nuevo: ");
                        }
                        filtro = "WHERE p.Id = @v";
                        valor = idBuscado;
                    }
                    else if (criterio == "2")
                    {
                        Console.Write("Nombre: ");
                        valor = "%" + (Console.ReadLine() ?? "").Trim() + "%";
                        filtro = "WHERE p.Nombre LIKE @v";
                    }
                    else if (criterio == "3")
                    {
                        Console.Write("Categoria: ");
                        valor = "%" + (Console.ReadLine() ?? "").Trim() + "%";
                        filtro = "WHERE c.Nombre LIKE @v";
                    }
                    else if (criterio == "4")
                    {
                        Console.Write("Precio minimo: ");
                        decimal minimo;
                        while (!decimal.TryParse(Console.ReadLine(), out minimo))
                        {
                            Console.Write("Valor invalido, intente de nuevo: ");
                        }

                        Console.Write("Precio maximo: ");
                        decimal maximo;
                        while (!decimal.TryParse(Console.ReadLine(), out maximo))
                        {
                            Console.Write("Valor invalido, intente de nuevo: ");
                        }

                        if (minimo > maximo)
                        {
                            throw new Exception("El precio minimo no puede ser mayor que el maximo.");
                        }

                        filtro = "WHERE p.Precio BETWEEN @v AND @w";
                        valor = Convert.ToDouble(minimo);
                        valor2 = Convert.ToDouble(maximo);
                    }
                    else
                    {
                        Console.WriteLine("Opcion invalida.");
                    }
                    break;
                }
            case "3":
                {
                    Console.Write("ID del plato a modificar: ");
                    int id;
                    while (!int.TryParse(Console.ReadLine(), out id))
                    {
                        Console.Write("Numero invalido, intente de nuevo: ");
                    }

                    SqliteCommand verificar = con.CreateCommand();
                    verificar.CommandText = "SELECT COUNT(*) FROM Platos WHERE Id = @id";
                    verificar.Parameters.AddWithValue("@id", id);

                    if (Convert.ToInt32(verificar.ExecuteScalar()) == 0)
                    {
                        throw new Exception("No existe un plato con ese ID.");
                    }

                    Console.Write("Nuevo nombre: ");
                    string nombre = (Console.ReadLine() ?? "").Trim();
                    Console.Write("Nueva categoria: ");
                    string categoria = (Console.ReadLine() ?? "").Trim();
                    Console.Write("Nuevo precio: ");
                    decimal precio;
                    while (!decimal.TryParse(Console.ReadLine(), out precio))
                    {
                        Console.Write("Valor invalido, intente de nuevo: ");
                    }

                    if (nombre == "" || categoria == "")
                    {
                        throw new Exception("Los campos no pueden estar vacios.");
                    }

                    if (precio <= 0)
                    {
                        throw new Exception("El precio debe ser mayor que cero.");
                    }

                    SqliteCommand buscarCategoria = con.CreateCommand();
                    buscarCategoria.CommandText = "SELECT Id FROM Categorias WHERE Nombre = @n";
                    buscarCategoria.Parameters.AddWithValue("@n", categoria);
                    object? existente = buscarCategoria.ExecuteScalar();
                    int categoriaId;

                    if (existente != null)
                    {
                        categoriaId = Convert.ToInt32(existente);
                    }
                    else
                    {
                        SqliteCommand crearCategoria = con.CreateCommand();
                        crearCategoria.CommandText = "INSERT INTO Categorias (Nombre) VALUES (@n); SELECT last_insert_rowid();";
                        crearCategoria.Parameters.AddWithValue("@n", categoria);
                        categoriaId = Convert.ToInt32(crearCategoria.ExecuteScalar());
                    }

                    SqliteCommand actualizar = con.CreateCommand();
                    actualizar.CommandText = "UPDATE Platos SET Nombre = @n, CategoriaId = @c, Precio = @p WHERE Id = @id";
                    actualizar.Parameters.AddWithValue("@n", nombre);
                    actualizar.Parameters.AddWithValue("@c", categoriaId);
                    actualizar.Parameters.AddWithValue("@p", Convert.ToDouble(precio));
                    actualizar.Parameters.AddWithValue("@id", id);
                    actualizar.ExecuteNonQuery();

                    Console.WriteLine("Plato modificado.");
                    break;
                }
            case "4":
                {
                    Console.Write("ID del plato a eliminar: ");
                    int id;
                    while (!int.TryParse(Console.ReadLine(), out id))
                    {
                        Console.Write("Numero invalido, intente de nuevo: ");
                    }

                    SqliteCommand verificar = con.CreateCommand();
                    verificar.CommandText = "SELECT COUNT(*) FROM Platos WHERE Id = @id";
                    verificar.Parameters.AddWithValue("@id", id);

                    if (Convert.ToInt32(verificar.ExecuteScalar()) == 0)
                    {
                        throw new Exception("No existe un plato con ese ID.");
                    }

                    SqliteCommand borrar = con.CreateCommand();
                    borrar.CommandText = "DELETE FROM Platos WHERE Id = @id";
                    borrar.Parameters.AddWithValue("@id", id);
                    borrar.ExecuteNonQuery();

                    Console.WriteLine("Plato eliminado.");
                    break;
                }
            case "5":
                {
                    filtro = "";
                    break;
                }
            case "6":
                {
                    Console.WriteLine("Fin del programa.");
                    break;
                }
            default:
                {
                    Console.WriteLine("Opcion invalida.");
                    break;
                }
        }

        if (filtro != null)
        {
            SqliteCommand buscar = con.CreateCommand();
            buscar.CommandText = selectPlatos + filtro + " ORDER BY p.Id";

            if (filtro.Contains("@v"))
            {
                buscar.Parameters.AddWithValue("@v", valor);
            }

            if (filtro.Contains("@w"))
            {
                buscar.Parameters.AddWithValue("@w", valor2);
            }

            List<Plato> platos = new List<Plato>();
            SqliteDataReader lector = buscar.ExecuteReader();

            while (lector.Read())
            {
                platos.Add(new Plato(
                    lector.GetInt32(0),
                    lector.GetString(1),
                    new Categoria(lector.GetInt32(2), lector.GetString(3)),
                    new Precio(Convert.ToDecimal(lector.GetDouble(4)))));
            }

            lector.Close();

            if (platos.Count == 0)
            {
                Console.WriteLine("No hay platos.");
            }

            foreach (Plato p in platos)
            {
                Console.WriteLine(p.Id + " - " + p.Nombre + " - " + p.Categoria.Nombre + " - " + p.Precio.Valor);
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}

con.Close();