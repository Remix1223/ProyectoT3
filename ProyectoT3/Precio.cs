public class Precio
{
    public decimal Valor { get; set; }

    public Precio(decimal valor)
    {
        Valor = valor;
    }

    public override string ToString()
    {
        return "RD$ " + Valor.ToString("N2");
    }
}