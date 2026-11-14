public class Carta
{
    private string cadena = "Data Source=restaurante.db;Foreign Keys=True";

    private void Ejecutar(string sql, params SqliteParameter[] parametros)
    {
        SqliteConnection con = new SqliteConnection(cadena);

        try
        {
            con.Open();
            SqliteCommand cmd = new SqliteCommand(sql, con);

            foreach (SqliteParameter p in parametros)
            {
                cmd.Parameters.Add(p);
            }

            cmd.ExecuteNonQuery();
        }
        finally
        {
            con.Close();
        }
    }

    private List<Plato> Consultar(string filtro, params SqliteParameter[] parametros)
    {
        List<Plato> resultado = new List<Plato>();
        SqliteConnection con = new SqliteConnection(cadena);

        try
        {
            con.Open();
            string sql = "SELECT p.Id, p.Nombre, c.Id, c.Nombre, p.Precio FROM Platos p INNER JOIN Categorias c ON p.CategoriaId = c.Id " + filtro + " ORDER BY p.Id";
            SqliteCommand cmd = new SqliteCommand(sql, con);

            foreach (SqliteParameter p in parametros)
            {
                cmd.Parameters.Add(p);
            }

            SqliteDataReader lector = cmd.ExecuteReader();

            while (lector.Read())
            {
                Categoria categoria = new Categoria(lector.GetInt32(2), lector.GetString(3));
                Plato plato = new Plato(lector.GetInt32(0), lector.GetString(1), categoria, new Precio(Convert.ToDecimal(lector.GetDouble(4))));
                resultado.Add(plato);
            }

            lector.Close();
        }
        finally
        {
            con.Close();
        }

        return resultado;
    }

    private Categoria ObtenerCategoria(string nombre)
    {
        SqliteConnection con = new SqliteConnection(cadena);

        try
        {
            con.Open();
            SqliteCommand buscar = new SqliteCommand("SELECT Id, Nombre FROM Categorias WHERE Nombre = @n", con);
            buscar.Parameters.AddWithValue("@n", nombre);
            SqliteDataReader lector = buscar.ExecuteReader();

            if (lector.Read())
            {
                Categoria existente = new Categoria(lector.GetInt32(0), lector.GetString(1));
                lector.Close();
                return existente;
            }

            lector.Close();
            SqliteCommand crear = new SqliteCommand("INSERT INTO Categorias (Nombre) VALUES (@n); SELECT last_insert_rowid();", con);
            crear.Parameters.AddWithValue("@n", nombre);
            int id = Convert.ToInt32(crear.ExecuteScalar());
            return new Categoria(id, nombre);
        }
        finally
        {
            con.Close();
        }
    }

    public void Preparar()
    {
        Ejecutar("CREATE TABLE IF NOT EXISTS Categorias (Id INTEGER PRIMARY KEY AUTOINCREMENT, Nombre TEXT NOT NULL UNIQUE COLLATE NOCASE)");
        Ejecutar("CREATE TABLE IF NOT EXISTS Platos (Id INTEGER PRIMARY KEY AUTOINCREMENT, Nombre TEXT NOT NULL, CategoriaId INTEGER NOT NULL, Precio REAL NOT NULL, FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id))");
    }

    public void Agregar(string nombre, string categoria, decimal precio)
    {
        if (nombre == "" || categoria == "")
        {
            throw new Exception("Los campos no pueden estar vacios.");
        }

        if (precio <= 0)
        {
            throw new Exception("El precio debe ser mayor que cero.");
        }

        Categoria cat = ObtenerCategoria(categoria);

        Ejecutar("INSERT INTO Platos (Nombre, CategoriaId, Precio) VALUES (@n, @c, @p)",
            new SqliteParameter("@n", nombre),
            new SqliteParameter("@c", cat.Id),
            new SqliteParameter("@p", Convert.ToDouble(precio)));
    }

    public List<Plato> Listar()
    {
        return Consultar("");
    }

    public Plato? BuscarPorId(int id)
    {
        List<Plato> lista = Consultar("WHERE p.Id = @id", new SqliteParameter("@id", id));

        if (lista.Count == 0)
        {
            return null;
        }

        return lista[0];
    }

    public List<Plato> BuscarPorNombre(string texto)
    {
        return Consultar("WHERE p.Nombre LIKE @t", new SqliteParameter("@t", "%" + texto + "%"));
    }

    public List<Plato> BuscarPorCategoria(string texto)
    {
        return Consultar("WHERE c.Nombre LIKE @t", new SqliteParameter("@t", "%" + texto + "%"));
    }

    public List<Plato> BuscarPorPrecio(decimal minimo, decimal maximo)
    {
        if (minimo > maximo)
        {
            throw new Exception("El precio minimo no puede ser mayor que el maximo.");
        }

        return Consultar("WHERE p.Precio BETWEEN @min AND @max",
            new SqliteParameter("@min", Convert.ToDouble(minimo)),
            new SqliteParameter("@max", Convert.ToDouble(maximo)));
    }

    public void Modificar(int id, string nombre, string categoria, decimal precio)
    {
        if (BuscarPorId(id) == null)
        {
            throw new Exception("No existe un plato con ese ID.");
        }

        if (nombre == "" || categoria == "")
        {
            throw new Exception("Los campos no pueden estar vacios.");
        }

        if (precio <= 0)
        {
            throw new Exception("El precio debe ser mayor que cero.");
        }

        Categoria cat = ObtenerCategoria(categoria);

        Ejecutar("UPDATE Platos SET Nombre = @n, CategoriaId = @c, Precio = @p WHERE Id = @id",
            new SqliteParameter("@n", nombre),
            new SqliteParameter("@c", cat.Id),
            new SqliteParameter("@p", Convert.ToDouble(precio)),
            new SqliteParameter("@id", id));
    }

    public void Eliminar(int id)
    {
        if (BuscarPorId(id) == null)
        {
            throw new Exception("No existe un plato con ese ID.");
        }

        Ejecutar("DELETE FROM Platos WHERE Id = @id", new SqliteParameter("@id", id));
    }
}