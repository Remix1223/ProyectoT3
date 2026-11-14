public class Plato
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public Categoria Categoria { get; set; }
    public Precio Precio { get; set; }

    public Plato(int id, string nombre, Categoria categoria, Precio precio)
    {
        Id = id;
        Nombre = nombre;
        Categoria = categoria;
        Precio = precio;
    }
}